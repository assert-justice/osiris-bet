using System;
using System.Linq;
using Godot;
using Osiris.System;
using Prion.Node;

namespace Osiris.Data;

public static class OsiDiceHandler
{
    static Func<string,string> Evaluator;
    public static string Evaluate(string formula)
    {
        if (!OsiSystem.IsGm())
        {
            OsiSystem.Logger.ReportError("Only gms can evaluate dice formulas.");
            return default;
        }
        return Evaluator(formula);
    }
    public static void SetEvaluator(Func<string,string> evaluator){
        if (!OsiSystem.IsGm())
        {
            OsiSystem.Logger.ReportError("Only gms can set the dice evaluator function.");
            return;
        }
        Evaluator = evaluator;
    }
    public static int[] RollDice(int number, int size)
    {
        if (!OsiSystem.IsGm())
        {
            OsiSystem.Logger.ReportError("Only gms can roll dice directly.");
            return [];
        }
        return [..Enumerable.Range(0, number).Select((_)=> Mathf.CeilToInt(GD.Randf() * size))];
    }
    public static void Reset(){Evaluator = (_) =>
    {
        OsiSystem.Logger.ReportError("No dice evaluator function set.");
        return default;
    };}
}
