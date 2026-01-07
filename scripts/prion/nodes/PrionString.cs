using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionString : PrionNode
{
    public string Value = "";
    public PrionString(){}
    public PrionString(string value){Value = value;}

    public static PrionString FromJson(JsonNode jsonNode)
    {
        if(jsonNode.GetValueKind() == System.Text.Json.JsonValueKind.String)
        {
            if(jsonNode.AsValue().TryGetValue(out string value)){
                return new(value);
            }
        }
        return new(jsonNode.ToJsonString());
    }
    public static PrionString FromString(string str)
    {
        return new(str);
    }

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{Value}\"");
    }

    public override string ToString()
    {
        return Value;
    }
}