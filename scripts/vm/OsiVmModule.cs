using System;
using Jint;
using Jint.Native.Object;
using Osiris.System;

namespace Osiris.Vm;

public class OsiVmModule(OsiVm vm, ObjectInstance obj)
{
	public OsiVm Vm = vm;
    public ObjectInstance Object = obj;
    public bool TryCall(string functionName)
    {
		// return vm.Try(e =>
		// {
			
		// });
        try
		{
			Object.Get(functionName).Call();
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
