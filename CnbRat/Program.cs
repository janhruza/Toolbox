using CnbRat.Core;

using System;

using Toolbox;
using Toolbox.UI;

using static CnbRat.MenuIds;

internal class Program : IApplication
{
    public static void DisplayBanner()
    {
        Console.WriteLine($"{Terminal.AccentTextStyle} ██████╗███╗   ██╗██████╗ ██████╗  █████╗ ████████╗{ANSI.ANSI_RESET}");
        Console.WriteLine($"{Terminal.AccentTextStyle}██╔════╝████╗  ██║██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝{ANSI.ANSI_RESET}");
        Console.WriteLine($"{Terminal.AccentTextStyle}██║     ██╔██╗ ██║██████╔╝██████╔╝███████║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"{Terminal.AccentTextStyle}██║     ██║╚██╗██║██╔══██╗██╔══██╗██╔══██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"{Terminal.AccentTextStyle}╚██████╗██║ ╚████║██████╔╝██║  ██║██║  ██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"{Terminal.AccentTextStyle} ╚═════╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   {ANSI.ANSI_RESET}");
        Console.WriteLine($"CNB Exchange rates!                  by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
        return;
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
        Console.Clear();
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
        Setup.Initialize();

        // construct menu
        MenuItemCollection mainMenu;

        while (true)
        {
            Console.Clear();
            DisplayBanner();

            mainMenu = new MenuItemCollection
            {
                new MenuItem((int)ID_FETCH_DATA, "Fetch", DateTime.Now.ToShortDateString()),
                new MenuItem((int)ID_FETCH_DATA_SPECIFIC, "Fetch to Date", "[select]"),
                new MenuItem(),
                new MenuItem((int)ID_LAST_REPORT, "Last Report"),
                new MenuItem((int)ID_ABOUT_CNBRAT, "About CnbRat"),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Exit", "ESC")
            };

            // TODO: main menu goes here
            int option = ConsoleMenu.SelectMenu(mainMenu);
            switch (option)
            {
                case 0:
                case -1:
                    goto AppExit;

                default: break;
            }
        }
        

    AppExit:
        PostExitCleanup();
        return 0;
    }
}