using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.X86;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

using static Mediavax.MenuIds;
using static Toolbox.ANSI;

namespace Mediavax;

internal class Program : IApplication
{
    static Program()
    {
        _ytdlp_version = string.Empty;
    }

    public static void DisplayBanner()
    {
        Console.WriteLine($"\e[38;5;213m███╗   ███╗███████╗██████╗ ██╗ █████╗ ██╗   ██╗ █████╗ ██╗  ██╗\e[0m");
        Console.WriteLine($"\e[38;5;212m████╗ ████║██╔════╝██╔══██╗██║██╔══██╗██║   ██║██╔══██╗╚██╗██╔╝\e[0m");
        Console.WriteLine($"\e[38;5;211m██╔████╔██║█████╗  ██║  ██║██║███████║██║   ██║███████║ ╚███╔╝ \e[0m");
        Console.WriteLine($"\e[38;5;210m██║╚██╔╝██║██╔══╝  ██║  ██║██║██╔══██║╚██╗ ██╔╝██╔══██║ ██╔██╗ \e[0m");
        Console.WriteLine($"\e[38;5;209m██║ ╚═╝ ██║███████╗██████╔╝██║██║  ██║ ╚████╔╝ ██║  ██║██╔╝ ██╗\e[0m");
        Console.WriteLine($"\e[38;5;208m╚═╝     ╚═╝╚══════╝╚═════╝ ╚═╝╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝╚═╝  ╚═╝\e[0m");
        Console.WriteLine($"Simple yt-dlp wrapper!                           by {Terminal.AccentTextStyle}@jendahruza");
        Console.WriteLine();

        return;
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
        Console.Clear();
        return;
    }

    /// <summary>
    /// Gets the name of the yt-dlp executable name (dynamic, different on Linux, Mac and Windows).
    /// </summary>
    public static string YT_DLP => Environment.OSVersion.Platform == PlatformID.Win32NT ? "yt-dlp.exe" : "yt-dlp";

    /// <summary>
    /// Checks if YT-DLP exists in local directory, in directory defined in the PATH variable, both directories or none at all.
    /// </summary>
    /// <returns></returns>
    public static (bool yt_dlpExists, bool yt_dlpExistsPath) DownloaderExists()
    {
        // TODO: check if yt-dlp exists in local path, PATH env. variable or both
        bool yt_dlpExists = false;
        bool yt_dlpExistsPath = false;

        // Semicolot (;)    - Windows
        // Colon (:)        - Linux & macOS
        char separator = Environment.OSVersion.Platform == PlatformID.Win32NT ? ';' : ':';

        if (Environment.GetEnvironmentVariable("PATH") is string pathValue)
        {
            string[] dirs = pathValue.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach (string dir in dirs)
            {
                string fullPath = Path.Combine(dir, YT_DLP);
                if (File.Exists(fullPath))
                {
                    yt_dlpExistsPath = true;
                    _path = fullPath;
                    break; // Optional: stop after first match
                }
            }
        }

        else
        {
            yt_dlpExistsPath = false;
        }

        // check if local file exists
        yt_dlpExists = File.Exists(YT_DLP);

        // return
        return (yt_dlpExists, yt_dlpExistsPath);
    }

    public static string _ytdlp_version;

    public static bool GetDownloaderVersion(out string version)
    {
        version = string.Empty;

        (bool b1, bool b2) = DownloaderExists();
        if (b1 == false && b2 == false)
        {
            Log.Error("YT-DLP not found.");
            return false;
        }

        try
        {
            Console.Write("Checking YT-DLP version... ");
            if (Toolbox.Core.CreateProcess(GetDownloader(), $"--version", out Process? proc, false, string.Empty) == true)
            {
                proc.WaitForExit();
                version = proc.StandardOutput.ReadToEnd().Trim();
                return true;
            }

            else
            {
                return false;
            }
        }

        catch (Exception ex)
        {
            Log.Exception(ex);
            return false;
        }
    }

    public static bool GetDownloaderVersion()
    {
        return GetDownloaderVersion(out _ytdlp_version);
    }

    internal static string _path;

    /// <summary>
    /// Gets path of the found YT-DLP.
    /// </summary>
    /// <returns>Path to the YT-DLP or <see cref="string.Empty"/> if no downloader is found.</returns>
    public static string GetDownloader()
    {
        (bool b1, bool b2) = DownloaderExists();
        if (b1 == false && b2 == false)
        {
            return string.Empty;
        }

        else if (b1 == true)
        {
            return YT_DLP;
        }

        else
        {
            return _path;
        }
    }

    /// <summary>
    /// Processes the input action <paramref name="actionResult"/> and prints either a successful <paramref name="trueMessage"/> or failed <paramref name="falseMessage"/>.
    /// </summary>
    /// <param name="actionResult">Input operation result.</param>
    /// <param name="trueMessage">A message that will be printed if the <paramref name="actionResult"/> is evaluated as <see langword="true"/>.</param>
    /// <param name="falseMessage">A message that will be printed if the <paramref name="actionResult"/> is evaluated as <see langword="false"/>.</param>
    public static void HandleAction(bool actionResult, string trueMessage, string falseMessage)
    {
        if (actionResult)
        {
            Console.Clear();
            Console.Write($"\e[48;5;10m\e[38;5;0m GOOD {ANSI_RESET} ");
            Console.WriteLine(trueMessage);
        }

        else
        {
            Console.Clear();
            Console.Write($"\e[48;5;1m\e[38;5;15m FAIL {ANSI_RESET} ");
            Console.WriteLine(falseMessage);
        }

        Terminal.Pause();
        return;
    }

    /// <summary>
    /// Ensures a valid alt text based on <paramref name="value"/>.
    /// </summary>
    /// <param name="value">A value to evaluate.</param>
    /// <param name="alt">Alternative text if the <paramref name="value"/> is empty.</param>
    /// <returns>
    /// The original <paramref name="value"/> or <paramref name="alt"/>.
    /// </returns>
    public static string ActionText(string value, string alt = "[default]")
    {
        return string.IsNullOrWhiteSpace(value) ? alt : value;
    }

    static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            // arguments are disabled
            Console.WriteLine("Arguments are disabled.");
            return 1;
        }

        // initialize
        Setup.Initialize();
        Console.Title = "Mediavax - YT-DLP wrapper";

        // get downloader version
        if (GetDownloaderVersion(out _ytdlp_version) == false)
        {
            Log.Error("YT-DLP was not found in either PATH or local folder.");
            Console.Error.WriteLine($"YT-DLP was not found.");
            return 0xDEAD;
        }

        // set terminal size
        //Console.Write("\e[8;25;80t");

        // main app loop
        while (true)
        {
            // clear & display banner
            Console.Clear();
            DisplayBanner();

            // draw menu
            MenuItemCollection menu = new MenuItemCollection
            {
                new MenuItem((int)ID_SELECT_URL, "Media", ActionText(MediaItem.Current.Address, "[select]")),
                new MenuItem((int)ID_SELECT_MODE, "Mode", ActionText(MediaItem.Current.Mode)),
                new MenuItem((int)ID_SELECT_QUALITY, "Quality", ActionText(MediaItem.Current.Quality)),
                new MenuItem((int)ID_SELECT_FORMAT, "Format", ActionText(MediaItem.Current.Format)),
                new MenuItem((int)ID_SELECT_LOCATION, "Location", ActionText(MediaItem.Current.Location)),
                new MenuItem(),
                new MenuItem((int)ID_EXTRAS, "Extras", ">"),
                new MenuItem(),
                new MenuItem((int)ID_START_DOWNLOAD, "Verify & Download"),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Exit", "ESC")
            };

            // get selected menu item
            int option = ConsoleMenu.SelectMenu(menu);

            switch (option)
            {
                // exit the application
                case -1:    // ESC key pressed
                case (int)ID_EXIT:
                    goto AppExit;

                // select media URL
                case (int)ID_SELECT_URL:
                    {
                        Console.Clear();
                        MenuActions.SelectMediaSource();
                    }
                    break;

                // select mode: audio-only, video-only, both
                case (int)ID_SELECT_MODE:
                    break;

                // select download quality
                case (int)ID_SELECT_QUALITY:
                    break;

                // select output format: MP4, MP3, etc
                case (int)ID_SELECT_FORMAT:
                    {
                        Console.Clear();
                        if (MenuActions.SelectFormat() == false)
                        {
                            Console.WriteLine("Unable to get available formats.");
                            Terminal.Pause();
                        }
                    }
                    break;

                // select output directory
                case (int)ID_SELECT_LOCATION:
                    {
                        Console.Clear();
                        MenuActions.SelectLocation();
                    }
                    break;

                // open extras menu
                case (int)ID_EXTRAS:
                    {
                        Console.Clear();
                        MenuActions.OpenExtras();
                    }
                    break;

                // start the download
                case (int)ID_START_DOWNLOAD:
                    {
                        Console.Clear();
                        HandleAction(MenuActions.StartDownload(),
                            trueMessage: $"The {Terminal.AccentTextStyle}download{ANSI_RESET} has {Terminal.AccentTextStyle}started{ANSI_RESET}.",
                            falseMessage: $"{Terminal.AccentTextStyle}Unable to start{ANSI_RESET} download.");
                    }
                    break;

                // undefined option
                default:
                    break;
            }
        }

        // app exit
        AppExit:
            PostExitCleanup();
            return 0;
    }
}
