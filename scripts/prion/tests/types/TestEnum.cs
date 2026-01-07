using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestEnum
    {
        readonly Random Rng = new();
        readonly StringBuilder sb = new();
        const string validEnumChars = PrionParseUtils.AlphaLower + PrionParseUtils.AlphaUpper + PrionParseUtils.Numeric + "_";
        [TestMethod]
        public void Create()
        {
            int trials = 100;
            for (int idx = 0; idx < trials; idx++)
            {
                HashSet<string> options = GetRandomOptions();
                int index = Rng.Next(options.Count);
                string[] optionStrings = [.. options];
                string res = string.Join(", ", options);
                res = "enum: " + res + ": " + optionStrings[index];
                if(!PrionEnum.TryFromString(res, out PrionEnum _, out string error))
                {
                    Assert.Fail($"Enum parse failed with error: {error}.");
                }
            }
        }
        string GetRandomIdent()
        {
            int length = Rng.Next(1,256);
            sb.Clear();
            sb.EnsureCapacity(length);
            for (int f = 0; f < length; f++)
            {
                char c = validEnumChars[Rng.Next(validEnumChars.Length)];
                sb.Append(c);
            }
            return sb.ToString();
        }
        HashSet<string> GetRandomOptions()
        {
            int length = Rng.Next(1,256);
            HashSet<string> options = new(length);
            while(options.Count < length) options.Add(GetRandomIdent());
            return options;
        }
    }
}
