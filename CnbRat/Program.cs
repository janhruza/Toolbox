using CnbRat;
using CnbRat.Core;

using System;

using Toolbox;
using Toolbox.UI;

using static CnbRat.MenuIds;

internal class Program : IApplication
{
    static Program()
    {
        _exchangeManager = new ExchangeManager();
        Config.LoadDefault(out _config);
    }

    public static void DisplayBanner()
    {
        Console.WriteLine($"\t{Terminal.Colors.Accent1} ██████╗███╗   ██╗██████╗ ██████╗  █████╗ ████████╗{ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent2}██╔════╝████╗  ██║██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝{ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent3}██║     ██╔██╗ ██║██████╔╝██████╔╝███████║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent4}██║     ██║╚██╗██║██╔══██╗██╔══██╗██╔══██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent5}╚██████╗██║ ╚████║██████╔╝██║  ██║██║  ██║   ██║   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\t{Terminal.Colors.Accent6} ╚═════╝╚═╝  ╚═══╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   {ANSI.ANSI_RESET}");
        Console.WriteLine($"\tCNB Exchange rates!                  by {Terminal.AccentTextStyle}@jendahruza{ANSI.ANSI_RESET}");
        Console.WriteLine();
        return;
    }

    public static void PostExitCleanup()
    {
        // post exit cleanup
        Console.Clear();
        Config.WriteDefault(_config);
        return;
    }

    // associated objects
    static ExchangeManager _exchangeManager;

    static Config _config;

    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine("Arguments are disabled.");
            return 1;
        }

        // initialize terminal
        Setup.Initialize();
        Console.Title = "CnbRat - Exchange Rates";

        // load config
        Terminal.AccentTextStyle = _config.AccentTextStyle;
        Terminal.AccentHighlightStyle = _config.AccentHighlightStyle;

        // set default values
        //Terminal.AccentTextStyle = "\e[38;5;40m";                   // green text
        //Terminal.AccentHighlightStyle = "\e[48;5;40m\e[38;5;0m";    // green highlight

        // construct menu
        MenuItemCollection mainMenu;

        while (true)
        {
            Console.Clear();
            DisplayBanner();

            mainMenu = new MenuItemCollection
            {
                new MenuItem((int)ID_FETCH_DATA, "Fetch", DateTime.Now.ToShortDateString()),
                new MenuItem((int)ID_FETCH_DATA_SPECIFIC, "Fetch Specific Date", "[select]"),
                new MenuItem((int)ID_OPEN_ARCHIVE, "Archived Reports"),
                new MenuItem(),
                new MenuItem((int)ID_CONVERTER, "Currency Converter"),
                new MenuItem((int)ID_VIEW_RATES, "View Exchange Rates"),
                new MenuItem(),
                new MenuItem((int)ID_LAST_REPORT, "Last Report"),
                new MenuItem((int)ID_ABOUT_CNBRAT, "About CnbRat", Resources.Version.ToString()),
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

                case (int)ID_FETCH_DATA:
                    {
                        Console.Clear();
                        if (MenuActions.FetchReport(_exchangeManager) == true)
                        {
                            Log.Success($"Report fetched.");
                        }

                        else
                        {
                            Console.WriteLine();
                            Terminal.Pause();
                        }
                    }
                    break;

                case (int)ID_FETCH_DATA_SPECIFIC:
                    {
                        Console.Clear();
                        if (MenuActions.FetchReportToDate(_exchangeManager) == true)
                        {
                            Log.Success($"Report fetched.");
                        }

                        else
                        {
                            Console.WriteLine();
                            Terminal.Pause();
                        }
                    }
                    break;

                case (int)ID_ABOUT_CNBRAT:
                    {
                        Console.Clear();
                        MenuActions.AboutCnbRat();
                        Console.WriteLine();
                        Terminal.Pause();
                    }
                    break;

                case (int)ID_LAST_REPORT:
                    {
                        Console.Clear();
                        if (MenuActions.DisplayLastReport(_exchangeManager) == false)
                        {
                            // no report available
                            MenuActions.ErrorNoReport();
                        }

                        Console.WriteLine();
                        Terminal.Pause();
                    }
                    break;

                case (int)ID_VIEW_RATES:
                    {
                        Console.Clear();
                        if (MenuActions.ViewReport(_exchangeManager) == false)
                        {
                            // no report available
                            MenuActions.ErrorNoReport();
                        }

                        Console.WriteLine();
                        Terminal.Pause();
                    }
                    break;

                case (int)ID_CONVERTER:
                    {
                        Console.Clear();
                        if (MenuActions.CurrencyConverter(_exchangeManager) == false)
                        {
                            // no report available
                            MenuActions.ErrorNoReport();
                        }
                        Console.WriteLine();
                        Terminal.Pause();
                    }
                    break;

                case (int)ID_OPEN_ARCHIVE:
                    {
                        Console.Clear();
                        if (MenuActions.BrowseArchive(_exchangeManager) == false)
                        {
                            Console.WriteLine("Action failed.");
                            Terminal.Pause();
                        }
                    }
                    break;

                default: break;
            }
        }

    AppExit:
        PostExitCleanup();
        return 0;
    }
}