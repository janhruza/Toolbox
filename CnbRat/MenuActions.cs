using CnbRat.Core;

using System;
using System.Linq;

using Toolbox;
using Toolbox.UI;

namespace CnbRat;

internal static class MenuActions
{
    public static bool ErrorNoReport()
    {
        Log.Warning("No report available.");
        Console.WriteLine($"No report available. {Terminal.AccentTextStyle}Fetch{ANSI.ANSI_RESET} a report first.");
        return true;
    }

    public static bool AboutCnbRat()
    {
        Console.WriteLine($"About {Terminal.AccentTextStyle}CnbRat{ANSI.ANSI_RESET}");
        Console.WriteLine($"Convert currencies with CNB precision. {Terminal.AccentTextStyle}Local. Fast. Reliable.{ANSI.ANSI_RESET}");
        Console.WriteLine();
        Console.WriteLine($"CnbRat is a local-first CLI tool for currency conversion using official daily exchange rates from the Czech National Bank (ČNB). Fetch rates, view supported currencies, and convert money amounts directly in your terminal. Fast, offline-friendly, and configurable via JSON.");
        return true;
    }

    public static bool DisplayLastReport(ExchangeManager manager)
    {
        // check if previous report is valid
        if (manager.IsReady == false) return false;

        // display lates report info
        Console.WriteLine($"{Terminal.AccentHighlightStyle} LAST REPORT {ANSI.ANSI_RESET}");
        Console.WriteLine($"Date:       {Terminal.AccentTextStyle}{manager.Exchange.Date}{ANSI.ANSI_RESET}\n" +
                          $"Release:    {Terminal.AccentTextStyle}{manager.Exchange.Release}{ANSI.ANSI_RESET}\n" +
                          $"Currencies: {Terminal.AccentTextStyle}{manager.Rates.Count}{ANSI.ANSI_RESET}");

        return true;
    }

    public static bool FetchReport(ExchangeManager manager)
    {
        // check if can fetch
        if (manager == null) return false;

        Console.Write("Fetching data... ");
        bool result = manager.Fetch();
        Console.WriteLine();

        if (result == false)
        {
            Console.WriteLine($"Unable to fetch report data. See the {Terminal.AccentTextStyle}log file{ANSI.ANSI_RESET} for more information.");
        }

        else
        {
            Console.WriteLine($"Data {Terminal.AccentTextStyle}fetched successfully{ANSI.ANSI_RESET}.");
        }

        return result;
    }

    public static bool ViewReport(ExchangeManager manager)
    {
        // check exchange manager
        if (manager.IsReady == false) return false;
        if (manager.Rates.Count == 0) return false;

        // display report (currencies, values, etc.)
        int lcountry, lcurrency, lamount, lcode, lrate;

        // get data size (for display purposes)
        lcountry = manager.Rates.Max(x => x.Country.Length);
        lcurrency = manager.Rates.Max(x => x.Currency.Length);
        lamount = manager.Rates.Max(x => x.Amount.ToString().Length);
        lcode = manager.Rates.Max(x => x.Code.Length);
        lrate = manager.Rates.Max(x => x.Value.ToString().Length);

        // first rows
        string firstLine = $"{"Country".PadRight(lcountry)} {"Currency".PadRight(lcurrency)} {"Amount".PadRight(lamount)} {"Code".PadRight(lcode)} {"Rate".PadRight(lrate)}";
        Console.WriteLine(firstLine);
        Console.WriteLine(new string('-', firstLine.Length));

        // data rows
        for (int x = 0; x < manager.Rates.Count; x++)
        {
            RateInfo rate = manager.Rates[x];

            // TODO: fix line coloring
            Console.WriteLine($"{(x % 2 == 0 ? "\e[7m" : ANSI.ANSI_RESET)}{rate.Country.PadRight(lcountry)} {rate.Currency.PadRight(lcurrency)} {rate.Amount.ToString().PadLeft(lamount)} {rate.Code.PadRight(lcode)} {rate.Value.ToString().PadLeft(lrate)}{ANSI.ANSI_RESET}");
        }

        return true;
    }
}
