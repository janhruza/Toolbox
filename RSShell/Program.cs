using RSShell.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Toolbox;
using Toolbox.UI;

namespace RSShell;

/// <summary>
/// Representing the RSShell program class.
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

    private static List<RssChannel> _channels = [];

    /// <summary>
    /// Interface implementation, unused.
    /// </summary>
    public static Version Version => new Version();

    /// <summary>
    /// Representing the main application method.
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
        if (Config.Load(out Config cfg) == true)
        {
            Config.Current = cfg;
        }

        else
        {
            Config.Current = new Config();
        }

        if (args.Length > 0)
        {
            // TODO: arguments disabled
            Console.WriteLine("Arguments are disabled.");
            return 1;
        }

        else
        {
            Console.Title = "RSShell - Terminal RSS Reader";

            while (true)
            {
                Console.Clear();
                DisplayBanner();

                int option = HandleMenu();

                switch (option)
                {
                    case ID_LIST_FEEDS:
                        {
                            if (ListFeeds() == true)
                            {
                                Terminal.Pause();
                            }
                        }
                        break;

                    case ID_ADD_FEED:
                        {
                            if (AddRssFeed() == true)
                            {
                                Terminal.Pause($"{Terminal.AccentTextStyle}RSS feed added.{ANSI.ANSI_RESET} Press enter to continue. . . ");
                            }

                            else
                            {
                                Terminal.Pause($"{Terminal.AccentTextStyle}No feed was added.{ANSI.ANSI_RESET} Press enter to continue. . . "); ;
                            }
                        }
                        break;

                    case ID_ABOUT:
                        {
                            if (AboutDialog() == true)
                            {
                                Terminal.Pause();
                            }
                        }
                        break;

                    case ID_SEPARATOR:
                        break;

                    case ID_FETCH_FEEDS:
                        {
                            if (await FetchAllFeeds() == true)
                            {
                                Terminal.Pause();
                            }
                        }
                        break;

                    case ID_SELECT_FEED:
                        {
                            // open feedview to select the feed to view
                            Console.Clear();
                            if (SelectFeed(out RssChannel channel) == true)
                            {
                                // open feed view
                                Console.Clear();
                                _ = OpenFeedView(channel);
                                Terminal.Pause();
                            }
                        }
                        break;

                    // 'exit' option
                    case ID_EXIT:
                        goto AppExit;
                }
            }

        // cleanup code
        AppExit:
            PostExitCleanup();
            return 0;
        }
    }

    public static void PostExitCleanup()
    {
        // TODO: cleanup code
        _ = Config.Save(Config.Current);
        Console.Clear();
        return;
    }

    public static void DisplayBanner()
    {
        // Displays the ANSI colored banner
        Console.Write("\e[38;5;21m");
        Console.WriteLine("██████╗ ███████╗███████╗██╗  ██╗███████╗██╗     ██╗     ");
        Console.Write("\e[38;5;57m");
        Console.WriteLine("██╔══██╗██╔════╝██╔════╝██║  ██║██╔════╝██║     ██║     ");
        Console.Write("\e[38;5;93m");
        Console.WriteLine("██████╔╝███████╗███████╗███████║█████╗  ██║     ██║     ");
        Console.Write("\e[38;5;129m");
        Console.WriteLine("██╔══██╗╚════██║╚════██║██╔══██║██╔══╝  ██║     ██║     ");
        Console.Write("\e[38;5;165m");
        Console.WriteLine("██║  ██║███████║███████║██║  ██║███████╗███████╗███████╗");
        Console.Write("\e[38;5;201m");
        Console.Write("╚═╝  ╚═╝╚══════╝╚══════╝╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝");
        Console.WriteLine("\e[0m"); // reset
        Console.WriteLine($"Terminal RSS reader                       by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
        return;
    }

    public static void LoadConfig()
    {
        // NOTE: implemented to satisfy IApplication interface, no real use
        return;
    }

    private static int HandleMenu()
    {
        MenuItemCollection menu = new MenuItemCollection
        {
            new MenuItem(ID_SELECT_FEED, "Open Feed"),
            new MenuItem(),
            new MenuItem(ID_FETCH_FEEDS, "Fetch All Feeds"),
            new MenuItem(),
            new MenuItem(ID_LIST_FEEDS, "List RSS Feeds"),
            new MenuItem(ID_ADD_FEED, "Add a new RSS Feed"),
            new MenuItem(ID_ABOUT, "About RSShell"),
            new MenuItem(),
            new MenuItem(ID_EXIT, "Exit"),
        };

        // get user input
        return ConsoleMenu.SelectMenu(menu);
    }

    private static bool ListFeeds()
    {
        // ensure config
        Config.Current ??= new Config();

        Console.Clear();

        if (Config.Current.Feeds.Count == 0)
        {
            Console.WriteLine($"No RSS feeds found. Use the \'{Terminal.AccentTextStyle}Add a new RSS Feed{ANSI.ANSI_RESET}\' option to start.\n");
            return true;
        }

        // size of the longest URL in the feeds list
        int longest = Config.Current.Feeds.Max(x => x.Length);

        Console.WriteLine($"ID\tURL Address");
        Console.WriteLine($"--\t{new string('\u2015', longest)}");
        for (int x = 0; x < Config.Current.Feeds.Count; x++)
        {
            Console.WriteLine($"{(x + 1):D2}\t{Config.Current.Feeds[x]}");
        }

        Console.WriteLine();
        return true;
    }

    private static bool AddRssFeed()
    {
        // ensure config
        Config.Current ??= new Config();

        Console.Clear();
        string addr = Terminal.Input($"Enter {Terminal.AccentTextStyle}RSS feed source{ANSI.ANSI_RESET} (leave blank to cancel)\n# ", false);
        Console.WriteLine(ANSI.ANSI_RESET);

        if (addr.IsEmpty() == true)
        {
            // no feed added
            return false;
        }

        // add feed
        Config.Current.Feeds.Add(addr);
        return true;
    }

    private static bool AboutDialog()
    {
        Console.Clear();

        Console.WriteLine($"{Terminal.AccentTextStyle}About RSShell{ANSI.ANSI_RESET}\nSimple RSS reader inside your terminal!\n");
        return true;
    }

    private static async Task<bool> FetchAllFeeds()
    {
        Console.Clear();
        _channels.Clear();

        if (Config.Current.Feeds.Count == 0)
        {
            Console.WriteLine($"No RSS feeds to fetch. Use the \'{Terminal.AccentTextStyle}Add a new RSS Feed{ANSI.ANSI_RESET}\' option to add new feeds.\n");
            return true;
        }

        foreach (string uri in Config.Current.Feeds)
        {
            try
            {
                Uri url = new Uri(uri, UriKind.RelativeOrAbsolute);
                Console.Write($"Fetching {Terminal.AccentTextStyle}{url.DnsSafeHost}{ANSI.ANSI_RESET} "); // extra space for the aesthetics
                RssChannel channel = await RssReader.Read(url);
                _channels.Add(channel);
                Console.WriteLine($"fetched!");
                _ = Log.Information($"Feed {uri} fetched.", nameof(FetchAllFeeds));
            }

            catch (Exception ex)
            {
                _ = Log.Exception(ex);
            }
        }

        Console.WriteLine();
        return true;
    }

    private static bool SelectFeed(out RssChannel channel)
    {
        channel = new RssChannel();
        if (_channels.Count == 0)
        {
            Console.WriteLine($"No feeds fetched. Please fetch the feeds first using the \'{Terminal.AccentTextStyle}Fetch All Feeds{ANSI.ANSI_RESET}\' option.\n");
            Terminal.Pause();
            return false;
        }

        MenuItemCollection items = [];
        for (int x = 0; x < _channels.Count; x++)
        {
            items.Add(new MenuItem(x, _channels[x].Title ?? _channels[x].Description));
        }

        // select channel
        int option = ConsoleMenu.SelectMenu(items);
        if (option == -1)
        {
            // selection cancelled
            return false;
        }

        channel = _channels[option];
        return true;
    }

    private static bool OpenFeedView(RssChannel channel)
    {
        if (channel.Items.Count == 0)
        {
            // no items in the feed
            Console.WriteLine("No items found in this feed.");
            return true;
        }

        foreach (RssItem item in channel.Items)
        {
            // print item to the screen
            Console.WriteLine($"{Terminal.AccentHighlightStyle}{item.Title}{ANSI.ANSI_RESET}");
            Console.WriteLine(item.Description);
            Console.WriteLine();
        }

        return true;
    }
}
