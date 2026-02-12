using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

namespace Mediavax;

internal class Program : IApplication
{
    public static string _ytdlp_version;

    internal static string _path;

    static Program()
    {
        Program._ytdlp_version = string.Empty;
        Program._path = string.Empty;
    }

    public static Dictionary<string, (string, string)> ThemeOptions =>
        new Dictionary<string, (string AccentStyle, string HighlightStyle)>
        {
            { "Default", ("\e[38;5;210m", "\e[48;5;210m\e[38;5;0m") },
            { "Red", ("\e[38;5;196m", "\e[48;5;196m\e[38;5;15m") },
            { "Green", ("\e[38;5;46m", "\e[48;5;46m\e[38;5;0m") },
            { "Blue", ("\e[38;5;21m", "\e[48;5;21m\e[38;5;15m") },
            { "Cyan", ("\e[38;5;51m", "\e[48;5;51m\e[38;5;0m") },
            { "Magenta", ("\e[38;5;201m", "\e[48;5;201m\e[38;5;0m") },
            { "Yellow", ("\e[38;5;226m", "\e[48;5;226m\e[38;5;0m") },
            { "Orange", ("\e[38;5;214m", "\e[48;5;214m\e[38;5;0m") },
            { "Purple", ("\e[38;5;93m", "\e[48;5;93m\e[38;5;15m") },
            { "Pink", ("\e[38;5;205m", "\e[48;5;205m\e[38;5;0m") }
        };

    /// <summary>
    ///     Gets the name of the yt-dlp executable name (dynamic, different on Linux, Mac and Windows).
    /// </summary>
    public static string YT_DLP => Environment.OSVersion.Platform == PlatformID.Win32NT ? "yt-dlp.exe" : "yt-dlp";

    public static void DisplayBanner()
    {
        Console.WriteLine(value: "\e[38;5;213m███╗   ███╗███████╗██████╗ ██╗ █████╗ ██╗   ██╗ █████╗ ██╗  ██╗\e[0m");
        Console.WriteLine(value: "\e[38;5;212m████╗ ████║██╔════╝██╔══██╗██║██╔══██╗██║   ██║██╔══██╗╚██╗██╔╝\e[0m");
        Console.WriteLine(value: "\e[38;5;211m██╔████╔██║█████╗  ██║  ██║██║███████║██║   ██║███████║ ╚███╔╝ \e[0m");
        Console.WriteLine(value: "\e[38;5;210m██║╚██╔╝██║██╔══╝  ██║  ██║██║██╔══██║╚██╗ ██╔╝██╔══██║ ██╔██╗ \e[0m");
        Console.WriteLine(value: "\e[38;5;209m██║ ╚═╝ ██║███████╗██████╔╝██║██║  ██║ ╚████╔╝ ██║  ██║██╔╝ ██╗\e[0m");
        Console.WriteLine(value: "\e[38;5;208m╚═╝     ╚═╝╚══════╝╚═════╝ ╚═╝╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝╚═╝  ╚═╝\e[0m");
        Console.WriteLine(
            value: $"Simple yt-dlp wrapper!                           by {Terminal.AccentTextStyle}@jendahruza");
        Console.WriteLine();
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
        Console.Clear();
    }

    public static void LoadConfig()
    {
        // NOTE: implemented only to satisfy the interface requirement, no real use in this app
    }

    /// <summary>
    ///     Interface implementation, unused.
    /// </summary>
    public static Version Version => new();

    /// <summary>
    ///     Checks if YT-DLP exists in local directory, in directory defined in the PATH variable, both directories or none at
    ///     all.
    /// </summary>
    /// <returns></returns>
    public static (bool yt_dlpExists, bool yt_dlpExistsPath) DownloaderExists()
    {
        bool yt_dlpExistsPath = false;

        // Semicolot (;)    - Windows
        // Colon (:)        - Linux & macOS
        char separator = Environment.OSVersion.Platform == PlatformID.Win32NT ? ';' : ':';

        if (Environment.GetEnvironmentVariable(variable: "PATH") is string pathValue)
        {
            string[] dirs = pathValue.Split(separator: separator, options: StringSplitOptions.RemoveEmptyEntries);
            foreach (string dir in dirs)
            {
                string fullPath = Path.Combine(path1: dir, path2: Program.YT_DLP);
                if (File.Exists(path: fullPath))
                {
                    yt_dlpExistsPath = true;
                    Program._path = fullPath;
                    break; // Optional: stop after first match
                }
            }
        }

        else
        {
            yt_dlpExistsPath = false;
        }

        // TODO: check if yt-dlp exists in local path, PATH env. variable or both
        // check if local file exists
        bool yt_dlpExists = File.Exists(path: Program.YT_DLP);

        // return
        return (yt_dlpExists, yt_dlpExistsPath);
    }

    public static bool GetDownloaderVersion(out string version)
    {
        version = string.Empty;

        (bool b1, bool b2) = Program.DownloaderExists();
        if (!b1 && !b2)
        {
            _ = Log.Error(message: "YT-DLP not found.");
            return false;
        }

        try
        {
            Console.Write(value: "Checking YT-DLP version... ");
            if (Toolbox.Core.CreateProcess(command: Program.GetDownloader(), arguments: "--version",
                    process: out Process? proc, shellExec: false, directory: string.Empty))
            {
                proc.WaitForExit();
                version = proc.StandardOutput.ReadToEnd().Trim();
                return true;
            }

            return false;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex);
            return false;
        }
    }

    public static bool GetDownloaderVersion()
    {
        return Program.GetDownloaderVersion(version: out Program._ytdlp_version);
    }

    /// <summary>
    ///     Gets path of the found YT-DLP.
    /// </summary>
    /// <returns>Path to the YT-DLP or <see cref="string.Empty" /> if no downloader is found.</returns>
    public static string GetDownloader()
    {
        (bool b1, bool b2) = Program.DownloaderExists();
        if (!b1 && !b2) return string.Empty;

        if (b1) return Program.YT_DLP;

        return Program._path;
    }

    /// <summary>
    ///     Processes the input action <paramref name="actionResult" /> and prints either a successful
    ///     <paramref name="trueMessage" /> or failed <paramref name="falseMessage" />.
    /// </summary>
    /// <param name="actionResult">Input operation result.</param>
    /// <param name="trueMessage">
    ///     A message that will be printed if the <paramref name="actionResult" /> is evaluated as
    ///     <see langword="true" />.
    /// </param>
    /// <param name="falseMessage">
    ///     A message that will be printed if the <paramref name="actionResult" /> is evaluated as
    ///     <see langword="false" />.
    /// </param>
    public static void HandleAction(bool actionResult, string trueMessage, string falseMessage)
    {
        if (actionResult)
        {
            Console.Clear();
            Console.Write(value: $"\e[48;5;10m\e[38;5;0m GOOD {ANSI.ANSI_RESET} ");
            Console.WriteLine(value: trueMessage);
        }

        else
        {
            Console.Clear();
            Console.Write(value: $"\e[48;5;1m\e[38;5;15m FAIL {ANSI.ANSI_RESET} ");
            Console.WriteLine(value: falseMessage);
        }

        Terminal.Pause();
    }

    /// <summary>
    ///     Ensures a valid alt text based on <paramref name="value" />.
    /// </summary>
    /// <param name="value">A value to evaluate.</param>
    /// <param name="alt">Alternative text if the <paramref name="value" /> is empty.</param>
    /// <returns>
    ///     The original <paramref name="value" /> or <paramref name="alt" />.
    /// </returns>
    public static string ActionText(string value, string alt = "[default]")
    {
        return string.IsNullOrWhiteSpace(value: value) ? alt : value;
    }

    private static string GetDisplayLocation()
    {
        if (MediaItem.Current.Location.StartsWith(
                value: Environment.GetFolderPath(folder: Environment.SpecialFolder.UserProfile)))
            return MediaItem.Current.Location.Replace(
                oldValue: Environment.GetFolderPath(folder: Environment.SpecialFolder.UserProfile),
                newValue: "~");

        return MediaItem.Current.Location;
    }

    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            // arguments are disabled
            Console.WriteLine(value: "Arguments are disabled.");
            return 1;
        }

        // initialize
        _ = Setup.Initialize();
        Console.Title = "Mediavax - YT-DLP wrapper";

        // get downloader version
        if (!Program.GetDownloaderVersion(version: out Program._ytdlp_version))
        {
            _ = Log.Error(message: "YT-DLP was not found in either PATH or local folder.");
            Console.Error.WriteLine(value: "YT-DLP was not found.");
            return 0xDEAD;
        }

        // set terminal size
        //Console.Write("\e[8;25;80t");

        // main app loop
        while (true)
        {
            // clear & display banner
            Console.Clear();
            Program.DisplayBanner();

            // draw menu
            MenuItemCollection menu = new()
            {
                new MenuItem(id: (int)MenuIds.ID_SELECT_URL, text: "Media",
                    alt: Program.ActionText(value: MediaItem.Current.Address, alt: "[select]")),
                new MenuItem(id: (int)MenuIds.ID_SELECT_FORMAT, text: "Format",
                    alt: Program.ActionText(value: MediaItem.Current.Format)),
                new MenuItem(id: (int)MenuIds.ID_SELECT_LOCATION, text: "Location",
                    alt: Program.ActionText(value: Program.GetDisplayLocation())),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXTRAS, text: "Extras", alt: ">"),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_START_DOWNLOAD, text: "Verify & Download"),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Exit", alt: "ESC")
            };

            // get selected menu item
            int option = ConsoleMenu.SelectMenu(items: menu);

            switch (option)
            {
                // exit the application
                case -1: // ESC key pressed
                case (int)MenuIds.ID_EXIT:
                    goto AppExit;

                // select media URL
                case (int)MenuIds.ID_SELECT_URL:
                {
                    Console.Clear();
                    _ = MenuActions.SelectMediaSource();
                }
                    break;

                // select mode: audio-only, video-only, both
                case (int)MenuIds.ID_SELECT_MODE:
                    break;

                // select download quality
                case (int)MenuIds.ID_SELECT_QUALITY:
                    break;

                // select output format: MP4, MP3, etc
                case (int)MenuIds.ID_SELECT_FORMAT:
                {
                    Console.Clear();
                    if (!MenuActions.ListFormats())
                    {
                        Console.WriteLine(value: "Unable to get available formats.");
                        Terminal.Pause();
                    }
                }
                    break;

                // select output directory
                case (int)MenuIds.ID_SELECT_LOCATION:
                {
                    Console.Clear();
                    _ = MenuActions.SelectLocation();
                }
                    break;

                // open extras menu
                case (int)MenuIds.ID_EXTRAS:
                {
                    Console.Clear();
                    _ = MenuActions.OpenExtras();
                }
                    break;

                // start the download
                case (int)MenuIds.ID_START_DOWNLOAD:
                {
                    Console.Clear();
                    if (MenuActions.VerifyDownload())
                    {
                        Console.Clear();
                        if (!MenuActions.StartDownload())
                        {
                            _ = Log.Error(message: "Download process failed.", tag: nameof(Program.Main));
                            Console.WriteLine(
                                value:
                                $"{Environment.NewLine}Download process {Terminal.AccentTextStyle}failed{ANSI.ANSI_RESET}.");
                        }

                        else
                        {
                            _ = Log.Success(message: "Download completed.", tag: nameof(Program.Main));
                            Console.WriteLine(
                                value:
                                $"{Environment.NewLine}Download {Terminal.AccentTextStyle}completed{ANSI.ANSI_RESET}.");

                            // set a new download item
                            MediaItem.Current = new MediaItem();
                        }

                        Terminal.Pause();
                    }
                }
                    break;

                // undefined option
            }
        }

        // app exit
        AppExit:
        Program.PostExitCleanup();
        return 0;
    }
}