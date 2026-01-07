using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace Prion.Node;
public class PrionUBigInt : PrionNode
{
    public readonly List<bool> Data = [];
    const string HexChars = "0123456789abcdef";
    public PrionUBigInt(){}
    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{this}\"");
    }
    public override string ToString()
    {
        return $"ubigint: {ToHexString()}";
    }
    public static bool TryFromString(string value, out PrionUBigInt node, out string error)
    {
        node = default;
        error = default;
        if (!value.StartsWith("ubigint:"))
        {
            error = $"ubigint signature not present at start of string {value}.";
            return false;
        }
        value = value[8..].Trim();
        if(value.StartsWith("0x") && TryFromHexString(value, out node, out error)) return true;
        if(value.StartsWith("0b") && TryFromBinaryString(value, out node, out error)) return true;
        error = $"Invalid ubigint value {value}";
        return false;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionUBigInt node, out string error)
    {
        node = default;
        if(jsonNode is null)
        {
            error = "Invalid json kind. Value cannot be null.";
            return false;
        }
        var kind = jsonNode.GetValueKind();
        if(kind != System.Text.Json.JsonValueKind.String)
        {
            error = "Invalid json kind";
            return false;
        }
        if(!jsonNode.AsValue().TryGetValue(out string sValue))
        {
            error = "Should be unreachable";
            return false;
        }
        return TryFromString(sValue, out node, out error);
    }
    public static PrionUBigInt FromULong(ulong value)
    {
        int length = ULongGetLength(value);
        var res = new PrionUBigInt();
        res.SetRange(0, length, value);
        return res;
    }
    public static bool TryFromHexString(string value, out PrionUBigInt node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        if (!value.StartsWith("0x"))
        {
            error = $"Hex strings must begin with 0x, found '{value}'";
            return false;
        }
        node = new PrionUBigInt();
        value = value[2..].ToLower();
        node.EnsureCapacity(value.Length * 4);
        for (int idx = 0; idx < value.Length; idx++)
        {
            char c = value[value.Length - 1 - idx];
            int charValue = HexChars.ToList().FindIndex(ch => ch == c);
            if(charValue == -1)
            {
                error = $"Found invalid char {c} in hex string";
                return false;
            }
            byte v = (byte)charValue;
            node.SetRange(idx * 4, 4, v);
        }
        return true;
    }
    public static bool TryFromBinaryString(string value, out PrionUBigInt node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        if (!value.StartsWith("0b"))
        {
            error = $"Binary strings must begin with 0b, found '{value}'";
            return false;
        }
        var res = new PrionUBigInt();
        value = value[2..];
        for (int idx = 0; idx < value.Length; idx++)
        {
            char c = value[value.Length - 1 - idx];
            if(c == '0') res.PushBit(false);
            else if(c == '1') res.PushBit(true);
            else
            {
                error = $"Found invalid char {c} in binary string";
                return false;
            }
        }
        node = res;
        return true;
    }
    public bool GetBit(int idx)
    {
        // TODO: hook up error reporting.
        if(idx < 0) return false;
        EnsureCapacity(idx+1);
        return Data[idx];
    }
    public void SetBit(int idx, bool value)
    {
        // TODO: hook up error reporting.
        if(idx < 0) return;
        EnsureCapacity(idx+1);
        Data[idx] = value;
    }
    public void PushBit(bool value)
    {
        Data.Add(value);
    }
    public void PushNibble(byte value)
    {
        EnsureCapacity(Data.Count + 4);
        SetRange(Data.Count, 4, value);
    }
    public void PushByte(byte value)
    {
        int start = Data.Count;
        EnsureCapacity(start + 8);
        SetRange(start, 8, value);
    }
    public ulong GetRange(int offset, int length)
    {
        if(offset < 0) return 0;
        if(length < 0) return 0;
        EnsureCapacity(offset + length);
        ulong res = 0;
        for (int i = 0; i < length; i++)
        {
            int idx = i + offset;
            ulong mask = (ulong)1 << i;
            if(Data[idx]) res |= mask;
        }
        return res;
    }
    public void SetRange(int offset, int length, ulong value)
    {
        if(offset < 0) return;
        if(length < 0) return;
        ulong maxSize = ((ulong)1 << length) - 1;
        if(value > maxSize) return;
        EnsureCapacity(offset + length);
        for (int i = 0; i < length; i++)
        {
            int idx = i + offset;
            Data[idx] = (value & 1) == 1;
            value >>= 1;
        }
    }
    public string ToHexString()
    {
        int rem = Data.Count % 4;
        if(rem > 0) EnsureCapacity(Data.Count + 4 - rem);
        int numChars = Data.Count / 4;
        var sb = new StringBuilder(numChars + 2);
        sb.Append("0x");
        for (int i = 0; i < numChars; i++)
        {
            int idx = numChars - 1 - i;
            ulong n = GetRange(idx * 4, 4);
            char c = HexChars[(int)n];
            sb.Append(c);
        }
        return sb.ToString();
    }
    public string ToBinaryString()
    {
        var sb = new StringBuilder(Data.Count + 2);
        sb.Append("0b");
        for (int i = 0; i < Data.Count; i++)
        {
            int idx = Data.Count - 1 - i;
            bool val = Data[idx];
            sb.Append(val ? '1' : '0');
        }
        return sb.ToString();
    }
    void EnsureCapacity(int capacity)
    {
        if(capacity < 0) return;
        if(Data.Capacity < capacity)
        {
            Data.Capacity = capacity;
            while(Data.Count < capacity) Data.Add(false);
        }
    }
    static int ULongGetLength(ulong value)
    {
        int res = 0;
        while(value != 0)
        {
            res += 1;
            value >>= 1;
        }
        return res;
    }
}
