using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionI32(int value) : PrionNode
{
    public int Value = value;

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"i32:{Value}";
    }
    public static bool TryFromString(string value, out PrionI32 node, out string error)
    {
        node = default;
        error = default;
        if (!value.StartsWith("i32:"))
        {
            error = $"i32 signature not present at start of string {value}.";
            return false;
        }
        if(int.TryParse(value[4..], out int i))
        {
            node = new PrionI32(i);
            return true;
        }
        error = $"could not parse string {value} as i32.";
        return false;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionI32 node, out string error)
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
