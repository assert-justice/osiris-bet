using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;

namespace Prion.Tests
{
    [TestClass]
    public class TestParseJsonNumber
    {
        [TestMethod]
        public void ParseJsonNumber()
        {
            var jsonNode = JsonNode.Parse("10");
            if(!PrionNode.TryFromJson(jsonNode, out PrionF32 prionNode, out string error))
            {
                Assert.Fail(error);
            }
        }
        [TestMethod]
        public void ParseJsonF32()
        {
            string str = "f32: 10";
            string jsonStr = $"\"{str}\"";
            var jsonNode = JsonNode.Parse(jsonStr);
            Assert.IsTrue(PrionNode.TryFromJson(jsonNode, out PrionF32 prionNode, out string error));
            Assert.IsTrue(prionNode.ToString() == str);
            Assert.IsTrue(prionNode.ToJson().ToString() == str);
            jsonNode = JsonNode.Parse("\"10\"");
            if(!PrionNode.TryFromJson(jsonNode, out _, out error))
            {
                Assert.Fail(error);
            }

        }
        [TestMethod]
        public void ParseJsonI32()
        {
            string str = "i32:10";
            string jsonStr = $"\"{str}\"";
            var jsonNode = JsonNode.Parse(jsonStr);
            Assert.IsTrue(PrionNode.TryFromJson(jsonNode, out PrionI32 prionNode, out string error));
            Assert.IsTrue(PrionI32.TryFromJson(jsonNode, out prionNode, out error));
            Assert.IsTrue(prionNode.ToString() == str);
            Assert.IsTrue(prionNode.ToJson().ToString() == str);
            // jsonNode = JsonNode.Parse("\"10\"");
            // if(!PrionI32.TryFromJson(jsonNode, out _, out error))
            // {
            //     Assert.Fail(error);
            // }
        }
        [TestMethod]
        public void ParseJsonUBigInt()
        {
            string str = "ubigint: 0x10";
            string jsonStr = $"\"{str}\"";
            var jsonNode = JsonNode.Parse(jsonStr);
            if(!PrionNode.TryFromJson(jsonNode, out PrionUBigInt prionNode, out string error)) Assert.Fail(error);
            if(!PrionUBigInt.TryFromJson(jsonNode, out prionNode, out error)) Assert.Fail(error);
            Assert.IsTrue(prionNode.ToString() == str);
            Assert.IsTrue(prionNode.ToJson().ToString() == str);
            jsonNode = JsonNode.Parse("\"10\"");
            Assert.IsFalse(PrionUBigInt.TryFromJson(jsonNode, out prionNode, out error));
        }
    }
}
