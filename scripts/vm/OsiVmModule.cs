using System;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Osiris.System;

namespace Osiris.Vm;

public class OsiVmModule(OsiVm vm, ObjectInstance obj)
{
	public OsiVm Vm = vm;
    public ObjectInstance Object = obj;
    public bool TryCall(string functionName, params JsValue[] args)
    {
		// return vm.Try(e =>
		// {
			
		// });
        try
		{
			if(args.Length > 0) Object.Get(functionName).Call(args);
			else Object.Get(functionName).Call();
			return true;
		}
		catch (Exception e)
		{
			string message = e.ToString();
			OsiSystem.Logger.ReportError(message);
			return false;
		}
    }
}
