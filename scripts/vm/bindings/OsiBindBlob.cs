using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Jint.Native;
using Jint.Runtime.Interop;
using Osiris.Data;
using Osiris.System;
using Prion.Node;
using Prion.Schema;

namespace Osiris.Vm;

public class BlobWrapper(string group): DataClassWrapper(group)
{
    public JsValue data
    {
        get
        {
            var data = GetObject<OsiBlob>().GetData();
            if(data is null) return null;
            return OsiSystem.Session.Vm.ParseJson(data.ToJson().ToJsonString());
        }
        set
        {
            string jsonString = OsiSystem.Session.Vm.ToJsonString(value);
            var jsonNode = JsonNode.Parse(jsonString);
            if(!PrionDict.TryFromJson(jsonNode, out var prionNode, out string error))
            {
                OsiSystem.Logger.ReportError(error);
                return;
            }
            OsiEvent.EmitEvent(Id, "setData", prionNode);
        }
    }
    public JsValue getPath(string path)
    {
        var data = GetObject<OsiBlob>().GetData();
        if(data is null) return null;
        if(data.TryGetPath(path, out PrionNode res)) 
            return OsiSystem.Session.Vm.ParseJson(res.ToJson().ToJsonString());
        else return null;
    }
    public void setPath(string path, JsValue jsValue, bool canAdd = false, bool canChangeType = false)
    {
        string jsonString = OsiSystem.Session.Vm.ToJsonString(jsValue);
        var jsonNode = JsonNode.Parse(jsonString);
        if(!PrionNode.TryFromJson(jsonNode, out var prionNode, out string error))
        {
            OsiSystem.Logger.ReportError(error);
            return;
        }
        PrionDict payload = new();
        payload.Set("path", path);
        payload.Set("canAdd", canAdd);
        payload.Set("canChangeType", canChangeType);
        payload.Set("data", prionNode);
        OsiEvent.EmitEvent(Id, "setPath", payload);
    }
    public bool validate(string schema)
    {
        if(PrionSchemaManager.Validate(schema, GetObject<OsiBlob>().GetData(), out string error)) return true;
        OsiSystem.Logger.ReportError(error);
        return false;
    }
    public bool validatePath(string schema, string path)
    {
        var data = GetObject<OsiBlob>().GetData();
        if(data is null) return false;
        if(!data.TryGetPath(path, out PrionNode res)) return false;
        if(PrionSchemaManager.Validate(schema, res, out string error)) return true;
        OsiSystem.Logger.ReportError(error);
        return false;
    }
}

public static class OsiBindBlob
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var obj = TypeReference.CreateTypeReference<BlobWrapper>(vm.Engine);
        module.Add("Blob", obj);

        // register event handlers
        static OsiBlob constructor(OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionString res)) return null;
            return new OsiBlob(osiEvent.TargetId, res.Value);
        }
        static void setData(OsiBlob blob, OsiEvent osiEvent)
        {
            blob.SetData(osiEvent.Payload);
        }
        static void setPath(OsiBlob blob, OsiEvent osiEvent)
        {
            if(!osiEvent.Payload.TryAs(out PrionDict dict))
            {
                OsiSystem.Logger.ReportError("Could not parse event payload, wrong type.");
                return;
            }
            // Todo: more error logging.Maybe schema validation?
            if(!dict.TryGet("path", out string path)) return;
            if(!dict.TryGet("canAdd", out bool canAdd)) return;
            if(!dict.TryGet("canChangeType", out bool canChangeType)) return;
            if(!dict.TryGet("data", out PrionNode prionNode)) return;
            if(!blob.GetData().TrySetPath(path, prionNode, canAdd, canChangeType))
            {
                OsiSystem.Logger.ReportError($"Failed to set path '{path}'");
                return;
            }
        }
        OsiGroup.CreateGroup<OsiBlob,BlobWrapper>("Blob", [("setData",setData),("setPath",setPath)], constructor, "DataClass");
        OsiSystem.Session.EventHandler.SetTypeLookup<OsiBlob>("Blob");
    }
}
