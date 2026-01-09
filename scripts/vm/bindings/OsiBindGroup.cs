using System;
using System.Collections.Generic;
using Jint.Native;
using Jint.Runtime.Interop;
using Osiris.Data;
using Osiris.System;

namespace Osiris.Vm;

public class GroupWrapper(string groupName, string baseName, bool isSealed = false)
{
    readonly string GroupName = groupName;
    readonly string BaseName = baseName;
    readonly bool IsSealed = isSealed;
    readonly bool IsFinished = false;
    readonly List<(string, Action<OsiData, OsiEvent>)> Methods = [];

    public void addMethod(string methodName, Action<DataClassWrapper, JsValue> method)
    {
        if(IsFinished)
        {
            OsiSystem.Logger.ReportError($"Attempted to add a method to finished group '{GroupName}'.");
            return;
        }
        void closure(OsiData blob, OsiEvent osiEvent)
        {
            if(!OsiSystem.Session.EventHandler.TryCallWrapper(GroupName, blob.Id, out var wrapper)) return;
            method(wrapper, OsiSystem.Session.Vm.ParseJson(osiEvent.Payload.ToJson().ToJsonString()));
        }
        Methods.Add((methodName, closure));
    }
    public void finish()
    {
        OsiGroup.CreateGroup(GroupName, [..Methods], BaseName, IsSealed);
    }
}

public static class OsiBindGroup
{
    public static void Bind(OsiVm vm, Dictionary<string, JsValue> module)
    {
        var obj = TypeReference.CreateTypeReference<GroupWrapper>(vm.Engine);
        module.Add("Group", obj);
    }
}