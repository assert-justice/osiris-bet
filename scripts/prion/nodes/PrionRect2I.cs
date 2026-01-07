using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Prion.Node;
public class PrionRect2I(PrionVector2I position, PrionVector2I size) : PrionNode
{
    public PrionVector2I Position = position;
    public PrionVector2I Size = size;

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"rect2i:{Position.X},{Position.Y},{Size.X},{Size.Y}";
    }
    public static bool TryFromString(string value, out PrionRect2I node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        if (!value.StartsWith("rect2i:"))
        {
            error = $"rect2i signature not present at start of string '{value}'.";
            return false;
        }
        value = value[7..];
        string[] coords = value.Split(',');
        if(coords.Length != 4)
        {
            error = $"rect2i expects exactly four comma separated integers after the signature.";
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
        node = new PrionRect2I(new(ints[0],ints[1]), new(ints[2],ints[3]));
        return true;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionRect2I node, out string error)
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
