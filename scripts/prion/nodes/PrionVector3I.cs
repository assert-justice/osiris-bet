using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionVector3I(int x, int y, int z) : PrionNode
{
    public int X = x;
    public int Y = y;
    public int Z = z;
    public static bool TryFromString(string value, out PrionVector3I node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        if (!value.StartsWith("vector3i:"))
        {
            error = $"vector3i signature not present at start of string '{value}'.";
            return false;
        }
        value = value[9..];
        string[] coords = value.Split(',');
        if(coords.Length != 3)
        {
            error = $"vector3i expects exactly three comma separated integers after the signature.";
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
        node = new PrionVector3I(ints[0], ints[1], ints[2]);
        return true;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionVector3I node, out string error)
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
    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"vector3i:{X},{Y}";
    }
}