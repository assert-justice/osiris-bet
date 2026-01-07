using System.Collections.Generic;
using Prion.Node;

namespace Prion.Schema;

public class PrionSchemaDict : PrionSchemaNode
{
    public readonly Dictionary<string, PrionSchemaNode> ChildSchemas;
    PrionSchemaDict(Dictionary<string, PrionSchemaNode> childSchemas)
    {
        ChildSchemas = childSchemas;
    }
    public static bool TryFromPrionDict(PrionDict prionDict, out PrionSchemaDict prionSchemaDict, out string error)
    {
        prionSchemaDict = default;
        error = default;
        Dictionary<string, PrionSchemaNode> childSchemas = [];
        foreach (var (key, node) in prionDict.Value)
        {
            if(key.StartsWith('#')) continue;
            if(!TryFromPrionNode(node, out PrionSchemaNode prionSchemaNode, out error)) return false;
            childSchemas[key] = prionSchemaNode;
        }
        prionSchemaDict = new(childSchemas);
        return true;
    }

    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        error = "";
        if(!prionNode.TryAs(out PrionDict prionDict))
        {
            error = $"Expected an array, found a '{prionNode.GetType()}'.";
            return false;
        }
        HashSet<string> keys = [.. prionDict.Value.Keys];
        foreach (var (key, value) in ChildSchemas)
        {
            keys.Remove(key);
            bool nullable = key.EndsWith('?');
            if(!prionDict.Value.TryGetValue(key, out PrionNode node))
            {
                if(nullable) continue;
                error = $"Dict is missing key from schema '{key}'.";
                return false;
            }
            if(nullable && node is PrionNull) continue;
            if(value is PrionSchemaString str && str.Value == "dynamic") continue;
            if(!value.TryValidate(node, out error)) return false;
        }
        if(keys.Count > 0)
        {
            error = $"Unexpected extra keys in dict: {string.Join(", ", keys)}";
            return false;
        }
        return true;
    }
}
