using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestVector2I
    {
        readonly Random Rng = new();
        [TestMethod]
        public void Create()
        {
            int trials = 100;
            for (int idx = 0; idx < trials; idx++)
            {
                int x = GetRandomCoord();
                int y = GetRandomCoord();
                string str = $"vector2i:{x},{y}";
                PrionVector2I vec = new(x, y);
                Assert.IsTrue(str == vec.ToString());
            }
        }
        int GetRandomCoord()
        {
            int val = Rng.Next();
            return (Rng.Next(2) == 0) ? val : -val;
        }
    }
}
