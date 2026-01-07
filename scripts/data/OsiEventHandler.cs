using System;
using System.Collections.Generic;
using Osiris.System;

namespace Osiris.Data;

class Group
{
    public readonly string Name;
    public readonly string BaseGroup;
    public readonly Func<OsiEvent, OsiData> Constructor;
    // public bool IsFinished{get; private set;}
    public bool IsSealed{get; private set;}
    readonly Dictionary<string, Action<OsiData, OsiEvent>> Methods = [];
    public Group(string name, string baseGroup, Func<OsiEvent, OsiData> constructor)
    {
        Name = name;
        BaseGroup = baseGroup;
        Constructor = constructor;
    }
    public void Seal(){IsSealed = true;}
    public bool AddMethod<T>(string name, Action<T, OsiEvent> action) where T : OsiData
    {
        if (Methods.ContainsKey(name))
        {
            OsiSystem.Logger.ReportError($"A method of '{name}' already exists in group '{Name}'.");
            return false;
        }
        void closure(OsiData data, OsiEvent osiEvent)
        {
            if(data is T obj)action(obj, osiEvent);
            else OsiSystem.Logger.ReportError($"Mismatched type, expected a '{typeof(T)}', received a '{data.GetType()}'");
        }
        Methods.Add(name, closure);
        return true;
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
    readonly Dictionary<string, Group> Groups = [];
    public void AddGroup<T>(string groupName, string baseGroupName, Func<OsiEvent, OsiData> constructor, (string, Action<T, OsiEvent>)[] methods)
        where T : OsiData
    {
        if (Groups.ContainsKey(groupName))
        {
            OsiSystem.Logger.ReportError($"A group of name '{groupName}' already exists.");
            return;
        }
        if(baseGroupName is not null)
        {
            if(!Groups.TryGetValue(baseGroupName, out var baseGroup))
            {
                OsiSystem.Logger.ReportError($"No group of name '{baseGroupName}' exists.");
                return;
            }
            if (baseGroup.IsSealed)
            {
                OsiSystem.Logger.ReportError($"Base group of name '{baseGroupName}' is sealed and cannot be extended.");
                return;
            }
        }
        Group group = new(groupName, baseGroupName, constructor);
        foreach (var (name, method) in methods)
        {
            if(!group.AddMethod(name, method)) return;
        }
        Groups.Add(groupName, group);
    }
    public void SealGroup(string groupName)
    {
        if(!Groups.TryGetValue(groupName, out var group)) OsiSystem.Logger.ReportError($"No group of name '{groupName}' exists.");
        else group.Seal();
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

        if(!OsiSystem.Session.TryGetObject(osiEvent.TargetId, out OsiBlob blob))
        {
            if(Groups.TryGetValue(osiEvent.Verb, out var group))
            {
                OsiSystem.Session.AddObject(group.Constructor(osiEvent));
            }
            else
            {
                OsiSystem.Logger.ReportError($"Blob id '{osiEvent.TargetId}' not found and no group of name '{osiEvent.Verb}' exists.");
            }
            return;
        }
        string groupName = blob.Group;
        while(groupName is not null)
        {
            if(!Groups.TryGetValue(groupName, out var group))
            {
                OsiSystem.Logger.ReportError($"No group of name '{groupName}' exists, cannot call method '{osiEvent.Verb}'.");
                return;
            }
            if(group.TryCallMethod(blob, osiEvent)) return;
            groupName = group.BaseGroup;
        }
        OsiSystem.Logger.ReportError($"No method named '{osiEvent.Verb}' was found in group '{groupName}'.");
    }
}
