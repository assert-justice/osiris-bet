using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Jint.Native;
using Osiris.Data;
using Osiris.Vm;
using Prion.Node;
using Prion.Schema;

namespace Osiris.System;

public static class OsiLoadProject
{
	public static void Load()
	{
		// Handle user stuff
		OsiUser user = new(Guid.NewGuid())
		{
			DisplayName = "Riley"
		};
		// OsiSystem.Logger.Log($"user id: {user.Id}");
		OsiSystem.SetUser(user);
		OsiSystem.Session.HostId = user.Id;
		OsiSystem.Session.Gms.Add(user.Id);
		// Bind Osiris vm modules
		var vm = OsiSystem.Session.Vm;
		Dictionary<string, JsValue> dict = [];
		OsiBindLogging.Bind(vm, dict);
		OsiBindDice.Bind(vm, dict);
		OsiBindBlob.Bind(vm, dict);
		OsiBindGroup.Bind(vm, dict);
		OsiBindMap.Bind(vm, dict);
		vm.AddModule("Osiris", dict);
		// Load contents of project
		string projectPath = "res://example_project";
		// Load schemas
		PrionSchemaManager.Clear();
		string schemaPath = projectPath + "/schemas";
		foreach (var item in OsiSystem.Fs.DirectoryListFiles(schemaPath))
		{
			if(!item.EndsWith(".json")) continue;
			string src = OsiSystem.Fs.ReadFile($"{schemaPath}/{item}");
			var json = JsonNode.Parse(src);
			if(!PrionNode.TryFromJson(json, out PrionNode node, out string error))
			{
				OsiSystem.Logger.ReportError($"Failed to register schema at {item} with error: {error}");
				continue;
			}
			if(!PrionSchema.TryFromPrionNode(node, out var schema, out error))
			{
				OsiSystem.Logger.ReportError($"Failed to register schema at {item} with error: {error}");
				continue;
			}
			PrionSchemaManager.RegisterSchema(schema);
		}
		// Load scripts
		string scriptPath = projectPath + "/scripts";
		foreach (var item in OsiSystem.Fs.DirectoryListFiles(scriptPath))
		{
			if(!item.EndsWith(".js")) continue;
			string src = OsiSystem.Fs.ReadFile($"{scriptPath}/{item}");
			vm.TryAddModule(item[..^3], src);
		}
		// Call init method on main script
		if(!vm.TryImportModule("main", out var main)) OsiSystem.Logger.ReportError("No main file found");
		else main.TryCall("init");
	}
}
