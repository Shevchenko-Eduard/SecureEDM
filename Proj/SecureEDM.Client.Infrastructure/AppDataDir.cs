namespace SecureEDM.Client.Infrastructure;

public static class AppDataDir
{
    private const string DictionaryName = "SecureEDM";
    public static string GetPath()
    {
        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.LocalApplicationData;
        string folder = Environment.GetFolderPath(specialFolder);
        string path = Path.Combine(folder, DictionaryName);
        CreatedPath(path);
        return path;
    }
    private static void CreatedPath(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}