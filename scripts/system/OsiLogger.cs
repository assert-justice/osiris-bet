using System.Collections.Generic;
using Godot;

namespace Osiris.System;

public class OsiLogger
{
    public List<string> MessageLog = [];
    public List<string> ErrorLog = [];
    public void Log(params object[] messages)
    {
        string message = "";
		foreach (var m in messages)
		{
			message += m;
		}
		MessageLog.Add(message);
		GD.Print(message);
    }
    public void ReportError(params object[] messages)
    {
        string message = "";
		foreach (var m in messages)
		{
			message += m;
		}
		ErrorLog.Add(message);
		GD.PrintErr(message);
    }
}
