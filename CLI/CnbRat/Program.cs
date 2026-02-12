using System;
using CnbRat;
using CnbRat.Core;
using Toolbox;
using Toolbox.UI;

internal class Program : IApplication
{
    // associated objects
    private static readonly ExchangeManager _exchangeManager;

    private static Config _config;

    static Program()
    {
        Program._config = new Config();
        Program._exchangeManager = new ExchangeManager();
        Program.LoadConfig();
    }

    public static void DisplayBanner()
    {
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent1} ██████╗███╗   ██╗██████╗ ██████╗  █████╗ ████████╗{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent2}██╔════╝████╗  ██║██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent3}██║     ██╔██╗ ██║██████╔╝██████╔╝███████║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent4}██║     ██║╚██╗██║██╔══██╗██╔══██╗██╔══██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent5}╚██████╗██║ ╚████║██████╔╝██║  ██║██║  ██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\t{Terminal.Colors.Accent6} ╚═════╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   {ANSI.ANSI_RESET}");
        Console.WriteLine(
            value: $"\tCNB Exchange rates!                  by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
        Console.Clear();
        _ = Config.WriteDefault(config: Program._config);
    }

    public static void LoadConfig()
    {
        // load config
        _ = Config.LoadDefault(config: out Program._config);

        // apply config
        Terminal.AccentTextStyle = Program._config.AccentTextStyle;
        Terminal.AccentHighlightStyle = Program._config.AccentHighlightStyle;
        Terminal.Colors = Program._config.Colors;
    }

    /// <summary>
    ///     Interface implementation, unused.
    /// </summary>
    public static Version Version => new();

    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine(value: "Arguments are disabled.");
            return 1;
        }

        // initialize terminal
        _ = Setup.Initialize();
        Console.Title = "CnbRat - Exchange Rates";

        // load config
        Program.LoadConfig();

        // set default values
        //Terminal.AccentTextStyle = "\e[38;5;40m";                   // green text
        //Terminal.AccentHighlightStyle = "\e[48;5;40m\e[38;5;0m";    // green highlight

        // construct menu
        MenuItemCollection mainMenu;

        while (true)
        {
            Console.Clear();
            Program.DisplayBanner();

            mainMenu = new MenuItemCollection
            {
                new MenuItem(id: (int)MenuIds.ID_FETCH_DATA, text: "Fetch", alt: DateTime.Now.ToShortDateString()),
                new MenuItem(id: (int)MenuIds.ID_FETCH_DATA_SPECIFIC, text: "Fetch Specific Date", alt: "[select]"),
                new MenuItem(id: (int)MenuIds.ID_OPEN_ARCHIVE, text: "Archived Reports"),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_CONVERTER, text: "Currency Converter"),
                new MenuItem(id: (int)MenuIds.ID_VIEW_RATES, text: "View Exchange Rates"),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_LAST_REPORT, text: "View Latest Report"),
                new MenuItem(id: (int)MenuIds.ID_ABOUT_CNBRAT, text: "About CnbRat", alt: Resources.Version.ToString()),
                new MenuItem(),
                new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Exit", alt: "ESC")
            };

            // TODO: main menu goes here
            int option = ConsoleMenu.SelectMenu(items: mainMenu);
            switch (option)
            {
                case 0:
                case -1:
                    goto AppExit;

                case (int)MenuIds.ID_FETCH_DATA:
                {
                    Console.Clear();
                    if (MenuActions.FetchReport(manager: Program._exchangeManager))
                    {
                        _ = Log.Success(message: "Report fetched.");
                    }

                    else
                    {
                        Console.WriteLine();
                        Terminal.Pause();
                    }
                }
                    break;

                case (int)MenuIds.ID_FETCH_DATA_SPECIFIC:
                {
                    Console.Clear();
                    if (MenuActions.FetchReportToDate(manager: Program._exchangeManager))
                    {
                        _ = Log.Success(message: "Report fetched.");
                    }

                    else
                    {
                        Console.WriteLine();
                        Terminal.Pause();
                    }
                }
                    break;

                case (int)MenuIds.ID_ABOUT_CNBRAT:
                {
                    Console.Clear();
                    _ = MenuActions.AboutCnbRat();
                    Console.WriteLine();
                    Terminal.Pause();
                }
                    break;

                case (int)MenuIds.ID_LAST_REPORT:
                {
                    Console.Clear();
                    if (!MenuActions.DisplayLastReport(manager: Program._exchangeManager))
                        // no report available
                        _ = MenuActions.ErrorNoReport();

                    Console.WriteLine();
                    Terminal.Pause();
                }
                    break;

                case (int)MenuIds.ID_VIEW_RATES:
                {
                    Console.Clear();
                    if (!MenuActions.ViewReport(manager: Program._exchangeManager))
                        // no report available
                        _ = MenuActions.ErrorNoReport();

                    Console.WriteLine();
                    Terminal.Pause();
                }
                    break;

                case (int)MenuIds.ID_CONVERTER:
                {
                    Console.Clear();
                    if (!MenuActions.CurrencyConverter(manager: Program._exchangeManager))
                        // no report available
                        _ = MenuActions.ErrorNoReport();
                    Console.WriteLine();
                    Terminal.Pause();
                }
                    break;

                case (int)MenuIds.ID_OPEN_ARCHIVE:
                {
                    Console.Clear();
                    if (!MenuActions.BrowseArchive(manager: Program._exchangeManager))
                    {
                        Console.WriteLine(value: "Action failed.");
                        Terminal.Pause();
                    }
                }
                    break;
            }
        }

        AppExit:
        Program.PostExitCleanup();
        return 0;
    }
}