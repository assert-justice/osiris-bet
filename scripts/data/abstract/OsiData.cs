using System;
using Prion.Node;

namespace Osiris.Data;

public abstract class OsiData(Guid id, string group = "")
{
    public readonly Guid Id = id;
    public string Group = group;
    public virtual PrionDict ToNode()
    {
        PrionDict dict = new();
        dict.Set("id", Id);
        dict.Set("group?", Group);
        return dict;
    }
    public static bool BaseTryFromNode(PrionNode prionNode, out PrionDict prionDict, out Guid id)
    {
        id = default;
        if(!prionNode.TryAs(out prionDict)) return false;
        if(!prionDict.TryGet("id", out id)) return false;
        return true;
    }
    protected static bool TryFromNodeFactory<T>(PrionNode prionNode, Func<Guid, string, T> factory, out T data, out PrionDict dict) where T : OsiData
    {
        data = default;
        string group = "[no group]";
        BaseTryFromNode(prionNode, out dict, out Guid id);
        if(dict.TryGet("group?", out string groupName)) group = groupName;
        data = factory(id, group);
        if(!data.TryAppend(dict)) return false;
        return true;
    }
    protected virtual bool TryAppend(PrionDict dict)
    {
        dict.Set("group?", Group);
        return true;
    }

    public bool TryAs<T>(out T data) where T : OsiData
    {
        data = default;
        if(this is T res)
        {
            data = res;
            return true;
        }
        else return false;
    }
}