using System.Text;
using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionColor : PrionNode
{
    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Alpha;
    public PrionColor(){}
    public PrionColor(byte red, byte green, byte blue, byte alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }
    public PrionColor(ulong value)
    {
        var bf = PrionUBigInt.FromULong(value);
        _ = new PrionColor(bf);
    }
    public PrionColor(PrionUBigInt bf)
    {
        Alpha = (byte)bf.GetRange(0, 8);
        Blue = (byte)bf.GetRange(8, 8);
        Green = (byte)bf.GetRange(16, 8);
        Red = (byte)bf.GetRange(24, 8);
    }
    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        return $"color:{ToHtmlString()}";
    }
    public static bool TryFromString(string value, out PrionColor node, out string error)
    {
        node = default;
        error = default;
        if(!PrionParseUtils.MatchStart("color:", ref value))
        {
            error = $"Color signature not present at start of string '{value}'.";
            return false;
        }
        value = value.Trim();
        if(value.StartsWith("0x") && TryFromHexString(value, out node, out error)) return true;
        if(value.StartsWith("#") && TryFromHtmlString(value, out node, out error)) return true;
        error = $"Invalid color value {value}";
        return false;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionColor node, out string error)
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
            error = $"Invalid json kind. Expected string but received '{kind}'.";
            return false;
        }
        if(!jsonNode.AsValue().TryGetValue(out string sValue))
        {
            error = "Should be unreachable";
            return false;
        }
        return TryFromString(sValue, out node, out error);
    }
    public string ToHexString()
    {
        var val = new PrionUBigInt();
        val.PushByte(Alpha);
        val.PushByte(Blue);
        val.PushByte(Green);
        val.PushByte(Red);
        return val.ToHexString();
    }
    public string ToHtmlString()
    {
        return "#" + ToHexString()[2..];
    }
    public static bool TryFromHtmlString(string htmlString, out PrionColor node, out string error)
    {
        error = default;
        node = default;
        htmlString = htmlString.Trim();
        if (!htmlString.StartsWith('#'))
        {
            error = $"Html strings must begin with #, found '{htmlString}'";
            return false;
        }
        htmlString = htmlString[1..];
        if(htmlString.Length != 3 && htmlString.Length != 4 && htmlString.Length != 6 && htmlString.Length != 8)
        {
            error = $"Invalid length {htmlString.Length} for html color string, found '{htmlString}'";
            return false;
        }
        return TryFromHexString("0x" + htmlString, out node, out error);
    }
    public static bool TryFromHexString(string hexString, out PrionColor node, out string error)
    {
        node = default;
        error = default;
        if (!hexString.StartsWith("0x"))
        {
            error = $"Hex strings must begin with 0x, found '{hexString}'.";
            return false;
        }
        hexString = hexString[2..].Trim();
        var sb = new StringBuilder(10);
        sb.Append("0x");
        switch (hexString.Length)
        {
            case 3:
                sb.Append(hexString[0]);
                sb.Append(hexString[0]);
                sb.Append(hexString[1]);
                sb.Append(hexString[1]);
                sb.Append(hexString[2]);
                sb.Append(hexString[2]);
                sb.Append("ff");
                break;
            case 4:
                sb.Append(hexString[0]);
                sb.Append(hexString[0]);
                sb.Append(hexString[1]);
                sb.Append(hexString[1]);
                sb.Append(hexString[2]);
                sb.Append(hexString[2]);
                sb.Append(hexString[3]);
                sb.Append(hexString[3]);
                break;
            case 6:
                sb.Append(hexString);
                sb.Append("ff");
                break;
            case 8:
                sb.Append(hexString);
                break;
            default:
                error = $"Invalid length {hexString.Length + 2} for hex color string, found '{hexString}'";
                return false;
        }
        if(!PrionUBigInt.TryFromHexString(sb.ToString(), out PrionUBigInt bf, out error))
        {
            error = $"Oops";
            return false;
        }
        node = new PrionColor(bf as PrionUBigInt);
        return true;
    }
}
