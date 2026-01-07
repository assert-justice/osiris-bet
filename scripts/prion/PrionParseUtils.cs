
namespace Prion;
public static class PrionParseUtils
{
    public const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";
    public const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Numeric = "0123456789";
    public static bool IsAlpha(char c)
    {
        return IsInRange(c, 'a', 'z') || IsInRange(c, 'A', 'Z');
    }
    public static bool IsNumeric(char c)
    {
        return IsInRange(c, '0', '9');
    }
    public static bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsNumeric(c);
    }
    public static bool IsAlphanumericOrUnderscore(char c)
    {
        return IsAlpha(c) || IsNumeric(c) || c == '_';
    }
    public static bool IsInRange(char c, int min, int max)
    {
        return c >= min && c <= max;
    }
    public static bool IsInRange(string s, int min, int max)
    {
        foreach (char c in s)
        {
            if(!IsInRange(c, min, max)) return false;
        }
        return true;
    }
    public static bool MatchStart(string pattern, ref string str)
    {
        if (str.StartsWith(pattern))
        {
            str = str[pattern.Length..];
            return true;
        }
        return false;
    }
}
