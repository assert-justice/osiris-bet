using System;
using Prion.Node;

namespace Osiris.Data;

public class OsiData(Guid id, string group = null) : IOsiTryFromNode<OsiData>
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
    protected static bool TryFromNodeFactory<T>(PrionNode prionNode, Func<Guid, string, T> factory, out T data) where T : OsiData
    {
        string group = "[no group]";
        BaseTryFromNode(prionNode, out PrionDict dict, out Guid id);
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

    public static bool TryFromNode(PrionNode prionNode, out OsiData data)
    {
        return TryFromNodeFactory(prionNode, (id, group)=>new(id, group), out data);
    }
}
