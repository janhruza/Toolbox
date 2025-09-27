using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using RSShell.Core;
using RSShell.UI;

#if WINDOWS
using System.Runtime.InteropServices;
#endif

namespace RSShell;

internal class Program
{
#if WINDOWS
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
#endif

    const int ID_EXIT = 0;
    const int ID_LIST_FEEDS = 1;
    const int ID_ADD_FEED = 2;
    const int ID_ABOUT = 3;
    const int ID_SEPARATOR = 0x1000;
    const int ID_FETCH_FEEDS = 4;
    const int ID_SELECT_FEED = 5;

    static List<RssChannel> _channels = [];

    static async Task<int> Main(string[] args)
    {
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
            return 1;
        }

        else
        {
            Console.Title = "RSShell - Terminal RSS Reader";

            {
#if WINDOWS
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    // enable ANSI escape codes to older terminals
                    var handle = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (!GetConsoleMode(handle, out uint mode))
                    {
                        Console.WriteLine("Failed to get console mode.");
                        return -1;
                    }

                    mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                    if (!SetConsoleMode(handle, mode))
                    {
                        Console.WriteLine("Failed to set console mode.");
                        return -1;
                    }
                }

#endif
            }


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
                                Console.Write("Press enter to continue. . . ");
                                Console.ReadLine();
                            }
                        }
                        break;

                    case ID_ADD_FEED:
                        {
                            if (AddRssFeed() == true)
                            {
                                Console.Write($"\e[38;5;200mRSS feed added.\e[0m Press enter to continue. . . ");
                                Console.ReadLine();
                            }
                        }
                        break;

                    case ID_ABOUT:
                        {
                            if (AboutDialog() == true)
                            {
                                Console.Write("Press enter to continue. . . ");
                                Console.ReadLine();
                            }
                        }
                        break;

                    case ID_SEPARATOR:
                        break;

                    case ID_FETCH_FEEDS:
                        {
                            if (await FetchAllFeeds() == true)
                            {
                                Console.Write("Press enter to continue. . . ");
                                Console.ReadLine();
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
                                OpenFeedView(channel);
                                Console.Write("Press enter to continue. . . ");
                                Console.ReadLine();
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

    static void PostExitCleanup()
    {
        // TODO: cleanup code
        Config.Save(Config.Current);
        return;
    }

    static void DisplayBanner()
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
        Console.WriteLine("\e[0m\n"); // reset + line break
        return;
    }

    static string AnsiiText(string text, string format)
    {
        return format + text + "\e[0m";
    }

    static int HandleMenu()
    {
        MenuItemCollection menu = new MenuItemCollection
        {
            new MenuItem(ID_SELECT_FEED, "Open Feed"),
            new MenuItem(),
            new MenuItem(ID_FETCH_FEEDS, "Fetch All Feeds"),
            new MenuItem(),
            new MenuItem(ID_LIST_FEEDS, "List RSS Feeds"),
            new MenuItem(ID_ADD_FEED, "Add a new RSS feed"),
            new MenuItem(ID_ABOUT, "About RSShell"),
            new MenuItem(),
            new MenuItem(ID_EXIT, "Exit"),
        };

        // get user input
        return ConsoleMenu.SelectMenu(menu);
    }

    static bool ListFeeds()
    {
        if (Config.Current == null) return false;

        Console.Clear();

        if (Config.Current.Feeds.Count == 0)
        {
            Console.WriteLine("No RSS feeds found. Use the \'\e[38;5;201mAdd a new RSS feed\e[0m\' option to start.");
            return true;
        }

        foreach (string feed in Config.Current.Feeds)
        {
            if (string.IsNullOrEmpty(feed)) continue;
            Console.WriteLine(feed);
        }

        Console.WriteLine();

        return true;
    }

    static bool AddRssFeed()
    {
        if (Config.Current == null)
        {
            Config.Current = new Config();
        }

        Console.Clear();
        Console.Write("Enter RSS feed source\n# \e[38;5;200m");
        string addr = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("\e[0m");

        // add feed
        Config.Current.Feeds.Add(addr);
        return true;
    }

    static bool AboutDialog()
    {
        Console.Clear();

        Console.WriteLine("RSShell: Simple RSS reader inside your terminal!\n");
        return true;
    }

    static async Task<bool> FetchAllFeeds()
    {
        Console.Clear();
        _channels.Clear();
        foreach (string uri in Config.Current.Feeds)
        {
            Uri url = new Uri(uri, UriKind.RelativeOrAbsolute);
            Console.Write($"Fetching \e[38;5;200m{url.DnsSafeHost}\e[0m ");
            RssChannel channel = await RssReader.Read(url);
            _channels.Add(channel);
            Console.WriteLine($"fetched!");
        }

        return true;
    }

    static bool SelectFeed(out RssChannel channel)
    {
        channel = new RssChannel();
        if (_channels.Count == 0)
        {
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

    static bool OpenFeedView(RssChannel channel)
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
            Console.WriteLine($"\e[38;5;200m{item.Title}\e[0m");
            Console.WriteLine(item.Description);
            Console.WriteLine();
        }

        return true;
    }
}
