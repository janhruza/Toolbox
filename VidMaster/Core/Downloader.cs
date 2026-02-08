using System.Diagnostics;
using System.IO;

namespace VidMaster.Core;

/// <summary>
/// Representing the downloader class.
/// </summary>
public static class Downloader
{
    static string _path = "yt-dlp";

    /// <summary>
    /// Determines whether the downloader exists.
    /// </summary>
    /// <returns>Operation result.</returns>
    public static bool Exists() => File.Exists(_path);

    /// <summary>
    /// Opens the yt-dlp's latest release download page.
    /// </summary>
    public static void GetDownloader()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "https://github.com/yt-dlp/yt-dlp/releases/latest",
            UseShellExecute = true
        };

        _ = Process.Start(psi);
        return;
    }
}
