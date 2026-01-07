using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionBoolean(bool value) : PrionNode
{
    public bool Value = value;
    public override JsonNode ToJson()
    {
        return JsonNode.Parse(ToString());
    }

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}
