using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VidMaster.Core;

/// <summary>
/// Representing the downloader class.
/// </summary>
public static class Downloader
{
#if WINDOWS
    static string _path = "yt-dlp.exe";
#else
    static string _path = "yt-dlp";
#endif

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

    /// <summary>
    /// Begins downloading of the given media (<paramref name="url"/>) and saves it into the <paramref name="destination"/> folder.
    /// </summary>
    /// <param name="url">Media source.</param>
    /// <param name="destination">Destination folder.</param>
    /// <returns>The process exit code.</returns>
    /// <remarks>
    /// This method downloads the media with all the default settings.
    /// </remarks>
    public static async Task<int> Download(string url, string destination)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = _path,
            ArgumentList =
            {
                "--restrict-filenames",
                "--console-title",
                "--xattrs",
                "--progress",
                "-v",
                url
            },
            WindowStyle = ProcessWindowStyle.Normal,
            WorkingDirectory = destination
        };

        Process? proc = Process.Start(psi);
        if (proc is null)
        {
            return int.MaxValue;
        }

        await proc.WaitForExitAsync();
        return proc.ExitCode;
    }

    /// <summary>
    /// Gets the list of available media formats for the specified media at <paramref name="mediaUrl"/>.
    /// </summary>
    /// <param name="mediaUrl">Target media URL address.</param>
    /// <returns>A list of available formats. Can be empty.</returns>
    public static async Task<List<string>> GetAvailableFormats(string mediaUrl)
    {
        List<string> formats = [];
        return formats;
    }
}
