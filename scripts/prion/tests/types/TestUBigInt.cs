
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestUBigInt
    {
        readonly Random Rng = new();
        [TestMethod]
        public void Create()
        {
            string[] hexValues = [
                "0x00",
                "0xff",
            ];
            foreach (var item in hexValues)
            {
                DoesMatchHex(item, item);
            }
            int trials = 100;
            for (int idx = 0; idx < trials; idx++)
            {
                string str = GetRandomHexString();
                string strLower = str.ToLower();
                DoesMatchHex(str, strLower);
            }
            for (int idx = 0; idx < trials; idx++)
            {
                string str = GetRandomBinaryString();
                string strLower = str.ToLower();
                DoesMatchBinary(str, strLower);
            }
        }
        static void DoesMatchHex(string input, string expected)
        {
            if(!PrionUBigInt.TryFromHexString(input, out PrionUBigInt node, out string error))
            {
                Assert.Fail(error);
            }
            Assert.IsTrue(node.ToHexString() == expected);
        }
        static void DoesMatchBinary(string input, string expected)
        {
            if(!PrionUBigInt.TryFromBinaryString(input, out PrionUBigInt node, out string error))
            {
                Assert.Fail(error);
            }
            Assert.IsTrue(node.ToBinaryString() == expected);
        }
        string GetRandomHexString()
        {
            const string hexChars = "0123456789abcdefABCDEF";
            int length = Rng.Next(1, 255);
            var sb = new StringBuilder(length + 2);
            sb.Append("0x");
            for (int idx = 0; idx < length; idx++)
            {
                sb.Append(hexChars[Rng.Next(hexChars.Length)]);
            }
            return sb.ToString();
        }
        string GetRandomBinaryString()
        {
            int length = Rng.Next(1, 1023);
            var sb = new StringBuilder(length + 2);
            sb.Append("0b");
            for (int idx = 0; idx < length; idx++)
            {
                sb.Append(Rng.Next(2) == 1 ? '1' : '0');
            }
            return sb.ToString();
        }
    }
}
