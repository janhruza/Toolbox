using System;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

namespace Mediavax;

internal class Program : IApplication
{
    public static void DisplayBanner()
    {
        Console.WriteLine($"\e[38;5;213m███╗   ███╗███████╗██████╗ ██╗ █████╗ ██╗   ██╗ █████╗ ██╗  ██╗\e[0m");
        Console.WriteLine($"\e[38;5;212m████╗ ████║██╔════╝██╔══██╗██║██╔══██╗██║   ██║██╔══██╗╚██╗██╔╝\e[0m");
        Console.WriteLine($"\e[38;5;211m██╔████╔██║█████╗  ██║  ██║██║███████║██║   ██║███████║ ╚███╔╝ \e[0m");
        Console.WriteLine($"\e[38;5;210m██║╚██╔╝██║██╔══╝  ██║  ██║██║██╔══██║╚██╗ ██╔╝██╔══██║ ██╔██╗ \e[0m");
        Console.WriteLine($"\e[38;5;209m██║ ╚═╝ ██║███████╗██████╔╝██║██║  ██║ ╚████╔╝ ██║  ██║██╔╝ ██╗\e[0m");
        Console.WriteLine($"\e[38;5;208m╚═╝     ╚═╝╚══════╝╚═════╝ ╚═╝╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝╚═╝  ╚═╝\e[0m");
        Console.WriteLine($"Simple yt-dlp wrapper!                           by {Terminal.AccentTextStyle}@jendahruza");
        Console.WriteLine();

        return;
    }

    public static void PostExitCleanup()
    {
        return;
    }

    static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            // arguments are disabled
            Console.WriteLine("Arguments are disabled.");
            return 1;
        }

        // initialize
        Setup.Initialize();
        Console.Title = "Mediavax - YT-DLP wrapper";

        // set terminal size
        //Console.Write("\e[8;25;80t");

        // main app loop
        while (true)
        {
            // clear & display banner
            Console.Clear();
            DisplayBanner();

            // draw menu
            MenuItemCollection menu = new MenuItemCollection
            {
                new MenuItem(0x10, "Media", string.IsNullOrWhiteSpace(MediaItem.Current.Address) ? "[select]" : MediaItem.Current.Address),
                new MenuItem(),
                new MenuItem(0, "Exit", "ESC")
            };

            // get selected menu item
            int option = ConsoleMenu.SelectMenu(menu);

            switch (option)
            {
                case -1:    // ESC key pressed
                case 0:
                    goto AppExit;

                default:
                    break;
            }
        }

        // app exit
        AppExit:
            PostExitCleanup();
            return 0;
    }
}
