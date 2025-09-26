using System;
using System.Threading.Tasks;
using RSShell.UI;

namespace RSShell;

internal class Program
{
    const int ID_LIST_FEEDS = 1;
    const int ID_EXIT = 0;
    const int ID_ADD_FEED = 2;
    const int ID_ABOUT = 3;

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
            new MenuItem(ID_LIST_FEEDS, "List RSS Feeds"),
            new MenuItem(ID_ADD_FEED, "Add a new RSS feed"),
            new MenuItem(ID_ABOUT, "About RSShell"),
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
}
