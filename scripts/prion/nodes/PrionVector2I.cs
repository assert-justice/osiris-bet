using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Prion.Node;
public class PrionVector2I(int x, int y) : PrionNode
{
    public int X = x;
    public int Y = y;

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"vector2i:{X},{Y}";
    }
    public static bool TryFromString(string value, out PrionVector2I node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        if (!value.StartsWith("vector2i:"))
        {
            error = $"vector2i signature not present at start of string '{value}'.";
            return false;
        }
        value = value[9..];
        string[] coords = value.Split(',');
        if(coords.Length != 2)
        {
            error = $"vector2i expects exactly two comma separated integers after the signature.";
            return false;
        }
        List<int> ints = [];
        foreach (var item in coords)
        {
            if(!int.TryParse(item, out int v))
            {
                error = $"could not parse '{coords[0]}' as an integer.";
                return false;
            }
            ints.Add(v);
        }
        node = new PrionVector2I(ints[0], ints[1]);
        return true;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionVector2I node, out string error)
    {
        node = default;
        error = default;
        if(jsonNode is null)
        {
            error = "Invalid json kind. Value cannot be null.";
            return false;
        }
        var kind = jsonNode.GetValueKind();
        if(kind != System.Text.Json.JsonValueKind.String)
        {
            error = $"Invalid json kind, expected string, received '{kind}'.";
            return false;
        }
        if(!jsonNode.AsValue().TryGetValue(out string sValue))
        {
            error = "Should be unreachable";
            return false;
        }
        return TryFromString(sValue, out node, out error);
    }
}
