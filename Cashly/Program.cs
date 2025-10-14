using System;

using Toolbox;
using Toolbox.UI;

using static Cashly.MenuIds;

namespace Cashly;

internal class Program : IApplication
{
    public static Version Version => new Version(2025, 10, 14);

    public static void DisplayBanner()
    {
        // display banner
        Console.WriteLine($"\t{Terminal.Colors.Accent1} ██████╗ █████╗ ███████╗██╗  ██╗██╗  ██╗   ██╗{ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent2}██╔════╝██╔══██╗██╔════╝██║  ██║██║  ╚██╗ ██╔╝{ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent3}██║     ███████║███████╗███████║██║   ╚████╔╝ {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent4}██║     ██╔══██║╚════██║██╔══██║██║    ╚██╔╝  {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent5}╚██████╗██║  ██║███████║██║  ██║███████╗██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent6} ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\tBudget Manager                 by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
        return;
    }

    public static void LoadConfig()
    {
        // temp solution
        Terminal.Colors = ColorScheme.FromArray(ColorSchemes.BlueFade);
        Terminal.AccentTextStyle = "\e[38;5;33m"; // custom RGB value
        Terminal.AccentHighlightStyle = "\e[48;5;33m\e[38;5;15m";

        // load configuration from JSON file
        return;
    }

    public static void PostExitCleanup()
    {
        // additional post exit cleaning
        Console.Clear();
        return;
    }

    static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine("Arguments are disabled.");
            return 1;
        }

        // initialize terminal
        Setup.Initialize();
        Console.Title = "Cashly - Budget Manager";

        // load config
        LoadConfig();

        MenuItemCollection startMenu;

        while (true)
        {
            Console.Clear();
            DisplayBanner();

            startMenu = new MenuItemCollection
            {
                new MenuItem((int)ID_SELECT_PROFILE, "Select Profile"),
                new MenuItem((int)ID_CREATE_PROFILE, "Create a new Profile"),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Exit", "ESC"),
            };

            int option = ConsoleMenu.SelectMenu(startMenu);
            switch (option)
            {
                case ConsoleMenu.KEY_ESCAPE:
                case (int)ID_EXIT:
                    goto AppExit;

                // other option
                default: break;
            }
        }

    AppExit:
        PostExitCleanup();
        return 0;
    }
}
