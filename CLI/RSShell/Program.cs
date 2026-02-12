using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RSShell.Core;
using Toolbox;
using Toolbox.UI;

namespace RSShell;

/// <summary>
///     Representing the RSShell program class.
/// </summary>
internal class Program : IApplication
{
    private const int ID_EXIT = 0;
    private const int ID_LIST_FEEDS = 1;
    private const int ID_ADD_FEED = 2;
    private const int ID_ABOUT = 3;
    private const int ID_SEPARATOR = 0x1000;
    private const int ID_FETCH_FEEDS = 4;
    private const int ID_SELECT_FEED = 5;

    private static readonly List<RssChannel> _channels = [];

    /// <summary>
    ///     Interface implementation, unused.
    /// </summary>
    public static Version Version => new();

    public static void PostExitCleanup()
    {
        // TODO: cleanup code
        _ = Config.Save(config: Config.Current);
        Console.Clear();
    }

    public static void DisplayBanner()
    {
        // Displays the ANSI colored banner
        Console.Write(value: "\e[38;5;21m");
        Console.WriteLine(value: "██████╗ ███████╗███████╗██╗  ██╗███████╗██╗     ██╗     ");
        Console.Write(value: "\e[38;5;57m");
        Console.WriteLine(value: "██╔══██╗██╔════╝██╔════╝██║  ██║██╔════╝██║     ██║     ");
        Console.Write(value: "\e[38;5;93m");
        Console.WriteLine(value: "██████╔╝███████╗███████╗███████║█████╗  ██║     ██║     ");
        Console.Write(value: "\e[38;5;129m");
        Console.WriteLine(value: "██╔══██╗╚════██║╚════██║██╔══██║██╔══╝  ██║     ██║     ");
        Console.Write(value: "\e[38;5;165m");
        Console.WriteLine(value: "██║  ██║███████║███████║██║  ██║███████╗███████╗███████╗");
        Console.Write(value: "\e[38;5;201m");
        Console.Write(value: "╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝");
        Console.WriteLine(value: "\e[0m"); // reset
        Console.WriteLine(
            value:
            $"Terminal RSS reader                       by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
    }

    public static void LoadConfig()
    {
        // NOTE: implemented to satisfy IApplication interface, no real use
    }

    /// <summary>
    ///     Representing the main application method.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Application exit code.</returns>
    private static async Task<int> Main(string[] args)
    {
        // initialize workspace
        _ = Setup.Initialize();

        // setup colors
        Terminal.AccentTextStyle = "\e[38;5;225m";
        Terminal.AccentHighlightStyle = "\e[48;5;225m\e[38;5;0m";

        // load config
        if (Config.Load(config: out Config cfg))
            Config.Current = cfg;

        else
            Config.Current = new Config();

        if (args.Length > 0)
        {
            // TODO: arguments disabled
            Console.WriteLine(value: "Arguments are disabled.");
            return 1;
        }

        Console.Title = "RSShell - Terminal RSS Reader";

        while (true)
        {
            Console.Clear();
            Program.DisplayBanner();

            int option = Program.HandleMenu();

            switch (option)
            {
                case Program.ID_LIST_FEEDS:
                {
                    if (Program.ListFeeds()) Terminal.Pause();
                }
                    break;

                case Program.ID_ADD_FEED:
                {
                    if (Program.AddRssFeed())
                    {
                        Terminal.Pause(
                            message:
                            $"{Terminal.AccentTextStyle}RSS feed added.{ANSI.ANSI_RESET} Press enter to continue. . . ");
                    }

                    else
                    {
                        Terminal.Pause(
                            message:
                            $"{Terminal.AccentTextStyle}No feed was added.{ANSI.ANSI_RESET} Press enter to continue. . . ");
                        ;
                    }
                }
                    break;

                case Program.ID_ABOUT:
                {
                    if (Program.AboutDialog()) Terminal.Pause();
                }
                    break;

                case Program.ID_SEPARATOR:
                    break;

                case Program.ID_FETCH_FEEDS:
                {
                    if (await Program.FetchAllFeeds()) Terminal.Pause();
                }
                    break;

                case Program.ID_SELECT_FEED:
                {
                    // open feedview to select the feed to view
                    Console.Clear();
                    if (Program.SelectFeed(channel: out RssChannel channel))
                    {
                        // open feed view
                        Console.Clear();
                        _ = Program.OpenFeedView(channel: channel);
                        Terminal.Pause();
                    }
                }
                    break;

                // 'exit' option
                case Program.ID_EXIT:
                    goto AppExit;
            }
        }

        // cleanup code
        AppExit:
        Program.PostExitCleanup();
        return 0;
    }

    private static int HandleMenu()
    {
        MenuItemCollection menu = new()
        {
            new MenuItem(id: Program.ID_SELECT_FEED, text: "Open Feed"),
            new MenuItem(),
            new MenuItem(id: Program.ID_FETCH_FEEDS, text: "Fetch All Feeds"),
            new MenuItem(),
            new MenuItem(id: Program.ID_LIST_FEEDS, text: "List RSS Feeds"),
            new MenuItem(id: Program.ID_ADD_FEED, text: "Add a new RSS Feed"),
            new MenuItem(id: Program.ID_ABOUT, text: "About RSShell"),
            new MenuItem(),
            new MenuItem(id: Program.ID_EXIT, text: "Exit")
        };

        // get user input
        return ConsoleMenu.SelectMenu(items: menu);
    }

    private static bool ListFeeds()
    {
        // ensure config
        Config.Current ??= new Config();

        Console.Clear();

        if (Config.Current.Feeds.Count == 0)
        {
            Console.WriteLine(
                value:
                $"No RSS feeds found. Use the \'{Terminal.AccentTextStyle}Add a new RSS Feed{ANSI.ANSI_RESET}\' option to start.\n");
            return true;
        }

        // size of the longest URL in the feeds list
        int longest = Config.Current.Feeds.Max(selector: x => x.Length);

        Console.WriteLine(value: "ID\tURL Address");
        Console.WriteLine(value: $"--\t{new string(c: '\u2015', count: longest)}");
        for (int x = 0; x < Config.Current.Feeds.Count; x++)
            Console.WriteLine(value: $"{x + 1:D2}\t{Config.Current.Feeds[index: x]}");

        Console.WriteLine();
        return true;
    }

    private static bool AddRssFeed()
    {
        // ensure config
        Config.Current ??= new Config();

        Console.Clear();
        string addr = Terminal.Input(
            prompt: $"Enter {Terminal.AccentTextStyle}RSS feed source{ANSI.ANSI_RESET} (leave blank to cancel)\n# ",
            ensureValue: false);
        Console.WriteLine(value: ANSI.ANSI_RESET);

        if (addr.IsEmpty())
            // no feed added
            return false;

        // add feed
        Config.Current.Feeds.Add(item: addr);
        return true;
    }

    private static bool AboutDialog()
    {
        Console.Clear();

        Console.WriteLine(
            value:
            $"{Terminal.AccentTextStyle}About RSShell{ANSI.ANSI_RESET}\nSimple RSS reader inside your terminal!\n");
        return true;
    }

    private static async Task<bool> FetchAllFeeds()
    {
        Console.Clear();
        Program._channels.Clear();

        if (Config.Current.Feeds.Count == 0)
        {
            Console.WriteLine(
                value:
                $"No RSS feeds to fetch. Use the \'{Terminal.AccentTextStyle}Add a new RSS Feed{ANSI.ANSI_RESET}\' option to add new feeds.\n");
            return true;
        }

        foreach (string uri in Config.Current.Feeds)
            try
            {
                Uri url = new(uriString: uri, uriKind: UriKind.RelativeOrAbsolute);
                Console.Write(
                    value:
                    $"Fetching {Terminal.AccentTextStyle}{url.DnsSafeHost}{ANSI.ANSI_RESET} "); // extra space for the aesthetics
                RssChannel channel = await RssReader.Read(uri: url);
                Program._channels.Add(item: channel);
                Console.WriteLine(value: "fetched!");
                _ = Log.Information(message: $"Feed {uri} fetched.", tag: nameof(Program.FetchAllFeeds));
            }

            catch (Exception ex)
            {
                _ = Log.Exception(exception: ex);
            }

        Console.WriteLine();
        return true;
    }

    private static bool SelectFeed(out RssChannel channel)
    {
        channel = new RssChannel();
        if (Program._channels.Count == 0)
        {
            Console.WriteLine(
                value:
                $"No feeds fetched. Please fetch the feeds first using the \'{Terminal.AccentTextStyle}Fetch All Feeds{ANSI.ANSI_RESET}\' option.\n");
            Terminal.Pause();
            return false;
        }

        MenuItemCollection items = [];
        for (int x = 0; x < Program._channels.Count; x++)
            items.Add(item: new MenuItem(id: x,
                text: Program._channels[index: x].Title ?? Program._channels[index: x].Description));

        // select channel
        int option = ConsoleMenu.SelectMenu(items: items);
        if (option == -1)
            // selection cancelled
            return false;

        channel = Program._channels[index: option];
        return true;
    }

    private static bool OpenFeedView(RssChannel channel)
    {
        if (channel.Items.Count == 0)
        {
            // no items in the feed
            Console.WriteLine(value: "No items found in this feed.");
            return true;
        }

        foreach (RssItem item in channel.Items)
        {
            // print item to the screen
            Console.WriteLine(value: $"{Terminal.AccentHighlightStyle}{item.Title}{ANSI.ANSI_RESET}");
            Console.WriteLine(value: item.Description);
            Console.WriteLine();
        }

        return true;
    }
}