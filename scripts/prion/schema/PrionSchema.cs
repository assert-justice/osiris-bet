using Prion.Node;

namespace Prion.Schema;

public class PrionSchema : PrionSchemaNode
{
    public readonly string Name;
    public readonly string Version;
    readonly PrionSchemaNode Data;
    PrionSchema(string name, string version, PrionSchemaNode data)
    {
        Name = name;
        Version = version;
        Data = data;
    }
    public static bool TryFromPrionNode(PrionNode prionNode, out PrionSchema prionSchema, out string error)
    {
        prionSchema = default;
        if(!prionNode.TryAs(out PrionDict prionDict))
        {
            error = $"Expected top level node of schema to be a dict, found '{prionNode.GetType()}'.";
            return false;
        }
        if(!prionDict.TryGet("name", out string name))
        {
            error = "Top level of a schema must contain a name field of type string.";
            return false;
        }
        if(!prionDict.TryGet("version", out string version))
        {
            error = "Top level of a schema must contain a version field of type string.";
            return false;
        }
        if(!prionDict.TryGet("data", out PrionNode prionData))
        {
            error = "Top level of a schema must contain a data field.";
            return false;
        }
        if(!TryFromPrionNode(prionData, out PrionSchemaNode data, out error)) return false;
        prionSchema = new(name, version, data);
        return true;
    }
    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        return Data.TryValidate(prionNode, out error);
    }
}
