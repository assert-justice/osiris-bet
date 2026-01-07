using System.Collections.Generic;
using System.Linq;
using Prion.Node;

namespace Prion.Schema;

public class PrionSchemaEnum : PrionSchemaNode
{
    readonly string[] Options;
    PrionSchemaEnum(string[] options)
    {
        Options = options;
    }
    public static bool TryFromString(string str, out PrionSchemaEnum prionSchemaEnum, out string error)
    {
        prionSchemaEnum = default;
        error = default;
        str = str[12..]; // remove "schema_enum:"
        var options = str.Split(',').Select(s => s.Trim()).ToArray();
        if(new HashSet<string>([..options]).Count != options.Length)
        {
            error = "Schema enum contains duplicates";
            return false;
        }
        prionSchemaEnum = new(options);
        return true;
    }
    public override bool TryValidate(PrionNode prionNode, out string error)
    {
        error = default;
        if(!prionNode.TryAs(out PrionEnum prionEnum))
        {
            error = $"Expected an enum, found a '{prionNode.GetType()}'.";
            return false;
        }
        // var options = prionEnum.Options.ToList();
        if(Options.Length != prionEnum.Options.Length)
        {
            error = "Enum options do not match schema.";
            return false;
        }
        for (int idx = 0; idx < Options.Length; idx++)
        {
            if(Options[idx] != prionEnum.Options[idx])
            {
                error = "Enum options do not match schema.";
                return false;
            }
        }
        return true;
    }
}