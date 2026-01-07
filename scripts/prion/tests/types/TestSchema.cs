using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prion.Node;
using Prion.Schema;

namespace Prion.Tests
{
    [TestClass]
    public class TestSchema
    {
        [TestMethod]
        public void Create()
        {
            string exampleSchema = TestUtils.ReadFile("scripts/schemas/actor_schema.json");
            var jsonNode = JsonNode.Parse(exampleSchema);
            if(jsonNode.GetValueKind() != System.Text.Json.JsonValueKind.Object)
            {
                Assert.Fail($"Expected json object, found '{jsonNode.GetValueKind()}'.");
            }
            if(!PrionNode.TryFromJson(jsonNode, out PrionNode prionNode, out string error))
            {
                Assert.Fail(error);
            }
            if(!PrionSchema.TryFromPrionNode(prionNode, out PrionSchema _, out error))
            {
                Assert.Fail(error);
            }
        }
    }
}
