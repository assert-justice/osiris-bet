using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionDict : PrionNode
{
	public readonly Dictionary<string, PrionNode> Value = [];
	public static bool TryFromJson(JsonNode jsonNode, out PrionDict prionDict, out string error)
	{
		error = default;
		prionDict = default;
		if(jsonNode is null)
		{
			error = "Invalid json kind. Expected an object, received null.";
			return false;
		}
		var kind = jsonNode.GetValueKind();
		prionDict = new();
		if(kind != System.Text.Json.JsonValueKind.Object)
		{
			error = $"Invalid json kind. Expected an object, received '{kind}'.";
			return false;
		}
		foreach (var (key, value) in jsonNode.AsObject())
		{
			if(!TryFromJson(value, out PrionNode prionNode, out error)) return false;
			prionDict.Value.Add(key, prionNode);
		}
		return true;
	}
	public override JsonNode ToJson()
	{
		JsonObject obj = [];
		foreach (var (key, value) in Value)
		{
			obj.Add(key, value.ToJson());
		}
		return obj;
	}
	public override string ToString()
	{
		return "PrionDict";
	}

	public bool TryGet<T>(string key, out T value) where T : PrionNode
	{
		value = default;
		if(!Value.TryGetValue(key, out PrionNode res)) return false;
		if(res is not T) return false;
		value = res as T;
		return true;
	}
	public bool TryGet(string key, out Guid value)
	{
		value = default;
		if(!TryGet(key, out PrionGuid res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out bool value)
	{
		value = default;
		if(!TryGet(key, out PrionBoolean res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out string value)
	{
		value = default;
		if(!TryGet(key, out PrionString res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out float value)
	{
		value = default;
		if(!TryGet(key, out PrionF32 res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out int value)
	{
		value = default;
		if(!TryGet(key, out PrionI32 res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out ulong value)
	{
		value = default;
		if(!TryGet(key, out PrionU64 res)) return false;
		value = res.Value;
		return true;
	}
	public bool TryGet(string key, out HashSet<Guid> guids)
	{
		guids = default;
		if(!TryGet(key, out PrionArray res)) return false;
		if(!res.TryAs(out List<PrionGuid> prionGuids)) return false;
		guids = [.. prionGuids.Select(o => o.Value)];
		return true;
	}
	public T GetDefault<T>(string key, T defaultVal) where T : PrionNode
	{
		if(!TryGet(key, out T value)) return defaultVal;
		return value;
	}
	public string GetDefault(string key, string defaultVal)
	{
		if(!TryGet(key, out PrionString prionString)) return defaultVal;
		return prionString.Value;
	}
	public void Set(string key, Guid guid)
	{
		Value[key] = new PrionGuid(guid);
	}
	public void Set(string key, bool value)
	{
		Value[key] = new PrionBoolean(value);
	}
	public void Set<T>(string key, T value) where T : PrionNode
	{
		Value[key] = value;
	}
	public void Set(string key, string str)
	{
		Value[key] = new PrionString(str);
	}
	public void Set(string key, float value)
	{
		Value[key] = new PrionF32(value);
	}
	public void Set(string key, int value)
	{
		Value[key] = new PrionI32(value);
	}
	public void Set(string key, ulong value)
	{
		Value[key] = new PrionU64(value);
	}
	public void Set(string key, HashSet<Guid> guids)
	{
		PrionArray prionArray = new();
		foreach (var guid in guids)
		{
			prionArray.Value.Add(new PrionGuid(guid));
		}
		Set(key, prionArray);
	}
	protected override bool TryGetValue(string key, out PrionNode prionNode)
	{
		if(!Value.TryGetValue(key, out prionNode)) return false;
		return true;
	}
	protected override bool TrySetValue<T>(string key, ref T prionNode)
	{
		Value[key] = prionNode;
		return true;
	}
}
