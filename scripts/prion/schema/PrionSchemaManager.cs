using System;
using System.Collections.Generic;
using Prion.Node;

namespace Prion.Schema;

public static class PrionSchemaManager
{
    static readonly Dictionary<Type, PrionSchema> SchemasByType = [];
    static readonly Dictionary<string, PrionSchema> SchemasByName = [];
    public static bool SchemaExists(string name){return SchemasByName.ContainsKey(name);}
    public static bool SchemaExists(Type type){return SchemasByType.ContainsKey(type);}
    public static void RegisterSchema(PrionSchema schema, Type type = null)
    {
        if(type is not null) SchemasByType.Add(type, schema);
        SchemasByName.Add(schema.Name, schema);
    }
    public static void UpdateSchema(PrionSchema schema, Type type = null)
    {
        if(SchemasByName.ContainsKey(schema.Name)) throw new Exception("Attempted to update a nonexistent schema");
        if(type is not null)
        {
            if(!SchemasByType.ContainsKey(type)) throw new Exception("Attempted to update a nonexistent schema");
            SchemasByType[type] = schema;
        }
        SchemasByName[schema.Name] = schema;
    }
    public static bool Validate(Type type, PrionNode node, out string error)
    {
        if(!SchemasByType.TryGetValue(type, out PrionSchema schema))
        {
            error = $"Could not find schema for type '{type}'.";
            return false;
        }
        return schema.TryValidate(node, out error);
    }
    public static bool Validate(string name, PrionNode node, out string error)
    {
        if(!SchemasByName.TryGetValue(name, out PrionSchema schema))
        {
            error = $"Could not find schema for name '{name}'.";
            return false;
        }
        return schema.TryValidate(node, out error);
    }
    public static void Clear()
    {
        SchemasByType.Clear();
        SchemasByName.Clear();
    }
}
