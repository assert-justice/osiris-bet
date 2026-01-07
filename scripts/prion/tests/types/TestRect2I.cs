using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestRect2I
    {
        readonly Random Rng = new();
        [TestMethod]
        public void Create()
        {
            int trials = 100;
            for (int idx = 0; idx < trials; idx++)
            {
                int pX = GetRandomCoord();
                int pY = GetRandomCoord();
                int sX = GetRandomCoord();
                int sY = GetRandomCoord();
                string str = $"rect2i:{pX},{pY},{sX},{sY}";
                PrionRect2I rect = new(new(pX, pY), new(sX, sY));
                Assert.IsTrue(str == rect.ToString());
            }
        }
        int GetRandomCoord()
        {
            int val = Rng.Next();
            return (Rng.Next(2) == 0) ? val : -val;
        }
    }
}
