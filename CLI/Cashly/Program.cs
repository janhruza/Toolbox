using System;
using Toolbox;
using Toolbox.UI;

namespace Cashly;

internal class Program : IApplication
{
    public static Version Version => new(major: 2025, minor: 10, build: 14);

    public static void DisplayBanner()
    {
        // display banner
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent1} ██████╗ █████╗ ███████╗██╗  ██╗██╗  ██╗   ██╗{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent2}██╔════╝██╔══██╗██╔════╝██║  ██║██║  ╚██╗ ██╔╝{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent3}██║     ███████║███████╗███████║██║   ╚████╔╝ {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent4}██║     ██╔══██║╚════██║██╔══██║██║    ╚██╔╝  {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent5}╚██████╗██║  ██║███████║██║  ██║███████╗██║   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent6} ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\tBudget Manager                 by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
    }

    public static void LoadConfig()
    {
        // temp solution
        Terminal.Colors = ColorScheme.FromArray(colors: ColorSchemes.BlueFade);
        Terminal.AccentTextStyle = "\e[38;5;33m"; // custom RGB value
        Terminal.AccentHighlightStyle = "\e[48;5;33m\e[38;5;15m";

        // load configuration from JSON file
    }

    public static void PostExitCleanup()
    {
        // additional post exit cleaning
        Console.Clear();
    }

    /// <summary>
    ///     Clears the console screen and buffer (same as <see cref="Console.Clear" />) and prints the banner afterwards using
    ///     the <see cref="DisplayBanner" /> method.
    /// </summary>
    public static void Clear()
    {
        Console.Clear();
        Program.DisplayBanner();
    }

    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine(value: "Arguments are disabled.");
            return 1;
        }

        // initialize terminal
        _ = Setup.Initialize();
        Console.Title = "Cashly - Budget Manager";

        // load config
        Program.LoadConfig();

        MenuItemCollection startMenu;

        while (true)
        {
            Program.Clear();

            startMenu = new MenuItemCollection
            {
                new MenuItem(id: (int)MenuIds.ID_SELECT_PROFILE, text: "Select Profile"),
                new MenuItem(id: (int)MenuIds.ID_CREATE_PROFILE, text: "Create a new Profile"),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Exit", alt: "ESC")
            };

            int option = ConsoleMenu.SelectMenu(items: startMenu);
            switch (option)
            {
                case ConsoleMenu.KEY_ESCAPE:
                case (int)MenuIds.ID_EXIT:
                    goto AppExit;

                case (int)MenuIds.ID_SELECT_PROFILE:
                {
                    if (!MenuActions.SelectProfile())
                    {
                        Program.Clear();
                        Terminal.Pause(
                            message:
                            $"Profile selection {Terminal.AccentTextStyle}failed{ANSI.ANSI_RESET} or was {Terminal.AccentTextStyle}cancelled{ANSI.ANSI_RESET}.\n" +
                            $"Press {Terminal.AccentTextStyle}enter{ANSI.ANSI_RESET} to continue. . . ");
                    }

                    else
                    {
                        // profile loaded, load main menu
                        _ = MenuActions.LoadProfileSession();
                    }
                }
                    break;

                case (int)MenuIds.ID_CREATE_PROFILE:
                {
                    if (!MenuActions.CreateProfile())
                    {
                        Program.Clear();
                        Terminal.Pause(
                            message:
                            $"Profile creation {Terminal.AccentTextStyle}failed{ANSI.ANSI_RESET} or was {Terminal.AccentTextStyle}cancelled{ANSI.ANSI_RESET}.\n" +
                            $"Press {Terminal.AccentTextStyle}enter{ANSI.ANSI_RESET} to continue. . . ");
                    }

                    else
                    {
                        // profile created, load main menu
                        _ = MenuActions.LoadProfileSession();
                    }
                }
                    break;

                // other option
            }
        }

        AppExit:
        Program.PostExitCleanup();
        return 0;
    }
}