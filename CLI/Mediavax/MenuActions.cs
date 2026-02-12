using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

namespace Mediavax;

/// <summary>
///     Representing Mediavax program's menu actions.
/// </summary>
internal static class MenuActions
{
    private static bool _downloadVerified;

    public static bool SelectMediaSource()
    {
        string address =
            Terminal.Input(
                prompt:
                $"Enter a media source ({Terminal.AccentTextStyle}absolute URL{ANSI.ANSI_RESET}). Leave blank to cancel.\n# ",
                ensureValue: false);
        if (string.IsNullOrWhiteSpace(value: address))
        {
            _ = Log.Information(message: "Media selection cancelled.", tag: nameof(MenuActions.SelectMediaSource));
            return false;
        }

        // check input
        if (Uri.TryCreate(uriString: address, uriKind: UriKind.Absolute, result: out Uri? uri))
        {
            MediaItem.Current.Address = uri.AbsoluteUri;
            _ = Log.Success(message: "Selected media updated.", tag: nameof(MenuActions.SelectMediaSource));
            return true;
        }

        _ = Log.Error(message: "Invalid URI format.", tag: nameof(MenuActions.SelectMediaSource));
        return false;
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
        MenuItemCollection items = new();
        foreach (KeyValuePair<int, YtDlpFormat> kp in formats)
        {
            int id = kp.Key;
            YtDlpFormat format = kp.Value;

            // combined
            if (format.HasVideo && format.HasAudio)
                items.Add(item: new MenuItem(id: id,
                    text: $"{(format.height + "p").PadRight(totalWidth: 15)}{format.ext}", alt: format.format_id));

            // audio only
            else if (!format.HasVideo && format.HasVideo)
                items.Add(item: new MenuItem(id: id, text: $"{format.acodec.PadRight(totalWidth: 15)}{format.ext}",
                    alt: format.format_id));

            // video only
            else if (format.HasVideo && !format.HasVideo)
                items.Add(item: new MenuItem(id: id, text: $"{format.vcodec.PadRight(totalWidth: 15)}{format.ext}",
                    alt: format.format_id));

            // extras
            else
                items.Add(item: new MenuItem(id: id, text: $"{format.format_note.PadRight(totalWidth: 15)}{format.ext}",
                    alt: format.format_id));
        }

        Console.Clear();
        int option = ConsoleMenu.SelectMenu(items: items);
        if (option == 0 || option == -1)
        {
            // selection cancelled
            _ = Log.Information(message: "Format selection cancelled", tag: nameof(MenuActions.SelectFormat));
            return false;
        }

        {
            // selected format
            string format = formats[key: option].format_id;
            MediaItem.Current.Format = format;
            _ = Log.Success(message: $"New format selected: {format}", tag: nameof(MenuActions.SelectFormat));
            return true;
        }
    }

    public static bool ListFormats()
    {
        if (string.IsNullOrEmpty(value: MediaItem.Current.Address))
        {
            _ = Log.Warning(message: "Select a media source first.", tag: nameof(MenuActions.SelectFormat));
            return false;
        }

        (bool bFile, bool bPath) = Program.DownloaderExists();
        if (!bFile && !bPath)
        {
            _ = Log.Warning(message: "YT-DLP was not found.");
            return false;
        }

        string path = Program.GetDownloader();
        string args = $"--dump-json --no-warnings {MediaItem.Current.Address}";

        try
        {
            ProcessStartInfo psi = new()
            {
                FileName = path,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process? proc = Process.Start(startInfo: psi);
            if (proc == null)
            {
                _ = Log.Error(message: "Process object is null.", tag: nameof(MenuActions.SelectFormat));
                return false;
            }

            Console.Write(value: "Gathering formats info... ");

            // Read output BEFORE waiting
            string json = proc.StandardOutput.ReadToEnd();
            string err = proc.StandardError.ReadToEnd();

            proc.WaitForExit();
            Console.WriteLine(value: "\n");

            if (!err.IsEmpty())
            {
                Console.WriteLine(value: $"{Terminal.AccentTextStyle}yt-dlp error{ANSI.ANSI_RESET}: " + err);
                return false;
            }

            // parse formats
            if (YtDlpParser.GetInfo(json: json, info: out YtDlpInfo info))
            {
                int idx = 0x10;
                Dictionary<int, YtDlpFormat> combined = [];
                Dictionary<int, YtDlpFormat> audioOnly = [];
                Dictionary<int, YtDlpFormat> videoOnly = [];
                Dictionary<int, YtDlpFormat> extras = [];

                // representing combined formats
                foreach (YtDlpFormat format in info.formats.Where(predicate: x => x.HasVideo && x.HasAudio))
                    combined.Add(key: idx++, value: format);

                // audio only
                foreach (YtDlpFormat format in info.formats.Where(predicate: x => !x.HasVideo && x.HasAudio))
                    audioOnly.Add(key: idx++, value: format);

                // video only
                foreach (YtDlpFormat format in info.formats.Where(predicate: x => x.HasVideo && !x.HasAudio))
                    videoOnly.Add(key: idx++, value: format);

                // extras, thumbnails, etc.
                foreach (YtDlpFormat format in info.formats.Where(predicate: x => !x.HasVideo && !x.HasAudio))
                    extras.Add(key: idx++, value: format);

                // display available categories
                MenuItemCollection categoriesMenu = new();

                // combined
                if (combined.Count > 0)
                    categoriesMenu.Add(
                        item: new MenuItem(id: (int)MenuIds.ID_FORMATS_COMBINED, text: "Combined",
                            alt: combined.Count.ToString()));

                // audio only
                if (audioOnly.Count > 0)
                    categoriesMenu.Add(
                        item: new MenuItem(id: (int)MenuIds.ID_FORMATS_AUDIO, text: "Audio Only",
                            alt: audioOnly.Count.ToString()));

                // video only
                if (videoOnly.Count > 0)
                    categoriesMenu.Add(
                        item: new MenuItem(id: (int)MenuIds.ID_FORMATS_VIDEO, text: "Video Only",
                            alt: videoOnly.Count.ToString()));

                // extras
                if (extras.Count > 0)
                    categoriesMenu.Add(item: new MenuItem(id: (int)MenuIds.ID_FORMATS_EXTRA, text: "Extras",
                        alt: extras.Count.ToString()));

                Console.Clear();
                int category = ConsoleMenu.SelectMenu(items: categoriesMenu);
                return category switch
                {
                    0 or -1 => false,
                    (int)MenuIds.ID_FORMATS_COMBINED => MenuActions.SelectFormat(formats: combined),
                    (int)MenuIds.ID_FORMATS_AUDIO => MenuActions.SelectFormat(formats: audioOnly),
                    (int)MenuIds.ID_FORMATS_VIDEO => MenuActions.SelectFormat(formats: videoOnly),
                    (int)MenuIds.ID_FORMATS_EXTRA => MenuActions.SelectFormat(formats: extras),
                    var _ => true
                };
            }

            _ = Log.Error(message: "Unable to parse formats.", tag: nameof(MenuActions.SelectFormat));
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(value: $"{Log.TypeNamesFormatted[key: Log.LogType.Error]} {ex.Message}");
            _ = Log.Error(message: $"Exception: {ex.Message}", tag: nameof(MenuActions.SelectFormat));
            return false;
        }
    }

    public static bool SelectLocation()
    {
        string location =
            Terminal.Input(
                prompt:
                $"Enter the {Terminal.AccentTextStyle}output directory{ANSI.ANSI_RESET}. Leave blank to cancel.\n# ",
                ensureValue: false);
        if (string.IsNullOrWhiteSpace(value: location))
        {
            _ = Log.Information(message: "Location selection cancelled.", tag: nameof(MenuActions.SelectLocation));
            return false;
        }

        if (!Directory.Exists(path: location))
        {
            _ = Log.Warning(message: "Directory not found. Action cancelled.", tag: nameof(MenuActions.SelectLocation));
            return false;
        }

        MediaItem.Current.Location = location;
        _ = Log.Success(message: "Location updated.", tag: nameof(MenuActions.SelectLocation));
        return true;
    }

    public static bool OpenExtras()
    {
        // extras collection
        while (true)
        {
            Console.Clear();
            MenuItemCollection items =
            [
                new MenuItem(id: (int)MenuIds.ID_IMPORT_COOKIES, text: "Import cookies",
                    alt: MediaItem.Current.BrowserCookies == string.Empty ? "None" : MediaItem.Current.BrowserCookies),

                new MenuItem(id: (int)MenuIds.ID_CUSTOM_ARGUMENTS, text: "Cutsom YT-DLP arguments",
                    alt: MediaItem.Current.CustomOptions == string.Empty ? "None" : "[values]"),

                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_YT_DLP_VERSION, text: "YT-DLP Version",
                    alt: Program.ActionText(value: Program._ytdlp_version, alt: "Unknown")),

                new MenuItem(id: (int)MenuIds.ID_UPDATE_YTDLP, text: "Check for Updates", alt: ">"),
                new MenuItem(id: (int)MenuIds.ID_SELECT_THEME, text: "Select Theme", alt: ""),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Back", alt: "ESC")
            ];

            int option = ConsoleMenu.SelectMenu(items: items);
            switch (option)
            {
                case (int)MenuIds.ID_EXIT:
                case -1:
                    goto MethodExit;

                case (int)MenuIds.ID_IMPORT_COOKIES:
                {
                    _ = MenuActions.SelectBrowser();
                }
                    break;

                case (int)MenuIds.ID_CUSTOM_ARGUMENTS:
                {
                    _ = MenuActions.GetCustomArguments();
                }
                    break;

                case (int)MenuIds.ID_UPDATE_YTDLP:
                {
                    _ = MenuActions.UpdateDownloader();
                }
                    break;

                case (int)MenuIds.ID_SELECT_THEME:
                {
                    _ = MenuActions.SelectTheme();
                }
                    break;
            }
        }

        MethodExit:
        return true;
    }

    public static bool SelectTheme()
    {
        MenuItemCollection themeMenu = new();


        do
        {
            themeMenu.Clear();
            for (int x = 1; x <= Program.ThemeOptions.Count; x++)
            {
                int realIndex = x - 1;
                KeyValuePair<string, (string, string)> kp = Program.ThemeOptions.ElementAt(index: realIndex);
                themeMenu.Add(item: new MenuItem(id: x, text: kp.Key));
            }

            themeMenu.Add(item: new MenuItem());
            themeMenu.Add(item: new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Back", alt: "ESC"));

            Console.Clear();
            int option = ConsoleMenu.SelectMenu(items: themeMenu);
            switch (option)
            {
                case (int)MenuIds.ID_EXIT:
                case ConsoleMenu.KEY_ESCAPE:
                    goto MethodReturn;

                default:
                    Terminal.AccentTextStyle = Program.ThemeOptions.ElementAt(index: option - 1).Value.Item1;
                    Terminal.AccentHighlightStyle = Program.ThemeOptions.ElementAt(index: option - 1).Value.Item2;
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
            MenuItemCollection browsers = new()
            {
                new MenuItem(id: (int)MenuIds.ID_BROWSER_NONE, text: "None",
                    alt: MediaItem.Current.BrowserCookies == string.Empty ? "\u2713" : string.Empty),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_BRAVE, text: "Brave",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_BRAVE ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_CHROME, text: "Google Chrome",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_CHROME ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_CHROMIUM, text: "Chromium",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_CHROMIUM ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_EDGE, text: "Microsoft Edge",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_EDGE ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_OPERA, text: "Opera",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_OPERA ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_VIVALDI, text: "Vivaldi",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_VIVALDI ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_FIREFOX, text: "Mozilla Firefox",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_FIREFOX ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_SAFARI, text: "Apple Safari",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_SAFARI ? "\u2713" : string.Empty),
                new MenuItem(id: (int)MenuIds.ID_BROWSER_WHALE, text: "Whale",
                    alt: MediaItem.Current.BrowserCookies == MediaItem.BROWSER_WHALE ? "\u2713" : string.Empty),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Back", alt: "ESC")
            };

            int option = ConsoleMenu.SelectMenu(items: browsers);
            switch (option)
            {
                case -1:
                case (int)MenuIds.ID_EXIT:
                    goto MethodExit;

                // no browser
                case (int)MenuIds.ID_BROWSER_NONE:
                    MediaItem.Current.BrowserCookies = string.Empty;
                    break;

                // brave browser
                case (int)MenuIds.ID_BROWSER_BRAVE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_BRAVE;
                    break;

                // chrome browser
                case (int)MenuIds.ID_BROWSER_CHROME:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_CHROME;
                    break;

                // chromium browser
                case (int)MenuIds.ID_BROWSER_CHROMIUM:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_CHROMIUM;
                    break;

                // edge browser
                case (int)MenuIds.ID_BROWSER_EDGE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_EDGE;
                    break;

                // opera browser
                case (int)MenuIds.ID_BROWSER_OPERA:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_OPERA;
                    break;

                // vivaldi browser
                case (int)MenuIds.ID_BROWSER_VIVALDI:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_VIVALDI;
                    break;

                // firefox browser
                case (int)MenuIds.ID_BROWSER_FIREFOX:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_FIREFOX;
                    break;

                // safari browser
                case (int)MenuIds.ID_BROWSER_SAFARI:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_SAFARI;
                    break;

                // whale browser
                case (int)MenuIds.ID_BROWSER_WHALE:
                    MediaItem.Current.BrowserCookies = MediaItem.BROWSER_WHALE;
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
        string args = Terminal.Input(
            prompt:
            $"Enter {Terminal.AccentTextStyle}additional arguments{ANSI.ANSI_RESET}\nCurrent value: {Terminal.AccentTextStyle}{MediaItem.Current.CustomOptions}{ANSI.ANSI_RESET}\n\nNew value\n# ",
            ensureValue: false);

        if (string.IsNullOrWhiteSpace(value: args)) return false;

        MediaItem.Current.CustomOptions = args;
        return true;
    }

    public static bool VerifyDownload()
    {
        // check if the download can start
        if (!MediaItem.Current.IsValid())
        {
            // can't download, not all required fields are filled with data
            _ = Log.Error(message: "Insufficient parameters. Fill all required fields.",
                tag: nameof(MenuActions.VerifyDownload));
            return false;
        }

        // confirm selected before proceeding to the download
        Console.Clear();

        Console.WriteLine(value: $"{Terminal.AccentHighlightStyle} SUMMARY {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Source:   {Terminal.AccentTextStyle}{Program.ActionText(value: MediaItem.Current.Address)}{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Format:   {Terminal.AccentTextStyle}{Program.ActionText(value: MediaItem.Current.Format)}{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Location: {Terminal.AccentTextStyle}{Program.ActionText(value: MediaItem.Current.Location)}{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Cookies:  {Terminal.AccentTextStyle}{Program.ActionText(value: MediaItem.Current.BrowserCookies, alt: "None")}{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Extras:   {Terminal.AccentTextStyle}{Program.ActionText(value: MediaItem.Current.CustomOptions, alt: "None")}{ANSI.ANSI_RESET}");
        Console.WriteLine();

        // confirm to start
        if (string.Equals(a: Terminal.Input(prompt: "Do you want to start the download? [Y/n]: "), b: "y",
                comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            // verify download
            MenuActions._downloadVerified = true;
            _ = Log.Success(message: "Download verified.", tag: nameof(MenuActions.VerifyDownload));
            return true;
        }

        // download cancelled
        MenuActions._downloadVerified = false;
        return false;
    }

    public static bool StartDownload()
    {
        if (!MenuActions._downloadVerified)
        {
            // download was not confirmed
            _ = Log.Warning(message: "Download was not confirmed.", tag: nameof(MenuActions.StartDownload));
            return false;
        }

        if (Toolbox.Core.CreateProcess(command: Program.YT_DLP, arguments: MediaItem.Current.BuildCommand(),
                process: out Process? proc))
        {
            proc.WaitForExit();
            return proc.ExitCode == 0 ? true : false;
        }

        _ = Log.Error(message: "Can't start the download process.", tag: nameof(MenuActions.StartDownload));
        return false;
    }

    public static bool UpdateDownloader()
    {
        Console.Clear();
        if (Toolbox.Core.CreateProcess(command: Program.YT_DLP, arguments: "--update", process: out Process? process))
        {
            if (process == null)
            {
                _ = Log.Error(message: "Unable to start a new process.");
                return false;
            }

            Console.WriteLine(value: "Update process started\n");
            process?.WaitForExit();

            if (process?.ExitCode == 0)
            {
                _ = Log.Success(message: "YT-DLP was updated.", tag: nameof(MenuActions.UpdateDownloader));
                return true;
            }

            Terminal.Pause(
                message:
                $"\nUpdate process {Terminal.AccentTextStyle}failed{ANSI.ANSI_RESET} with exit code {Terminal.AccentTextStyle}{process.ExitCode}{ANSI.ANSI_RESET}.\nPress {Terminal.AccentTextStyle}enter{ANSI.ANSI_RESET} to continue. . . ");
            return false;
        }

        // can't start process
        return false;
    }
}