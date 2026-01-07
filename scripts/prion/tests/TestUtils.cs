using System.IO;
public static class TestUtils
{
    public static string ReadFile(string filename)
    {
        string filepath = $"../../../../../{filename}";
        return File.ReadAllText(filepath);
    }
    public static bool FileExists(string filename)
    {
        string filepath = $"../../../../../{filename}";
        return File.Exists(filepath);
    }
    public static bool DirExists(string path)
    {
        path = $"../../../../../{path}";
        return Directory.Exists(path);
    }
    public static string[] DirGetFilenames(string path)
    {
        path = $"../../../../../{path}";
        return Directory.GetFiles(path);
    }
}
