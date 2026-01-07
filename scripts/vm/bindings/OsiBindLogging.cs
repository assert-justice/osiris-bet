using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native;
using Osiris.System;

namespace Osiris.Vm;

public static class OsiBindLogging
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var logging = new JsObject(vm.Engine);
        var fn = JsValue.FromObject(vm.Engine, new Action<JsValue[]>(Log));
        logging.Set("log", fn);
        fn = JsValue.FromObject(vm.Engine, new Action<JsValue[]>(LogError));
        logging.Set("logError", fn);
        module.Add("Logging", logging);
    }
    public static void Log(params JsValue[] messages)
    {
        OsiSystem.Logger.Log([.. messages.Select(m => m.ToString())]);
    }
    public static void LogError(params JsValue[] messages)
    {
        OsiSystem.Logger.ReportError([.. messages.Select(m => m.ToString())]);
    }
}
