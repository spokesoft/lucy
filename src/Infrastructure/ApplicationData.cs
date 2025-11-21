using System.Runtime.InteropServices;

namespace Lucy.Infrastructure;

/// <summary>
/// Provides the application data path based on the operating system.
/// </summary>
public static class ApplicationData
{
    /// <summary>
    /// Caches the application data path once determined.
    /// </summary>
    public static string? CachedAppDataPath { get; set;}

    /// <summary>
    /// Gets the application data path for Lucy.
    /// </summary>
    public static string GetPath()
    {
        var isDevelopment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development";

        if (isDevelopment)
            return AppContext.BaseDirectory;

        if (CachedAppDataPath != null)
            return CachedAppDataPath;

        string basePath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // %USERPROFILE%\AppData\Local\{appName}
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // $HOME/Library/Application Support/{appName}
            basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Library", "Application Support"
            );
        }
        else
        {
            // $HOME/.local/share/{appName}
            basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                ".local", "share"
            );
        }

        bool isWindowsOrOsx = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        var path = isWindowsOrOsx
            ? Path.Combine(basePath, "Spokesoft", "Lucy")
            : Path.Combine(basePath, "spokesoft", "lucy");

        EnsureDirectoryAndAppSettingsExists(path);
        CachedAppDataPath = path;
        return path;
    }

    /// <summary>
    /// Ensures that the specified directory exists and contains the
    /// appsettings.json file.
    /// </summary>
    private static void EnsureDirectoryAndAppSettingsExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var destFileName = Path.Combine(path, "appsettings.json");

        if (!File.Exists(destFileName))
        {
            var sourceFileName = Path.Combine(
                AppContext.BaseDirectory,
                "appsettings.json");

            File.Copy(sourceFileName, destFileName);
        }
    }
}
