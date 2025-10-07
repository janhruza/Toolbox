using System;

using Toolbox;
using Toolbox.UI;

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
        return;
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
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

        MenuItemCollection mainMenu = new MenuItemCollection
        {
            new MenuItem()
        };

        while (true)
        {
            Console.Clear();
            DisplayBanner();

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