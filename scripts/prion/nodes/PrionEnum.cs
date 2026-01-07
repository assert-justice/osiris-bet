using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Prion.Node;

public class PrionEnum : PrionNode
{
    public readonly string[] Options;
    int _Index ;
    public int Index
    {
        get => _Index;
        set
        {
            _Index = value % Options.Length;
        }
    }
    PrionEnum(string[] options, int index)
    {
        Options = options;
        _Index = index;
    }
    public string GetValue()
    {
        return Options[_Index];
    }

    public override JsonNode ToJson()
    {
        return JsonNode.Parse($"\"{ToString()}\"");
    }
    public override string ToString()
    {
        string res = string.Join(", ", Options);
        return "enum: " + res + ": " + GetValue();
    }
    public static bool TryFromOptions(string optionsStr, int idx, out PrionEnum prionEnum, out string error)
    {
        prionEnum = default;
        error = default;
        var options = optionsStr.Split(',').Select(s => s.Trim());
        if(new HashSet<string>([..options]).Count < options.Count())
        {
            error = "Options contain duplicate values.";
            return false;
        }
        if(idx < 0 || idx >= options.Count())
        {
            error = "Enum index is out of bounds.";
            return false;
        }
        prionEnum = new([..options], idx);
        return true;
    }
    public static bool TryFromOptions(string optionsStr, string selected, out PrionEnum prionEnum, out string error)
    {
        if(!TryFromOptions(optionsStr, 0, out prionEnum, out error)) return false;
        if (!prionEnum.TrySetValue(selected))
        {
            prionEnum = default;
            error = $"Invalid enum value '{selected}'";
            return false;
        }
        return true;
    }
    public static bool TryFromString(string value, out PrionEnum node, out string error)
    {
        node = default;
        error = default;
        value = value.Trim();
        var sections = value.Split(':').Select(s => s.Trim()).ToArray();
        if(sections.Length != 3)
        {
            error = $"Could not parse enum. Expected 3 sections, found {sections.Length}";
            return false;
        }
        if (sections[0] != "enum")
        {
            error = $"Enum signature not present at start of string '{value}'.";
            return false;
        }
        var options = sections[1].Split(',').Select(s => s.Trim()).ToArray();
        foreach (var option in options)
        {
            foreach (char c in option)
            {
                if(!PrionParseUtils.IsAlphanumericOrUnderscore(c))
                {
                    error = $"Unexpected character '{c}' in enum option '{option}'. only alphanumeric characters or underscores are allowed.";
                    return false;
                }
            }
        }
        string selected = sections[2];
        if (!options.Contains(selected))
        {
            error = $"Enum options '{string.Join(", ", options)}' do not contain selected option '{selected}'.";
            return false;
        }
        node = new PrionEnum(options, 0);
        node.TrySetValue(selected);
        return true;
    }
    public static bool TryFromJson(JsonNode jsonNode, out PrionEnum node, out string error)
    {
        node = default;
        if(jsonNode is null)
        {
            error = "Invalid json kind. Value cannot be null.";
            return false;
        }
        var kind = jsonNode.GetValueKind();
        if(kind == System.Text.Json.JsonValueKind.String)
        {
            if(jsonNode.AsValue().TryGetValue(out string sValue))
            {
                return TryFromString(sValue, out node, out error);
            }
            error = "Should be unreachable";
            return false;
        }
        else
        {
            error = "Invalid json kind";
            return false;
        }
    }
    public bool TrySetValue(string value)
    {
        int idx = Options.ToList().FindIndex(s => s == value);
        if(idx == -1)return false;
        _Index = idx;
        return true;
    }
    public bool MatchOptions(string optionsStr)
    {
        string[] options = [.. optionsStr.Split(',').Select(s => s.Trim())];
        if(options.Length != Options.Length) return false;
        for (int idx = 0; idx < options.Length; idx++)
        {
            if(options[idx] != Options[idx]) return false;
        }
        return true;
    }
}
