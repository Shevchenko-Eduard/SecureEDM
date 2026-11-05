namespace SecureEDM.Client.Infrastructure;

public static class AppDataDir
{
    private const string Dictionary = "SecureEDM";
    private const string SqliteFile = "SecureEDM.db";
    private const string SettingFile = "Setting.json";
    public static string AppData => GetPathToAppDataDir();
    public static string Sqlite => GetPathToSqliteFile();
    public static string Setting => GetPathToSettingFile();
    private static string GetPathToAppDataDir()
    {
        Environment.SpecialFolder specialFolder = Environment.SpecialFolder.LocalApplicationData;
        string folder = Environment.GetFolderPath(specialFolder);
        string path = Path.Combine(folder, Dictionary);
        CreatedPath(path);
        return path;
    }
    private static string GetPathToSqliteFile()
    {
        string path = Path.Combine(AppData, SqliteFile);
        return path;
    }
    private static string GetPathToSettingFile()
    {
        string path = Path.Combine(AppData, SettingFile);
        return path;
    }
    private static void CreatedPath(string path)
    {
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }
}