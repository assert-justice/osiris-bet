using System.Collections.Generic;
using Prion.Node;

namespace Prion.Schema;

public class PrionSchemaString : PrionSchemaNode
{
    public readonly string Value;
    static HashSet<string> ValidValues;
    PrionSchemaString(string value)
    {
        Value = value;
    }
    public static bool TryFromString(string value, out PrionSchemaString prionSchemaString, out string error)
    {
        error = default;
        prionSchemaString = default;
        ValidValues ??= [
            "boolean",
            "color",
            "dynamic",
            "f32",
            "guid",
            "i32",
            "rect2i",
            "string",
            "u64",
            "ubigint",
            "vector2i",
            "vector3i",
        ];
        if (!ValidValues.Contains(value))
        {
            error = $"Invalid schema type '{value}'.";
            return false;
        }
        prionSchemaString = new(value);
        return true;
    }
    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        error = default;
        if(prionNode is PrionString) return true;
        string str = prionNode.GetType().ToString().ToLower();
        if(!PrionParseUtils.MatchStart("prion.node.prion", ref str))
        {
            error = "should be unreachable";
            return false;
        }
        if (!ValidValues.Contains(str))
        {
            error = $"Expected a schema type, found a '{prionNode.GetType().ToString().ToLower()}'.";
            return false;
        }
        return true;
    }
}
