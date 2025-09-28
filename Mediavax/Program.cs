using System;
using System.Diagnostics;
using Mediavax.Core;
using Toolbox;
using Toolbox.UI;

using static Mediavax.MenuIds;
using static Toolbox.ANSI;

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
        Console.Clear();
        return;
    }

    /// <summary>
    /// Processes the input action <paramref name="actionResult"/> and prints either a successful <paramref name="trueMessage"/> or failed <paramref name="falseMessage"/>.
    /// </summary>
    /// <param name="actionResult">Input operation result.</param>
    /// <param name="trueMessage">A message that will be printed if the <paramref name="actionResult"/> is evaluated as <see langword="true"/>.</param>
    /// <param name="falseMessage">A message that will be printed if the <paramref name="actionResult"/> is evaluated as <see langword="false"/>.</param>
    public static void HandleAction(bool actionResult, string trueMessage, string falseMessage)
    {
        if (actionResult)
        {
            Console.Clear();
            Console.Write($"\e[48;5;10m\e[38;5;0m GOOD {ANSI_RESET} ");
            Console.WriteLine(trueMessage);
        }

        else
        {
            Console.Clear();
            Console.Write($"\e[48;5;1m\e[38;5;15m FAIL {ANSI_RESET} ");
            Console.WriteLine(falseMessage);
        }

        Terminal.Pause();
        return;
    }

    /// <summary>
    /// Ensures a valid alt text based on <paramref name="value"/>.
    /// </summary>
    /// <param name="value">A value to evaluate.</param>
    /// <param name="alt">Alternative text if the <paramref name="value"/> is empty.</param>
    /// <returns>
    /// The original <paramref name="value"/> or <paramref name="alt"/>.
    /// </returns>
    public static string ActionText(string value, string alt = "[default]")
    {
        return string.IsNullOrWhiteSpace(value) ? alt : value;
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
                new MenuItem((int)ID_SELECT_URL, "Media", ActionText(MediaItem.Current.Address, "[select]")),
                new MenuItem((int)ID_SELECT_MODE, "Mode", ActionText(MediaItem.Current.Mode)),
                new MenuItem((int)ID_SELECT_QUALITY, "Quality", ActionText(MediaItem.Current.Quality)),
                new MenuItem((int)ID_SELECT_FORMAT, "Format", ActionText(MediaItem.Current.Format)),
                new MenuItem((int)ID_SELECT_LOCATION, "Location", ActionText(MediaItem.Current.Location)),
                new MenuItem(),
                new MenuItem((int)ID_EXTRAS, "Extras", ">"),
                new MenuItem(),
                new MenuItem((int)ID_START_DOWNLOAD, "Verify & Download"),
                new MenuItem(),
                new MenuItem((int)ID_EXIT, "Exit", "ESC")
            };

            // get selected menu item
            int option = ConsoleMenu.SelectMenu(menu);

            switch (option)
            {
                // exit the application
                case -1:    // ESC key pressed
                case (int)ID_EXIT:
                    goto AppExit;

                // select media URL
                case (int)ID_SELECT_URL:
                    {
                        Console.Clear();
                        HandleAction(MenuActions.SelectMediaSource(),
                            trueMessage: $"Selected media {Terminal.AccentTextStyle}updated{ANSI_RESET} to {Terminal.AccentTextStyle}{MediaItem.Current.Address}{ANSI_RESET}",
                            falseMessage: $"Action {Terminal.AccentTextStyle}cancelled{ANSI.ANSI_RESET}.");
                    }
                    break;

                // select mode: audio-only, video-only, both
                case (int)ID_SELECT_MODE:
                    break;

                // select download quality
                case (int)ID_SELECT_QUALITY:
                    break;

                // select output format: MP4, MP3, etc
                case (int)ID_SELECT_FORMAT:
                    break;

                // select output directory
                case (int)ID_SELECT_LOCATION:
                    break;

                // open extras menu
                case (int)ID_EXTRAS:
                    {
                        Console.Clear();
                        MenuActions.OpenExtras();
                    }
                    break;

                // start the download
                case (int)ID_START_DOWNLOAD:
                    {
                        Console.Clear();
                        HandleAction(MenuActions.StartDownload(),
                            trueMessage: $"The {Terminal.AccentTextStyle}download{ANSI_RESET} has {Terminal.AccentTextStyle}started{ANSI_RESET}.",
                            falseMessage: $"{Terminal.AccentTextStyle}Unable to start{ANSI_RESET} download.");
                    }
                    break;

                // undefined option
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
