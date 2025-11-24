using Mediavax.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

    public static bool SelectFormat(Dictionary<int, YtDlpFormat> formats)
    {
        // Key: ID
        // Value: Associated format
        MenuItemCollection items = new MenuItemCollection();
        foreach (var kp in formats)
        {
            int id = kp.Key;
            YtDlpFormat format = kp.Value;

            // combined
            if (format.HasVideo == true && format.HasAudio == true) items.Add(new MenuItem(id, $"{(format.height + "p").PadRight(15)}{format.ext}", format.format_id));

            // audio only
            else if (format.HasVideo == false && format.HasVideo == true) items.Add(new MenuItem(id, $"{format.acodec.PadRight(15)}{format.ext}", format.format_id));

            // video only
            else if (format.HasVideo == true && format.HasVideo == false) items.Add(new MenuItem(id, $"{format.vcodec.PadRight(15)}{format.ext}", format.format_id));

            // extras
            else items.Add(new MenuItem(id, $"{format.format_note.PadRight(15)}{format.ext}", format.format_id));
        }

        Console.Clear();
        int option = ConsoleMenu.SelectMenu(items);
        if (option == 0 || option == -1)
        {
            // selection cancelled
            Log.Information("Format selection cancelled", nameof(SelectFormat));
            return false;
        }

        else
        {
            // selected format
            string format = formats[option].format_id;
            MediaItem.Current.Format = format;
            Log.Success($"New format selected: {format}", nameof(SelectFormat));
            return true;
        }
    }

    public static bool ListFormats()
    {
        if (string.IsNullOrEmpty(MediaItem.Current.Address))
        {
            Log.Warning("Select a media source first.", nameof(SelectFormat));
            return false;
        }

        (bool bFile, bool bPath) = Program.DownloaderExists();
        if (!bFile && !bPath)
        {
            Log.Warning("YT-DLP was not found.");
            return false;
        }

        string path = Program.GetDownloader();
        string args = $"--dump-json --no-warnings {MediaItem.Current.Address}";

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var proc = Process.Start(psi))
            {
                if (proc == null)
                {
                    Log.Error("Process object is null.", nameof(SelectFormat));
                    return false;
                }

                Console.Write("Gathering formats info... ");

                // Read output BEFORE waiting
                string json = proc.StandardOutput.ReadToEnd();
                string err = proc.StandardError.ReadToEnd();

                proc.WaitForExit();
                Console.WriteLine("\n");

                if (err.IsEmpty() == false)
                {
                    Console.WriteLine($"{Terminal.AccentTextStyle}yt-dlp error{ANSI_RESET}: " + err);
                    return false;
                }

                // parse formats
                if (YtDlpParser.GetInfo(json, out YtDlpInfo info) == true)
                {
                    // TODO: JSON parsed, enum audio/video only and combined formats and offer them to the user
                    int idx = 0x10;
                    Dictionary<int, YtDlpFormat> combined = [];
                    Dictionary<int, YtDlpFormat> audioOnly = [];
                    Dictionary<int, YtDlpFormat> videoOnly = [];
                    Dictionary<int, YtDlpFormat> extras = [];

                    // representing combined formats
                    foreach (YtDlpFormat format in info.formats.Where(x => x.HasVideo == true && x.HasAudio == true))
                    {
                        combined.Add(idx++, format);
                    }

                    // audio only
                    foreach (YtDlpFormat format in info.formats.Where(x => x.HasVideo == false && x.HasAudio))
                    {
                        audioOnly.Add(idx++, format);
                    }

                    // video only
                    foreach (YtDlpFormat format in info.formats.Where(x => x.HasVideo && x.HasAudio == false))
                    {
                        videoOnly.Add(idx++, format);
                    }

                    // extras, thumbnails, etc.
                    foreach (YtDlpFormat format in info.formats.Where(x => x.HasVideo == false && x.HasAudio == false))
                    {
                        extras.Add(idx++, format);
                    }

                    // display available categories
                    MenuItemCollection categoriesMenu = new MenuItemCollection();

                    // combined
                    if (combined.Count > 0) categoriesMenu.Add(new MenuItem((int)ID_FORMATS_COMBINED, "Combined", combined.Count.ToString()));

                    // audio only
                    if (audioOnly.Count > 0) categoriesMenu.Add(new MenuItem((int)ID_FORMATS_AUDIO, "Audio Only", audioOnly.Count.ToString()));

                    // video only
                    if (videoOnly.Count > 0) categoriesMenu.Add(new MenuItem((int)ID_FORMATS_VIDEO, "Video Only", videoOnly.Count.ToString()));

                    // extras
                    if (extras.Count > 0) categoriesMenu.Add(new MenuItem((int)ID_FORMATS_EXTRA, "Extras", extras.Count.ToString()));

                    Console.Clear();
                    int category = ConsoleMenu.SelectMenu(categoriesMenu);
                    switch (category)
                    {
                        case 0:
                        case -1:
                            return false;

                        case (int)ID_FORMATS_COMBINED:
                            {
                                // TODO: list combined formats
                                return SelectFormat(combined);
                            }

                        case (int)ID_FORMATS_AUDIO:
                            {
                                // TODO: list audio formats
                                return SelectFormat(audioOnly);
                            }

                        case (int)ID_FORMATS_VIDEO:
                            {
                                // TODO: list video formats
                                return SelectFormat(videoOnly);
                            }

                        case (int)ID_FORMATS_EXTRA:
                            {
                                // TODO: list extra formats
                                return SelectFormat(extras);
                            }

                        default: break;
                    }

                    return true;
                }

                else
                {
                    Log.Error("Unable to parse formats.", nameof(SelectFormat));
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{Log.TypeNamesFormatted[Log.LogType.Error]} {ex.Message}");
            Log.Error($"Exception: {ex.Message}", nameof(SelectFormat));
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
                new MenuItem((int)ID_SELECT_THEME, "Select Theme", ""),
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

                case (int)ID_SELECT_THEME:
                    {
                        SelectTheme();
                    }
                    break;

                default:
                    break;
            }
        }

    MethodExit:
        return true;
    }

    public static bool SelectTheme()
    {
        MenuItemCollection themeMenu = new MenuItemCollection();



        do
        {
            themeMenu.Clear();
            for (int x = 1; x <= Program.ThemeOptions.Count; x++)
            {
                int realIndex = x - 1;
                var kp = Program.ThemeOptions.ElementAt(realIndex);
                themeMenu.Add(new MenuItem(x, kp.Key));
            }

            themeMenu.Add(new MenuItem());
            themeMenu.Add(new MenuItem((int)ID_EXIT, "Back", "ESC"));

            Console.Clear();
            int option = ConsoleMenu.SelectMenu(themeMenu);
            switch (option)
            {
                case (int)ID_EXIT:
                case ConsoleMenu.KEY_ESCAPE:
                    goto MethodReturn;

                default:
                    Terminal.AccentTextStyle = Program.ThemeOptions.ElementAt(option - 1).Value.Item1;
                    Terminal.AccentHighlightStyle = Program.ThemeOptions.ElementAt(option - 1).Value.Item2;
                    break;
            }
        } while (true);

    MethodReturn:
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

    private static bool _downloadVerified = false;
    public static bool VerifyDownload()
    {
        // check if the download can start
        if (MediaItem.Current.IsValid() == false)
        {
            // can't download, not all required fields are filled with data
            Log.Error("Insufficient parameters. Fill all required fields.", nameof(VerifyDownload));
            return false;
        }

        // confirm selected before proceeding to the download
        Console.Clear();

        Console.WriteLine($"{Terminal.AccentHighlightStyle} SUMMARY {ANSI_RESET}");
        Console.WriteLine($"Source:   {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Address)}{ANSI_RESET}");
        Console.WriteLine($"Format:   {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Format)}{ANSI_RESET}");
        Console.WriteLine($"Location: {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.Location)}{ANSI_RESET}");
        Console.WriteLine($"Cookies:  {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.BrowserCookies, "None")}{ANSI_RESET}");
        Console.WriteLine($"Extras:   {Terminal.AccentTextStyle}{Program.ActionText(MediaItem.Current.CustomOptions, "None")}{ANSI_RESET}");
        Console.WriteLine();

        // confirm to start
        if (string.Equals(Terminal.Input("Do you want to start the download? [Y/n]: ", true), "y", StringComparison.OrdinalIgnoreCase) == true)
        {
            // verify download
            _downloadVerified = true;
            Log.Success("Download verified.", nameof(VerifyDownload));
            return true;
        }

        else
        {
            // download cancelled
            _downloadVerified = false;
            return false;
        }
    }

    public static bool StartDownload()
    {
        if (_downloadVerified == false)
        {
            // download was not confirmed
            Log.Warning("Download was not confirmed.", nameof(StartDownload));
            return false;
        }

        if (Toolbox.Core.CreateProcess(Program.YT_DLP, MediaItem.Current.BuildCommand(), out Process? proc))
        {
            proc.WaitForExit();
            return proc.ExitCode == 0 ? true : false;
        }

        else
        {
            Log.Error("Can't start the download process.", nameof(StartDownload));
            return false;
        }
    }

    public static bool UpdateDownloader()
    {
        Console.Clear();
        if (Toolbox.Core.CreateProcess(Program.YT_DLP, "--update", out Process? process) == true)
        {
            if (process == null)
            {
                Log.Error("Unable to start a new process.");
                return false;
            }

            Console.WriteLine("Update process started\n");
            process?.WaitForExit();

            if (process?.ExitCode == 0)
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
