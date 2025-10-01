using System;
using System.Diagnostics;
using System.IO;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

using static Mediavax.MenuIds;
using static Toolbox.ANSI;

namespace Mediavax;

/// <summary>
/// Representing Mediavax program's menu actions.
/// </summary>
internal static class MenuActions
{
    public static bool SelectMediaSource()
    {
        string address = Terminal.Input($"Enter a media source ({Terminal.AccentTextStyle}absolute URL{ANSI.ANSI_RESET}). Leave blank to cancel.\n# ", false);
        if (string.IsNullOrWhiteSpace(address))
        {
            Log.Information("Media selection cancelled.", nameof(SelectMediaSource));
            return false;
        }

        // check input
        if (Uri.TryCreate(address, UriKind.Absolute, out Uri? uri) == true)
        {
            MediaItem.Current.Address = uri.AbsoluteUri;
            Log.Success("Selected media updated.", nameof(SelectMediaSource));
            return true;
        }

        else
        {
            Log.Error("Invalid URI format.", nameof(SelectMediaSource));
            return false;
        }
    }

    public static bool SelectMode()
    {
        return true;
    }

    public static bool SelectMediaQuality()
    {
        return true;
    }

    public static bool SelectFormat()
    {
        if (string.IsNullOrEmpty(MediaItem.Current.Address) == true)
        {
            Log.Warning("Select a media source first.", nameof(SelectFormat));
            return false;
        }

        if (File.Exists("yt-dlp") == false)
        {
            Log.Error("yt-dlp was not found.", nameof(SelectFormat));
            return false;
        }

        string json = string.Empty;
        Console.Write("Gathering available formats... ");
        if (Toolbox.Core.CreateProcess("yt-dlp", $"--dump-json --no-warnings -F {MediaItem.Current.Address}", out Process? proc, false, string.Empty) == true)
        {
            proc.WaitForExit();
            Console.WriteLine();
            json = proc.StandardOutput.ReadToEnd().Trim();

            Console.WriteLine("FORMATS: ");
            Console.WriteLine(json);
            return true;
        }

        else
        {
            Log.Error("Unable to get available formats.", nameof(SelectFormat));
            return false;
        }
    }

    public static bool SelectLocation()
    {
        string location = Terminal.Input($"Enter the {Terminal.AccentTextStyle}output directory{ANSI.ANSI_RESET}. Leave blank to cancel.\n# ", false);
        if (string.IsNullOrWhiteSpace(location))
        {
            Log.Information("Location selection cancelled.", nameof(SelectLocation));
            return false;
        }

        if (Directory.Exists(location) == false)
        {
            Log.Warning($"Directory not found. Action cancelled.", nameof(SelectLocation));
            return false;
        }

        MediaItem.Current.Location = location;
        Log.Success("Location updated.", nameof(SelectLocation));
        return true;
    }

    public static bool OpenExtras()
    {
        // extras collection
        while (true)
        {
            Console.Clear();
            MenuItemCollection items = new MenuItemCollection
            {
                new MenuItem((int)ID_IMPORT_COOKIES, "Import cookies", MediaItem.Current.BrowserCookies == string.Empty ? "None" : MediaItem.Current.BrowserCookies),
                new MenuItem((int)ID_CUSTOM_ARGUMENTS, "Cutsom YT-DLP arguments", MediaItem.Current.CustomOptions == string.Empty ? "None" : "[values]"),
                new MenuItem(),
                new MenuItem((int)ID_YT_DLP_VERSION, "YT-DLP Version", Program.ActionText(Program._ytdlp_version, "Unknown")),
                new MenuItem((int)ID_UPDATE_YTDLP, "Check for Updates", ">"),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Back", "ESC")
            };

            int option = ConsoleMenu.SelectMenu(items);
            switch (option)
            {
                case (int)ID_EXIT:
                case -1:
                    goto MethodExit;

                case (int)ID_IMPORT_COOKIES:
                    {
                        SelectBrowser();
                    }
                    break;

                case (int)ID_CUSTOM_ARGUMENTS:
                    {
                        GetCustomArguments();
                    }
                    break;

                case (int)ID_UPDATE_YTDLP:
                    {
                        UpdateDownloader();
                    }
                    break;

                default:
                    break;
            }
        }

    MethodExit:
        return true;
    }

    public static bool SelectBrowser()
    {
        while (true)
        {
            Console.Clear();
            MenuItemCollection browsers = new MenuItemCollection
            {
                new MenuItem((int)ID_BROWSER_NONE, "None", MediaItem.Current.BrowserCookies == string.Empty ? "\u2713" : string.Empty),
                new MenuItem(),
                new MenuItem((int)ID_BROWSER_BRAVE, "Brave", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_BRAVE ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_CHROME, "Google Chrome", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_CHROME ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_CHROMIUM, "Chromium", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_CHROMIUM ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_EDGE, "Microsoft Edge", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_EDGE ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_OPERA, "Opera", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_OPERA ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_VIVALDI, "Vivaldi", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_VIVALDI ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_FIREFOX, "Mozilla Firefox", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_FIREFOX ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_SAFARI, "Apple Safari", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_SAFARI ? "\u2713" : string.Empty),
                new MenuItem((int)ID_BROWSER_WHALE, "Whale", MediaItem.Current.BrowserCookies == MediaItem.BROWSER_WHALE ? "\u2713" : string.Empty),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Back", "ESC")
            };

            int option = ConsoleMenu.SelectMenu(browsers);
            switch (option)
            {
                case -1:
                case (int)ID_EXIT:
                    goto MethodExit;

                // no browser
                case (int)ID_BROWSER_NONE:
                    MediaItem.Current.BrowserCookies = string.Empty;
                    break;

                // brave browser
                case (int)ID_BROWSER_BRAVE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_BRAVE;
                    break;

                // chrome browser
                case (int)ID_BROWSER_CHROME:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_CHROME;
                    break;

                // chromium browser
                case (int)ID_BROWSER_CHROMIUM:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_CHROMIUM;
                    break;

                // edge browser
                case (int)ID_BROWSER_EDGE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_EDGE;
                    break;

                // opera browser
                case (int)ID_BROWSER_OPERA:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_OPERA;
                    break;

                // vivaldi browser
                case (int)ID_BROWSER_VIVALDI:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_VIVALDI;
                    break;

                // firefox browser
                case (int)ID_BROWSER_FIREFOX:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_FIREFOX;
                    break;

                // safari browser
                case (int)ID_BROWSER_SAFARI:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_SAFARI;
                    break;

                // whale browser
                case (int)ID_BROWSER_WHALE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_WHALE;
                    break;

                default:
                    break;
            }
        }

        // method exit
        MethodExit:
            return true;
    }

    public static bool GetCustomArguments()
    {
        Console.Clear();
        string args = Terminal.Input($"Enter {Terminal.AccentTextStyle}additional arguments{ANSI_RESET}\nCurrent value: {Terminal.AccentTextStyle}{MediaItem.Current.CustomOptions}{ANSI_RESET}\n\nNew value\n# ", false);

        if (string.IsNullOrWhiteSpace(args))
        {
            return false;
        }

        MediaItem.Current.CustomOptions = args;
        return true;
    }

    public static bool StartDownload()
    {
        // check if the download can start
        if (MediaItem.Current.IsValid() == false)
        {
            // can't download, not all required fields are filled with data
            Log.Error("Insufficient parameters. Fill all required fields.", nameof(StartDownload));
            return false;
        }

        // confirm selected before proceeding to the download
        Console.Clear();

        Console.WriteLine($"{Terminal.AccentHighlightStyle} SUMMARY {ANSI_RESET}");
        Console.WriteLine($"\tSource:   {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Address)}{ANSI_RESET}");
        Console.WriteLine($"\tMode:     {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Mode)}{ANSI_RESET}");
        Console.WriteLine($"\tQuality:  {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Quality)}{ANSI_RESET}");
        Console.WriteLine($"\tFormat:   {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Format)}{ANSI_RESET}");
        Console.WriteLine($"\tLocation: {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Location)}{ANSI_RESET}");
        Console.WriteLine($"\tCookies:  {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.BrowserCookies, "None")}{ANSI_RESET}");
        Terminal.Pause();

        return true;
    }

    public static bool UpdateDownloader()
    {
        Console.Clear();
        if (Toolbox.Core.CreateProcess("yt-dlp", "--update", out Process? process) == true)
        {
            Console.WriteLine("Update process started\n");
            process?.WaitForExit();
            
            if (process.ExitCode == 0)
            {
                Log.Success("YT-DLP was updated.", nameof(UpdateDownloader));
                return true;
            }

            else
            {
                Terminal.Pause($"\nUpdate process {Terminal.AccentTextStyle}failed{ANSI_RESET} with exit code {Terminal.AccentTextStyle}{process.ExitCode}{ANSI_RESET}.\nPress {Terminal.AccentTextStyle}enter{ANSI_RESET} to continue. . . ");
                return false;
            }
        }

        // can't start process
        return false;
    }
}
