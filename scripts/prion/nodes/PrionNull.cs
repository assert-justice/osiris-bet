using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionNull : PrionNode
{
    public override JsonNode ToJson()
    {
        return null;
    }

    public override string ToString()
    {
        return "null";
    }
}