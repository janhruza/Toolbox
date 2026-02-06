using System;

using Toolbox;
using Toolbox.UI;

using static Cashly.MenuIds;
using static Toolbox.ANSI;

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

    /// <summary>
    /// Clears the console screen and buffer (same as <see cref="Console.Clear"/>) and prints the banner afterwards using the <see cref="DisplayBanner"/> method.
    /// </summary>
    public static void Clear()
    {
        Console.Clear();
        DisplayBanner();
        return;
    }

    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine("Arguments are disabled.");
            return 1;
        }

        // initialize terminal
        _ = Setup.Initialize();
        Console.Title = "Cashly - Budget Manager";

        // load config
        LoadConfig();

        MenuItemCollection startMenu;

        while (true)
        {
            Program.Clear();

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

                case (int)ID_SELECT_PROFILE:
                    {
                        if (MenuActions.SelectProfile() == false)
                        {
                            Program.Clear();
                            Terminal.Pause($"Profile selection {Terminal.AccentTextStyle}failed{ANSI_RESET} or was {Terminal.AccentTextStyle}cancelled{ANSI_RESET}.\n" +
                                           $"Press {Terminal.AccentTextStyle}enter{ANSI_RESET} to continue. . . ");
                            break;
                        }

                        else
                        {
                            // profile loaded, load main menu
                            _ = MenuActions.LoadProfileSession();
                        }
                    }
                    break;

                case (int)ID_CREATE_PROFILE:
                    {
                        if (MenuActions.CreateProfile() == false)
                        {
                            Program.Clear();
                            Terminal.Pause($"Profile creation {Terminal.AccentTextStyle}failed{ANSI_RESET} or was {Terminal.AccentTextStyle}cancelled{ANSI_RESET}.\n" +
                                           $"Press {Terminal.AccentTextStyle}enter{ANSI_RESET} to continue. . . ");
                        }

                        else
                        {
                            // profile created, load main menu
                            _ = MenuActions.LoadProfileSession();
                        }
                    }
                    break;

                // other option
                default: break;
            }
        }

    AppExit:
        PostExitCleanup();
        return 0;
    }
}
