using System.Collections.Generic;
using System.Linq;
using Prion.Node;

namespace Prion.Schema;

public class PrionSchemaNested(string[] schemaNames) : PrionSchemaNode
{
    public readonly string[] SchemaNames = schemaNames;
    public static bool TryFromString(string str, out PrionSchemaNested prionSchemaNested, out string error)
    {
        prionSchemaNested = default;
        error = default;
        str = str[7..]; // remove "schema:"
        var schemaNames = str.Split(',').Select(s => s.Trim());
        prionSchemaNested = new([..schemaNames]);
        return true;
    }
    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        List<string> errors = [];
        error = default;
        foreach (var schemaName in SchemaNames)
        {
            if(PrionSchemaManager.Validate(schemaName, prionNode, out _)) return true;
        }
        error = string.Join("\n", errors);
        return false;
    }
}
