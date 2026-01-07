using System;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using Osiris.System;

namespace Osiris.Vm;

public class OsiVm
{
    public readonly Engine Engine;
	readonly JsonParser JsonParser;
	readonly JsonSerializer JsonSerializer;
    public OsiVm()
    {
        Engine = new Engine(options =>
		{
			options.LimitMemory(4_000_000); // limit memory usage to 4 mb. pretty conservative, will likely need to go up
			options.TimeoutInterval(TimeSpan.FromMilliseconds(500)); // limit timeout to 500ms. seems reasonable
			options.Strict = true;
		});
		JsonParser = new(Engine);
		JsonSerializer = new(Engine);
    }
    public JsValue ParseJson(string jsonString)
	{
		return JsonParser.Parse(jsonString);
	}
	public string ToJsonString(JsValue jsValue)
	{
		return JsonSerializer.Serialize(jsValue).ToString();
	}
	public bool Try(Action<Engine> action)
	{
		try
		{
			action(Engine);
			return true;
		}
		catch (Exception e)
		{
			string message = e.ToString();
			OsiSystem.Logger.ReportError(message);
			return false;
		}
	}
	public T Try<T>(Func<Engine, T> action)
	{
		try
		{
			return action(Engine);
		}
		catch (JavaScriptException e)
		{
			// foreach (var item in e.Data)
			// {
			// 	OsiSystem.Logger.ReportError(item);
			// }
			// string message = e.ToString();
			// Engine.Advanced.StackTrace
			OsiSystem.Logger.ReportError(e.StackTrace);
			
			return default;
		}
	}
	public void AddModule(string name, Dictionary<string, JsValue> module)
	{
		// Try(e =>
		// {
		// 	Engine.Modules.Add(name, mb =>
		// 	{
		// 		foreach (var (n, mod) in module)
		// 		{
		// 			mb.ExportValue(n, mod);
		// 		}
		// 	});
		// });
		try
		{
			Engine.Modules.Add(name, mb =>
			{
				foreach (var (n, mod) in module)
				{
					mb.ExportValue(n, mod);
				}
			});
		}
		catch (Exception e)
		{
			string message = e.ToString();
			OsiSystem.Logger.ReportError(message);
		}
	}
    public bool TryAddModule(string moduleName, string src)
	{
		try
		{
			Engine.Modules.Add(moduleName, src);
			return true;
		}
		catch (Exception e)
		{
			string message = e.ToString();
			OsiSystem.Logger.ReportError(message);
			return false;
		}
	}
	public bool TryImportModule(string name, out OsiVmModule vmModule)
	{
		vmModule = default;
		// vmModule = Try<OsiVmModule>(e =>
		// {
		// 	return new(this, Engine.Modules.Import(name));
		// });
		// return vmModule is not null;
		try
		{
			vmModule = new(this, Engine.Modules.Import(name));
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
