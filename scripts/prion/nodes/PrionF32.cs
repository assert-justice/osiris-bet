using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionF32(float value) : PrionNode
{
    public float Value = value;
    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{Value}\"");
    }

    public override string ToString()
    {
        return $"f32: {Value}";
    }
    public static bool TryFromString(string value, out PrionF32 prionF32, out string error)
    {
        prionF32 = default;
        error = default;
        if(!PrionParseUtils.MatchStart("f32:", ref value))
        {
            error = $"F32 signature not present at start of string '{value}'.";
            return false;
        }
        value = value.Trim();
        if(!float.TryParse(value, out float v))
        {
            error = "Failed to parse string '{str}' as an f32.";
            return false;
        }
        prionF32 = new(v);
        return true;
    }
}
