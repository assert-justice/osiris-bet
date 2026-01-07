using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionU64(ulong value) : PrionNode
{
    public ulong Value = value;

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"u64:{Value}";
    }
    public static bool TryFromString(string value, out PrionU64 node, out string error)
    {
        node = default;
        error = default;
        if (!value.StartsWith("u64:"))
        {
            error = $"u64 signature not present at start of string {value}.";
            return false;
        }
        if(ulong.TryParse(value[4..], out ulong i))
        {
            node = new PrionU64(i);
            return true;
        }
        error = $"could not parse string {value} as u64.";
        return false;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionU64 node, out string error)
    {
        node = default;
        error = default;
        if(jsonNode is null)
        {
            error = "Invalid json kind. Value cannot be null.";
            return false;
        }
        var kind = jsonNode.GetValueKind();
        if(kind == System.Text.Json.JsonValueKind.String)
        {
            if(jsonNode.AsValue().TryGetValue(out string sValue))
            {
                return TryFromString(sValue, out node, out error);
            }
            error = "Should be unreachable";
            return false;
        }
        else
        {
            error = "Invalid json kind";
            return false;
        }
    }
}
