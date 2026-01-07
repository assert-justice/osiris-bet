using System;
using Prion.Node;

namespace Prion.Schema;

public abstract class PrionSchemaNode
{
    public static bool TryFromPrionNode(PrionNode prionNode, out PrionSchemaNode prionSchemaNode,  out string error)
    {
        prionSchemaNode = default;
        if(prionNode is PrionArray prionArray)
        {
            if(!PrionSchemaArray.TryFromPrionArray(prionArray, out PrionSchemaArray prionSchemaArray, out error)) return false;
            prionSchemaNode = prionSchemaArray;
            return true;
        }
        else if(prionNode is PrionDict prionDict)
        {
            if(!PrionSchemaDict.TryFromPrionDict(prionDict, out PrionSchemaDict prionSchemaDict, out error)) return false;
            prionSchemaNode = prionSchemaDict;
            return true;
        }
        else if(prionNode is PrionString prionString)
        {
            string str = prionString.Value;
            if (str.StartsWith("schema_enum:"))
            {
                if(!PrionSchemaEnum.TryFromString(str, out PrionSchemaEnum prionSchemaEnum, out error)) return false;
                prionSchemaNode = prionSchemaEnum;
            }
            else if (str.StartsWith("schema:"))
            {
                if(!PrionSchemaNested.TryFromString(str, out PrionSchemaNested prionSchemaNested, out error)) return false;
                prionSchemaNode = prionSchemaNested;
            }
            else
            {
                if(!PrionSchemaString.TryFromString(str, out PrionSchemaString prionSchemaString, out error)) return false;
                prionSchemaNode = prionSchemaString;
            }
            return true;
        }
        else
        {
            error = $"Node of type '{prionNode.GetType()}' cannot be converted into a schema node.";
            return false;
        }
    }
    public abstract bool TryValidate(PrionNode prionNode, out string error);
}
