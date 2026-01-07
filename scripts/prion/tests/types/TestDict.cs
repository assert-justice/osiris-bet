using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestDict
    {
        [TestMethod]
        public void Create()
        {
            var jsonNode = JsonNode.Parse("{\"bite me\": \"poo\"}");
            if(!PrionDict.TryFromJson(jsonNode, out PrionDict _, out string error))
            {
                Assert.Fail(error);
            }
            if(!PrionNode.TryFromJson(jsonNode, out PrionDict _, out error))
            {
                Assert.Fail(error);
            }
        }
    }
}
