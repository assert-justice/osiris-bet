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
    readonly List<(string, Action<OsiBlob, OsiEvent>)> Methods = [];

    public void addMethod(string methodName, Action<BlobWrapper, JsValue> method)
    {
        if(IsFinished)
        {
            OsiSystem.Logger.ReportError($"Attempted to add a method to finished group '{GroupName}'.");
            return;
        }
        void closure(OsiBlob blob, OsiEvent osiEvent)
        {
            method(new BlobWrapper(blob.Id), OsiSystem.Session.Vm.ParseJson(osiEvent.Payload.ToJson().ToJsonString()));
        }
        Methods.Add((methodName, closure));
    }
    public void finish()
    {
        static OsiBlob constructor(OsiEvent osiEvent)
        {
            if(OsiBlob.TryFromNode(osiEvent.Payload, out var blob)) return blob;
            OsiSystem.Logger.Log("Could not parse payload as OsiBlob.");
            return null;
        }
        OsiSystem.Session.EventHandler.AddGroup(GroupName, BaseName, constructor, [..Methods]);
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