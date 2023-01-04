using VsRemote.Exceptions;

namespace VsRemote.Utils;

public class VsPath
{
    public const char PATH_SEPARATOR = '/'; // it's hardcoded in vscode (or so it seems ...)
    public const string ROOT = "/";
    public static readonly string[] ROOT_PATH = new[] { ROOT };

    public static string[] Split(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new InvalidPath();
        if (path == ROOT)
            return Array.Empty<string>();
        return path.Split(PATH_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string Join(params string[] path)
    {
        if (path == null || path.Length == 0)
            return PATH_SEPARATOR.ToString();
        if (path[0].StartsWith(PATH_SEPARATOR))
            return string.Join(PATH_SEPARATOR, path);
        else
            return PATH_SEPARATOR + string.Join(PATH_SEPARATOR, path);
    }

    public static string Join(string[] path, string lastComponent)
    {
        return Join(Join(path), lastComponent);
    }

    public static bool IsRoot(string path)
    {
        return path.Length == 1 && path[0] == PATH_SEPARATOR;
    }

    public static string RemoveFirstDir(string path)
    {
        if (IsRoot(path))
        {
            throw new InvalidPath();
        }
        return RemoveFirstDir(Split(path));
    }
    public static string RemoveFirstDir(string[] path)
    {
        if (path.Length == 0)
        {
            throw new InvalidPath();
        }
        return Join(path.SkipA());
    }

    public static string Append(string full_path, string el)
    {
        if (full_path.EndsWith(PATH_SEPARATOR))
            return full_path + el;
        else
            return full_path + PATH_SEPARATOR + el;
    }

}
