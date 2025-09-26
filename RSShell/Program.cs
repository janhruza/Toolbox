using System;
using System.Threading.Tasks;
using RSShell.UI;

namespace RSShell;

internal class Program
{
    const int ID_EXIT = 0;

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
            new MenuItem(1, "List RSS Feeds"),
            new MenuItem(2, "Add a new RSS feed"),
            new MenuItem(3, "About RSShell"),
            new MenuItem(ID_EXIT, "Exit"),
        };

        // get user input
        return ConsoleMenu.SelectMenu(menu);
    }
}
