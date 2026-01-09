using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Jint.Native;
using Osiris.Data;
using Osiris.System;
using Prion.Node;

namespace Osiris.Vm;

public static class OsiBindDice
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var diceModule = new JsObject(vm.Engine);
        // evaluate(formula: string): object;
        Func<string,JsValue> evaluate = (formula) => vm.ParseJson(OsiDiceHandler.Evaluate(formula));
        diceModule.Set("evaluate", JsValue.FromObject(vm.Engine, evaluate));
        // setEvaluator(evaluator: (formula: string) => object): void;
        Action<Func<JsValue, object>> setEvaluator = (jsEvaluator) =>
        {
            string evaluator(string formula)
            {
                var val = JsValue.FromObject(vm.Engine, jsEvaluator(formula)); // This is dumb. Why.
                string jsonString = vm.ToJsonString(val);
                return jsonString;
            }
            OsiDiceHandler.SetEvaluator(evaluator);
        };
        diceModule.Set("setEvaluator", JsValue.FromObject(vm.Engine, setEvaluator));
        // rollDice(count: number, size: number): number[];
        // Func<int, int, int[]> rollDice = OsiDiceHandler.RollDice;
        Func<int, int, JsArray> rollDice = (number, size) =>
        {
            var roll = OsiDiceHandler.RollDice(number, size);
            return new JsArray(vm.Engine, [..roll.Select(n => new JsNumber(n))]);
        };
        diceModule.Set("rollDice", JsValue.FromObject(vm.Engine, rollDice));
        // requestRoll(formula: string, callback: (result: object)=>void): void;
        var eventHandler = OsiSystem.Session.EventHandler;
        Action<string, Action<JsValue>> requestRoll = (formula, jsCallback) =>
        {
            Guid callbackId = Guid.NewGuid();
            void callback(PrionNode payload)
            {
                if(!payload.TryAs(out PrionString result)) return;
                jsCallback(vm.ParseJson(result.Value));
            }
            eventHandler.AddCallback(callbackId, callback);
            // PrionDict dict = new();
            // dict.Set("callback_name", callbackId);
            // dict.Set("formula", formula);
            OsiEvent.EmitEvent(callbackId, "request_roll", new PrionString(formula));
        };
        diceModule.Set("requestRoll", JsValue.FromObject(vm.Engine, requestRoll));
        module.Add("Dice", diceModule);

        // Bind event handlers
        eventHandler.SetGlobalMethod("request_roll", e =>
        {
            if(!e.Payload.TryAs(out PrionString formula)) return;
            // if(!dict.TryGet("callback_name", out PrionString callbackName)) return;
            // if(!dict.TryGet("formula", out string formula)) return;
            string result = OsiDiceHandler.Evaluate(formula.Value);
            OsiEvent.EmitEventSingle(e.TargetId, "invoke_callback", new PrionString(result), e.UserId);
        });
    }
}
