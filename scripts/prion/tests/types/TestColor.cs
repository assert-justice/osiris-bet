
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestColor
    {
        readonly Random Rng = new();
        [TestMethod]
        public void Create()
        {
            string[] hexValues = [
                "0x00000000",
                "0xffff00ff",
            ];
            string[] htmlValues = [
                "#00000000",
                "#ffff00ff",
            ];
            foreach (var item in htmlValues)
            {
                DoesMatch("color:" + item, "color:" + Expand(item));
                DoesMatchHtml(item, item);
            }
            foreach (var item in hexValues)
            {
                DoesMatchHex(item, item);
            }
            int trials = 100;
            for (int idx = 0; idx < trials; idx++)
            {
                string str = GetRandomColor();
                DoesMatch("color:0x" + str, "color:" + Expand("#" + str));
                DoesMatchHex("0x" + str, Expand("0x" + str));
            }
            for (int idx = 0; idx < trials; idx++)
            {
                string str = "#" + GetRandomColor();
                DoesMatch("color:" + str, "color:" + Expand(str));
                DoesMatchHtml(str, Expand(str));
            }
        }
        static void DoesMatchHex(string input, string expected)
        {
            if(!PrionColor.TryFromHexString(input, out PrionColor node, out string error))
            {
                Assert.Fail($"Failed to parse, input: {input}, expected: {expected}, error: {error}");
            }
            if(node.ToHexString() != expected)
            {
                Assert.Fail($"Mismatch detected input: {input}, expected: {expected}, received: {(node as PrionColor).ToHexString()} error: {node}");
            }
        }
        static void DoesMatchHtml(string input, string expected)
        {
            if(!PrionColor.TryFromHtmlString(input, out PrionColor node, out string error))
            {
                Assert.Fail($"Failed to parse, input: {input}, expected: {expected}, error: {error}");
            }
            if(node.ToHtmlString() != expected)
            {
                Assert.Fail($"Mismatch detected, input: {input}, expected: {expected}, error: {node}");
            }
        }
        static void DoesMatch(string input, string expected)
        {
            if(!PrionColor.TryFromString(input, out PrionColor node, out string error))
            {
                Assert.Fail($"Failed to parse, input: {input}, expected: {expected}, error: {error}");
            }
            if(node.ToString() != expected)
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
