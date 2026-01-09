using System;
using System.Collections.Generic;
using Osiris.System;
using Osiris.Vm;
using Prion.Node;
using ConstructorFn = System.Func<System.Guid, string, Osiris.Data.OsiData>;
using WrapperFn = System.Func<System.Guid, Osiris.Vm.DataClassWrapper>;
using NamedMethod = (string, System.Action<Osiris.Data.OsiData, Osiris.Data.OsiEvent>);
using System.Linq;

namespace Osiris.Data;
public class OsiGroup
{
    public readonly string Name;
    public readonly string BaseGroupName;
    public readonly ConstructorFn Constructor;
    public readonly WrapperFn Wrapper;
    public readonly bool IsSealed;
    public readonly Type DataType;
    public readonly Type WrapperType;
    readonly Dictionary<string, Action<OsiData, OsiEvent>> Methods = [];
    private OsiGroup(
        Type dataType,
        Type wrapperType,
        string name, 
        string baseGroupName,
        NamedMethod[] methods,
        bool isSealed,
        ConstructorFn constructor,
        WrapperFn wrapper)
    {
        DataType = dataType;
        WrapperType = wrapperType;
        Name = name;
        BaseGroupName = baseGroupName;
        Constructor = constructor;
        Wrapper = wrapper;
        IsSealed = isSealed;
        foreach (var (methodName, method) in methods)
        {
            Methods.Add(methodName, method);
        }
    }
    public static void CreateGroup<T,U>(
        string name,
        (string, Action<T, OsiEvent>)[] methods,
        Func<Guid, string, T> constructor = null,
        string baseGroupName = null, 
        bool isSealed = false)
        // Func<Guid,U> wrapper = null)
        where T : OsiData
        where U : DataClassWrapper
    {
        NamedMethod fnWrapper(string methodName, Action<T, OsiEvent> method)
        {
            void fn(OsiData obj, OsiEvent osiEvent)
            {
                if(obj is T data) method(data, osiEvent);
                else OsiSystem.Logger.Log($"Mismatched types, expected '{typeof(T)}' but received '{obj.GetType()}'");
            }
            return (methodName, fn);
        }

        NamedMethod[] fns = [.. methods.Select(entry =>
        {
            var(methodName, method) = entry;
            return fnWrapper(methodName, method);
        })];
        OsiGroup group = new(typeof(T), typeof(U), name, baseGroupName, fns, isSealed, constructor, DataClassWrapper.WrapInternal<U>);
        OsiSystem.Session.EventHandler.AddGroup(group);
    }
    public static void CreateGroup(string name, NamedMethod[] methods, string baseGroupName, bool isSealed)
    {
        OsiGroup group = new(null, null, name, baseGroupName, methods, isSealed, null, null);
        OsiSystem.Session.EventHandler.AddGroup(group);
    }
    public bool TryCallMethod(OsiData data, OsiEvent osiEvent)
    {
        if(!Methods.TryGetValue(osiEvent.Verb, out var method)) return false;
        method(data, osiEvent);
        return true;
    }
}


public class OsiEventHandler
{
    readonly List<OsiEvent> Events = [];
    readonly HashSet<Guid> EventIds = [];
    readonly Dictionary<string, OsiGroup> Groups = [];
    readonly Dictionary<string, Action<OsiEvent>> GlobalMethods = [];
    readonly Dictionary<Guid, Action<PrionNode>> Callbacks = [];
    readonly Dictionary<Type, string> TypeLookup = [];
    public void SetGlobalMethod(string name, Action<OsiEvent> method)
    {
        GlobalMethods.Add(name, method); // Note, prohibits duplicates. Todo: better error message.
    }
    public void AddCallback(Guid id, Action<PrionNode> action)
    {
        Callbacks.Add(id, action);
    }
    public void AddGroup(OsiGroup group)
    {
        if (Groups.ContainsKey(group.Name))
        {
            OsiSystem.Logger.ReportError($"A group of name '{group.Name}' already exists.");
            return;
        }
        if(group.BaseGroupName is not null)
        {
            if(!Groups.TryGetValue(group.BaseGroupName, out var baseGroup))
            {
                OsiSystem.Logger.ReportError($"No group of name '{group.BaseGroupName}' exists.");
                return;
            }
            if (baseGroup.IsSealed)
            {
                OsiSystem.Logger.ReportError($"Base group of name '{group.BaseGroupName}' is sealed and cannot be extended.");
                return;
            }
        }
        Groups.Add(group.Name, group);
    }
    public void DispatchEvent(OsiEvent osiEvent)
    {
        if (EventIds.Contains(osiEvent.Id))
        {
            OsiSystem.Logger.ReportError($"Duplicate event id {osiEvent.Id} detected.");
            return;
        }
        Events.Add(osiEvent);
        EventIds.Add(osiEvent.Id);
        if(!osiEvent.ShouldExecute()) return;

        if(osiEvent.Verb == "invoke_callback")
        {
            if(!TryInvokeCallback(osiEvent)) OsiSystem.Logger.ReportError("Failed to invoke callback.");
            return;
        }

        if (GlobalMethods.TryGetValue(osiEvent.Verb, out var action))
        {
            action(osiEvent);
            return;
        }

        if(!OsiSystem.Session.TryGetObject(osiEvent.TargetId, out OsiData data))
        {
            if(!TryCallConstructor(osiEvent.TargetId, osiEvent.Verb, out var _))
            {
                OsiSystem.Logger.ReportError($"Id '{osiEvent.TargetId}' not found and no group of name '{osiEvent.Verb}' exists. Using fallback.");
                OsiSystem.Session.AddObject(new OsiData(osiEvent.TargetId, osiEvent.Verb));
            }
            return;
        }
        string groupName = data.Group;
        if(groupName is null && !TryGetGroupByType(data.GetType(), out groupName))
        {
            OsiSystem.Logger.ReportError($"No group of name '{data.Group ?? data.GetType().ToString()}' exists, cannot call method '{osiEvent.Verb}'.");
        }
        if(TryCallMethod(data.Group, data, osiEvent)) return;
        OsiSystem.Logger.ReportError($"Could not call method '{osiEvent.Verb}' was found in group '{data.Group ?? data.GetType().ToString()}' or its ancestors.");
    }
    public bool TryGetGroupByType(Type type, out string groupName)
    {
        return TypeLookup.TryGetValue(type, out groupName);
    }
    public bool TryGetGroupName(Guid id, out string groupName)
    {
        groupName = default;
        if(!OsiSystem.Session.TryGetObject(id, out OsiData data))
        {
            OsiSystem.Logger.ReportError($"Id '{id}' not found");
            return false;
        }
        if(data.Group is not null)
        {
            groupName = data.Group;
            return true;
        }
        return TryGetGroupByType(data.GetType(), out groupName);
    }
    bool TryInvokeCallback(OsiEvent osiEvent)
    {
        if(!Callbacks.TryGetValue(osiEvent.TargetId, out var action)) return false;
        action(osiEvent.Payload);
        Callbacks.Remove(osiEvent.TargetId);
        return true;
    }
    IEnumerable<OsiGroup> TraverseGroups(string groupName)
    {
        while(groupName is not null)
        {
            if(!Groups.TryGetValue(groupName, out var group))
            {
                OsiSystem.Logger.ReportError($"No group of name '{groupName}' exists.");
                yield break;
            }
            yield return group;
        }
        OsiSystem.Logger.ReportError($"No compatible group found.");
    }
    bool TryCallMethod(string groupName, OsiData data, OsiEvent osiEvent)
    {
        foreach (var group in TraverseGroups(groupName))
        {
            if(group.TryCallMethod(data, osiEvent)) return true;
        }
        return false;
    }
    bool TryCallConstructor(Guid id, string groupName, out OsiData data)
    {
        data = default;
        foreach (var group in TraverseGroups(groupName))
        {
            if(group.Constructor is null) continue;
            data = group.Constructor(id, groupName);
            return true;
        }
        return false;
    }
    public bool TryCallWrapper(string groupName, Guid id, out DataClassWrapper wrapper)
    {
        wrapper = default;
        foreach (var group in TraverseGroups(groupName))
        {
            if(group.Wrapper is null) continue;
            wrapper = group.Wrapper(id);
            return true;
        }
        return false;
    }
}
