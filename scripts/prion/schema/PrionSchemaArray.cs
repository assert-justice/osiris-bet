using Prion.Node;

namespace Prion.Schema;

public class PrionSchemaArray : PrionSchemaNode
{
    public readonly PrionSchemaNode ChildSchema;
    PrionSchemaArray(PrionSchemaNode childSchema)
    {
        ChildSchema = childSchema;
    }
    public static bool TryFromPrionArray(PrionArray prionArray, out PrionSchemaArray prionSchemaArray, out string error)
    {
        prionSchemaArray = default;
        if(prionArray.Value.Count != 1)
        {
            error = "Arrays in schemas can only have one element, the schema of all entries.";
            return false;
        }
        if(!TryFromPrionNode(prionArray.Value[0], out PrionSchemaNode prionSchemaNode, out error)) return false;
        prionSchemaArray = new PrionSchemaArray(prionSchemaNode);
        return true;
    }

    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        error = default;
        if(!prionNode.TryAs(out PrionArray prionArray))
        {
            error = $"Expected an array, found a '{prionNode.GetType()}'.";
            return false;
        }
        foreach (var child in prionArray.Value)
        {
            if(!ChildSchema.TryValidate(child, out error)) return false;
        }
        return true;
    }
}
