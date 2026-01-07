using System.Collections.Generic;
using Godot;

namespace Osiris.System;

public class OsiFs
{
    public virtual bool FileExists(string path)
    {
        return FileAccess.FileExists(path);
    }
    public virtual bool DirectoryExists(string path)
    {
        return DirAccess.DirExistsAbsolute(path);
    }
    public virtual string ReadFile(string path)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if(file is null)
		{
			OsiSystem.Logger.ReportError($"Could not open file at path '{path}'");
			return "";
		}
		return file.GetAsText();
    }
    public virtual void WriteFile(string path, string contents)
    {
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(contents);
    }
    public virtual string[] DirectoryListFiles(string path)
    {
        using var dir = DirAccess.Open(path);
		if(dir is null)
		{
			OsiSystem.Logger.ReportError($"Failed to open directory at path '{path}'.");
			return [];
		}
		List<string> filenames = [];
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir())
				{
					filenames.Add(fileName);
				}
				fileName = dir.GetNext();
			}
		}
		return [..filenames];
    }
    public virtual string[] DirectoryListDirectories(string path)
    {
        using var dir = DirAccess.Open(path);
		if(dir is null)
		{
			OsiSystem.Logger.ReportError($"Failed to open directory at path '{path}'.");
			return [];
		}
		List<string> filenames = [];
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (dir.CurrentIsDir())
				{
					filenames.Add(fileName);
				}
				fileName = dir.GetNext();
			}
		}
		return [..filenames];
    }
}
