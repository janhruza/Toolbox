using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace VidMaster.Core;

/// <summary>
/// Representing the downloader class.
/// </summary>
public static class Downloader
{
#if WINDOWS
    private static string _path = "yt-dlp.exe";
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
    /// <param name="format">The target video format. Can be null.</param>
    /// <returns>The process exit code.</returns>
    /// <remarks>
    /// If the <paramref name="format"/> is <see langword="null"/>, yt-dlp will use the default format.
    /// </remarks>
    public static async Task<int> Download(string url, string destination, string? format)
    {
        bool formatSpecified = true;
        if (string.IsNullOrWhiteSpace(format) == true)
        {
            formatSpecified = false;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = _path,
            WindowStyle = ProcessWindowStyle.Normal,
            WorkingDirectory = destination,
            ArgumentList =
            {
                "--restrict-filenames",
                "--console-title",
                "--xattrs",
                "--progress",
                "-v",
                "--embed-metadata",
                "--embed-thumbnail",
                "--embed-chapters",
                "--embed-subs",
                url
            }
        };

        if (formatSpecified)
        {
            psi.ArgumentList.Add($"-f {format}");
        }

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
    public static async Task<List<FormatInfo>> GetAvailableFormats(string mediaUrl)
    {
        if (string.IsNullOrWhiteSpace(mediaUrl) || !Downloader.Exists())
        {
            return [];
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = _path,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        // -J (dump-json) returns the entire info as a single JSON object
        psi.ArgumentList.Add("-J");
        psi.ArgumentList.Add(mediaUrl);

        using Process? proc = Process.Start(psi);
        if (proc is null) return [];

        // Capture output
        string jsonOutput = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();

        if (proc.ExitCode != 0) return [];

        try
        {
            // Deserialize the JSON string into our objects
            var data = JsonSerializer.Deserialize(
                jsonOutput,
                AppJsonContext.Default.YtDlpResponse); // Toto je vygenerovaný statický kód);
            return data?.Formats ?? [];
        }
        catch (JsonException)
        {
            // Handle parsing errors
            return [];
        }
    }

    /// <summary>
    /// Checks for yt-dlp updates.
    /// </summary>
    /// <returns>Operation result.</returns>
    public static async Task<bool> CheckForUpdates()
    {
        if (File.Exists(_path) == false)
        {
            return false;
        }

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = _path,
            Arguments = "--update"
        };

        var proc = Process.Start(psi);
        if (proc is null) return false;

        await proc.WaitForExitAsync();
        return proc.ExitCode == 0;
    }
}
