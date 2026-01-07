using System;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestParseJsonColor
    {
        readonly Random Rng = new();
        [TestMethod]
        public void ParseJsonColor()
        {
            string[] values = [
                "00000000",
                "ffff00ff",
            ];
            foreach (var item in values)
            {
                DoesMatch("color:#"+item, "color:"+Expand("#" + item));
            }
            foreach (var item in values)
            {
                DoesMatch("color:0x"+item, "color:"+Expand("#" + item));
            }
        }
        static void DoesMatch(string input, string expected)
        {
            string jsonStr = $"\"{input}\"";
            expected = $"\"{expected}\"";
            var jsonNode = JsonNode.Parse(jsonStr);
            if(!PrionNode.TryFromJson(jsonNode, out PrionColor node, out string error))
            {
                Assert.Fail($"Parse failed, input: {input}, expected: {expected}, error: {error}");
            }
            if(node.ToJson().ToJsonString() != expected)
            {
                Assert.Fail($"Mismatch detected input: {input}, expected: {expected}, received: {node.ToJson().ToJsonString()} error: {node}");
            }
            if(!PrionColor.TryFromJson(jsonNode, out node, out error))
            {
                Assert.Fail($"Parse failed, input: {input}, expected: {expected}, error: {error}");
            }
            if(node.ToJson().ToJsonString() != expected)
            {
                Assert.Fail($"Mismatch detected input: {input}, expected: {expected}, error: {node}");
            }
        }
        string GetRandomColor()
        {
            const string hexChars = "0123456789abcdefABCDEF";
            int[] lens = [3, 4, 6, 8];
            int length = lens[Rng.Next(4)];
            var sb = new StringBuilder(length + 2);
            for (int idx = 0; idx < length; idx++)
            {
                sb.Append(hexChars[Rng.Next(hexChars.Length)]);
            }
            return sb.ToString();
        }
        static string Expand(string s)
        {
            s = s.ToLower();
            string prefix;
            string value;
            if (s.StartsWith("0x"))
            {
                prefix = "0x";
                value = s[2..];
            }
            else
            {
                prefix = "#";
                value = s[1..];
            }
            string expanded = "";
            switch (value.Length)
            {
                case 3:
                for (int idx = 0; idx < value.Length; idx++)
                {
                    expanded += value[idx];
                    expanded += value[idx];
                }
                expanded += "ff";
                break;
                case 4:
                for (int idx = 0; idx < value.Length; idx++)
                {
                    expanded += value[idx];
                    expanded += value[idx];
                }
                break;
                case 6:
                expanded = value + "ff";
                break;
                case 8:
                expanded = value;
                break;
                default:
                break;
            }
            return prefix + expanded;
        }
    }
}
