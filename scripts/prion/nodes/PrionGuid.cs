using System;
using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionGuid(Guid value) : PrionNode
{
    public readonly Guid Value = value;
    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{this}\"");
    }
    public override string ToString()
    {
        return $"guid: {Value}";
    }
    public static bool TryFromString(string value, out PrionGuid prionGuid, out string error)
    {
        prionGuid = default;
        error = default;
        if(!PrionParseUtils.MatchStart("guid:", ref value))
        {
            error = $"Guid signature not present at start of string '{value}'.";
            return false;
        }
        value = value.Trim();
        if(!Guid.TryParse(value, out Guid guid))
        {
            error = "Failed to parse string '{str}' as an Guid.";
            return false;
        }
        prionGuid = new(guid);
        return true;
    }
}
