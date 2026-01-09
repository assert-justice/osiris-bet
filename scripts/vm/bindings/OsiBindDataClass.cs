using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime.Interop;
using Osiris.Data;
using Osiris.System;
using Prion.Node;

namespace Osiris.Vm;

public class DataClassWrapper
{
    protected readonly Guid Id;
    protected readonly string GroupName;
    DataClassWrapper(Guid id)
    {
        Id = id;
        GroupName = GetObject<OsiData>().Group;
        if(GroupName is null)
        {
            OsiSystem.Session.EventHandler.TryGetGroupByType(GetType(), out GroupName);
        }
    }
    protected T GetObject<T>() where T : OsiData
    {
        return OsiSystem.Session.TryGetObject(Id, out T obj) ? obj : null;
    }
    public DataClassWrapper(string groupName = null)
    {
        Id = Guid.NewGuid();
        OsiEvent.EmitEventStandard(Id, groupName, new PrionNull());
    }
    public static T WrapInternal<T>(Guid id) where T : DataClassWrapper
    {
        return new DataClassWrapper(id) as T;
    }
    public static DataClassWrapper wrap(Guid id)
    {
        var eventHandler = OsiSystem.Session.EventHandler;
        // Todo: more error handling.
        if(!eventHandler.TryGetGroupName(id, out string groupName)) return null;
        if(!eventHandler.TryCallWrapper(groupName, id, out var wrapper)) return null;
        return wrapper;
    }
    public static void inherit(ScriptFunction classObj, ScriptFunction baseClassObj, string[] methods, bool isSealed = false)
    {
        var vm = OsiSystem.Session.Vm;
        if(!vm.TryImportModule("utils", out var module)) return;
        // registerClass(classObj, baseName, methodNames, isSealed = false)
        JsArray jsMethods = new(vm.Engine, [..methods]);
        module.TryCall("registerClass", classObj, baseClassObj.GetOwnProperty("name").Value, jsMethods, isSealed);
    }
    public void applyEvent(string verb, JsValue jsValue)
    {
        string jsonString = OsiSystem.Session.Vm.ToJsonString(jsValue);
        var jsonNode = JsonNode.Parse(jsonString);
        if(!PrionNode.TryFromJson(jsonNode, out var prionNode, out string error))
        {
            OsiSystem.Logger.ReportError(error);
            return;
        }
        OsiEvent.EmitEvent(Id, verb, prionNode);
    }

    public Guid getId(){return Id;}
}

public static class OsiBindDataClass
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var obj = TypeReference.CreateTypeReference<BlobWrapper>(vm.Engine);
        module.Add("DataClass", obj);

        // register group
        OsiGroup.CreateGroup<OsiData, DataClassWrapper>("DataClass", [], (id, group)=>new OsiData(id, group));
    }
}