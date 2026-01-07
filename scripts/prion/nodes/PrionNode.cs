using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Prion.Node;

public abstract class PrionNode
{
    public abstract override string ToString();
    public abstract JsonNode ToJson();
    public static bool TryFromJson(JsonNode jsonNode, out PrionNode prionNode, out string error)
    {
        prionNode = default;
        error = default;
        if(jsonNode is null)
        {
            prionNode = new PrionNull();
            return true;
        }
        var kind = jsonNode.GetValueKind();
        switch (kind)
        {
            case System.Text.Json.JsonValueKind.Undefined:
            case System.Text.Json.JsonValueKind.Null:
                prionNode = new PrionNull();
                return true;
            case System.Text.Json.JsonValueKind.True:
            case System.Text.Json.JsonValueKind.False:
                prionNode = new PrionBoolean(kind == System.Text.Json.JsonValueKind.True);
                return true;
            case System.Text.Json.JsonValueKind.Array:
                if(!PrionArray.TryFromJson(jsonNode, out PrionArray prionArray, out error)) return false;
                prionNode = prionArray;
                return true;
            case System.Text.Json.JsonValueKind.Object:
                if(!PrionDict.TryFromJson(jsonNode, out PrionDict prionDict, out error)) return false;
                prionNode = prionDict;
                return true;
            case System.Text.Json.JsonValueKind.Number:
                if(!jsonNode.AsValue().TryGetValue(out float f))
                {
                    error = "should be unreachable";
                    return false;
                }
                prionNode = new PrionF32(f);
                return true;
            case System.Text.Json.JsonValueKind.String:
                if(!jsonNode.AsValue().TryGetValue(out string str))
                {
                    error = "should be unreachable";
                    return false;
                }
                if(str.StartsWith("color:"))
                {
                    if(!PrionColor.TryFromString(str, out PrionColor prionColor, out error)) return false;
                    prionNode = prionColor;
                }
                else if(str.StartsWith("enum:"))
                {
                    if(!PrionEnum.TryFromString(str, out PrionEnum prionEnum, out error)) return false;
                    prionNode = prionEnum;
                }
                else if(str.StartsWith("f32:"))
                {
                    if(!PrionF32.TryFromString(str, out PrionF32 prionF32, out error)) return false;
                    prionNode = prionF32;
                }
                else if(str.StartsWith("guid:"))
                {
                    if(!PrionGuid.TryFromString(str, out PrionGuid prionGuid, out error)) return false;
                    prionNode = prionGuid;
                }
                else if(str.StartsWith("i32:"))
                {
                    if(!PrionI32.TryFromString(str, out PrionI32 prionI32, out error)) return false;
                    prionNode = prionI32;
                }
                else if(str.StartsWith("rect2i:"))
                {
                    if(!PrionRect2I.TryFromString(str, out PrionRect2I prionRect2I, out error)) return false;
                    prionNode = prionRect2I;
                }
                else if(str.StartsWith("u64:"))
                {
                    if(!PrionU64.TryFromString(str, out PrionU64 prionU64, out error)) return false;
                    prionNode = prionU64;
                }
                else if(str.StartsWith("ubigint:"))
                {
                    if(!PrionUBigInt.TryFromString(str, out PrionUBigInt prionUBigInt, out error)) return false;
                    prionNode = prionUBigInt;
                }
                else if(str.StartsWith("vector2i:"))
                {
                    if(!PrionVector2I.TryFromString(str, out PrionVector2I prionVector2I, out error)) return false;
                    prionNode = prionVector2I;
                }
                else if(str.StartsWith("vector3i:"))
                {
                    if(!PrionVector3I.TryFromString(str, out PrionVector3I prionVector3I, out error)) return false;
                    prionNode = prionVector3I;
                }
                else
                {
                    prionNode = new PrionString(str);
                }
                return true;
            default:
                return false;
        }
    }
    public static bool TryFromJson<T>(JsonNode jsonNode, out T data, out string error) where T : PrionNode
    {
        data = default;
        if(!TryFromJson(jsonNode, out PrionNode prionNode, out error)) return false;
        if(!prionNode.TryAs(out data))
        {
            error = $"Type mismatch. Expected '{typeof(T)}' but received '{prionNode.GetType()}'.";
            return false;
        }
        return true;
    }
    public bool TryAs<T>(out T res)
    {
        res = default;
        if(this is T val)
        {
            res = val;
            return true;
        }
        return false;
    }
    public bool TryGetPath(string pathString, out PrionNode data)
    {
        data = default;
        var path = pathString.Split('/');
        if(path.Length == 0) return false;
        PrionNode value = this;
        foreach (string key in path)
        {
            if(key.Length == 0) return false;
            if(!value.TryGetValue(key, out value)) return false;
        }
        data = value;
        return true;
    }
    public bool TryGetPath<T>(string pathString, out T data) where T : PrionNode
    {
        data = default;
        if(!TryGetPath(pathString, out PrionNode node)) return false;
        if(node is T res)
        {
            data = res;
            return true;
        }
        return false;
    }
    public bool TryGetPath(string pathString, out string data)
    {
        data = default;
        if(!TryGetPath(pathString, out PrionString prionString)) return false;
        data = prionString.Value;
        return true;
    }
    public bool TryGetPath(string pathString, out Guid data)
    {
        data = default;
        if(!TryGetPath(pathString, out PrionGuid prionGuid)) return false;
        data = prionGuid.Value;
        return true;
    }
    public bool TrySetPath(string pathString, PrionNode node, bool canAdd = false, bool canChangeType = false)
    {
        var path = pathString.Split('/');
        if(path.Length == 0) return false;
        PrionNode value = this;
        PrionNode lastValue = this;
        string key = "";
        for (int idx = 0; idx < path.Length; idx++)
        {
            key = path[idx];
            lastValue = value;
            if(key.Length == 0) return false;
            if(!value.TryGetValue(key, out PrionNode nextValue))
            {
                if(!canAdd) return false;
                if(idx == path.Length - 1) return value.TrySetValue(key, ref node);
                PrionDict dict = new();
                if(!value.TrySetValue(key, ref dict)) return false;
                value = dict;
            }
            else value = nextValue;
        }
        bool res;
        if(canChangeType) res = true;
        else
        { 
            if(!lastValue.TryGetValue(key, out PrionNode prionNode)) return false;
            else res = node.GetType() == prionNode.GetType();
        }
        if (res)
        {
            res = lastValue.TrySetValue(key, ref node);
        }
        return res;
    }
    public bool TrySetPath(string pathString, string value, bool canAdd = false, bool canChangeType = false)
    {
        return TrySetPath(pathString, new PrionString(value), canAdd, canChangeType);
    }
    public bool TrySetPath(string pathString, Guid value, bool canAdd = false, bool canChangeType = false)
    {
        return TrySetPath(pathString, new PrionGuid(value), canAdd, canChangeType);
    }
    protected virtual bool TryGetValue(string key, out PrionNode prionNode)
    {
        prionNode = default;
        return false;
    }
    protected virtual bool TrySetValue<T>(string key, ref T prionNode) where T : PrionNode
    {
        return false;
    }
    protected virtual bool TrySetValue(string key, PrionNode value)
    {
        return false;
    }
}
