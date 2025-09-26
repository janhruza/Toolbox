using RSShell.UI;
using System;
using System.Security.Cryptography;

namespace RSShell;

internal class Program
{
    static int Main(string[] args)
    {
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
                    case 0:
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
            new MenuItem(0, "Exit"),
        };

        // get user input
        return ConsoleMenu.SelectMenu(menu);
    }
}
