using CnbRat.Core;

using System;
using System.IO;
using System.Linq;

using Toolbox;
using Toolbox.UI;

using static Toolbox.ANSI;

namespace CnbRat;

internal static class MenuActions
{
    public static bool ErrorNoReport()
    {
        Log.Warning("No report available.");
        Console.WriteLine($"No report available. {Terminal.AccentTextStyle}Fetch{ANSI_RESET} a report first.");
        return true;
    }

    public static bool AboutCnbRat()
    {
        Console.WriteLine($"About {Terminal.AccentTextStyle}CnbRat{ANSI_RESET}");
        Console.WriteLine($"Convert currencies with CNB precision. {Terminal.AccentTextStyle}Local. Fast. Reliable.{ANSI_RESET}");
        Console.WriteLine();
        Console.WriteLine($"CnbRat is a local-first CLI tool for currency conversion using official daily exchange rates from the Czech National Bank (ČNB). Fetch rates, view supported currencies, and convert money amounts directly in your terminal. Fast, offline-friendly, and configurable via JSON.");
        return true;
    }

    public static bool DisplayLastReport(ExchangeManager manager)
    {
        // check if previous report is valid
        if (manager.IsReady == false) return false;

        // display lates report info
        Console.WriteLine($"{Terminal.AccentHighlightStyle} LAST REPORT {ANSI_RESET}");
        Console.WriteLine($"Date:       {Terminal.AccentTextStyle}{manager.Exchange.Date}{ANSI_RESET}\n" +
                          $"Release:    {Terminal.AccentTextStyle}{manager.Exchange.Release}{ANSI_RESET}\n" +
                          $"Currencies: {Terminal.AccentTextStyle}{manager.Rates.Count}{ANSI_RESET}");

        return true;
    }

    public static bool FetchReport(ExchangeManager manager)
    {
        // check if can fetch
        if (manager == null) return false;

        Console.Write("Fetching data... ");
        bool result = manager.Fetch(DateOnly.MinValue);
        Console.WriteLine();

        if (result == false)
        {
            Console.WriteLine($"Unable to fetch report data. See the {Terminal.AccentTextStyle}log file{ANSI_RESET} for more information.");
        }

        else
        {
            manager.ArchiveReport();
            Console.WriteLine($"Data {Terminal.AccentTextStyle}fetched successfully{ANSI_RESET}.");
        }

        return result;
    }

    public static bool FetchReportToDate(ExchangeManager manager)
    {
        // check if can fetch
        if (manager == null) return false;

        // get date from user
        string input = Terminal.Input("Enter date (YYYY-MM-DD): ", false);
        if (DateOnly.TryParse(input, out DateOnly date) == false)
        {
            Log.Warning("Invalid date format.");
            Console.WriteLine("Invalid date format.");
            return false;
        }

        // fetch data to date
        Console.Write("Fetching data... ");
        bool result = manager.Fetch(date);
        Console.WriteLine();

        if (result == false)
        {
            Console.WriteLine($"Unable to fetch report data. See the {Terminal.AccentTextStyle}log file{ANSI_RESET} for more information.");
        }

        else
        {
            manager.ArchiveReport();
            Console.WriteLine($"Data {Terminal.AccentTextStyle}fetched successfully{ANSI_RESET}.");
        }

        return result;
    }

    static int GetLongerValue(int val1, int val2)
    {
        return val1 > val2 ? val1 : val2;
    }

    public static bool ViewReport(ExchangeManager manager)
    {
        // check exchange manager
        if (manager.IsReady == false) return false;
        if (manager.Rates.Count == 0) return false;

        // display report (currencies, values, etc.)
        int lcountry, lcurrency, lamount, lcode, lrate;

        string COUNTRY = "Country";
        string CURRENCY = "Currency";
        string AMOUNT = "Amount";
        string CODE = "Code";
        string RATE = "Rate (CZK)";

        // get data size (for display purposes)
        lcountry = GetLongerValue(manager.Rates.Max(x => x.Country.Length), COUNTRY.Length);
        lcurrency = GetLongerValue(manager.Rates.Max(x => x.Currency.Length), CURRENCY.Length);
        lamount = GetLongerValue(manager.Rates.Max(x => x.Amount.ToString().Length), AMOUNT.Length);
        lcode = GetLongerValue(manager.Rates.Max(x => x.Code.Length), CODE.Length);
        lrate = GetLongerValue(manager.Rates.Max(x => x.Value.ToString().Length), RATE.Length);

        // first rows
        string firstLine = $"{COUNTRY.PadRight(lcountry)} {CURRENCY.PadRight(lcurrency)} {AMOUNT.PadRight(lamount)} {CODE.PadRight(lcode)} {RATE.PadRight(lrate)}";
        Console.WriteLine($"{Terminal.AccentTextStyle}{firstLine}{ANSI_RESET}");
        Console.WriteLine(new string('-', firstLine.Length));

        // data rows
        for (int x = 0; x < manager.Rates.Count; x++)
        {
            RateInfo rate = manager.Rates[x];

            // TODO: fix line coloring
            Console.WriteLine($"{(x % 2 == 0 ? ANSI_REVERSE : ANSI_RESET)}{rate.Country.PadRight(lcountry)} {rate.Currency.PadRight(lcurrency)} {rate.Amount.ToString().PadLeft(lamount)} {rate.Code.PadRight(lcode)} {rate.Value.ToString().PadLeft(lrate)}{ANSI.ANSI_RESET}");
        }

        return true;
    }

    static int SelectCurrency(ExchangeManager manager, string prompt)
    {
        // check exchange manager (if conversions are available)
        if (manager == null || manager.IsReady == false) return -1;

        // list currencies and let user select one
        int longest = manager.Rates.Max(x => x.Country.Length);

        Console.Clear();
        for (int x = 0; x < manager.Rates.OrderBy(x => x.Code.Normalize()).Count(); x += 2)
        {
            RateInfo rate1 = manager.Rates[x];
            RateInfo rate2;

            if (manager.Rates.Count > x+1)
            {
                rate2 = manager.Rates[x + 1];
            }
            else
            {
                rate2 = new RateInfo { Code = "N/A" };
            }

            Console.Write($"{Terminal.AccentTextStyle}{rate1.Code:D2}{ANSI_RESET} - {rate1.Country.Normalize().PadRight(longest)} | ");

            if (rate2.Code != "N/A")
            {
                Console.WriteLine($"{Terminal.AccentTextStyle}{rate2.Code:D2}{ANSI_RESET} - {rate2.Country.Normalize().PadRight(longest)}");
            }

            else
            {
                Console.WriteLine(); // line break
            }
        }

        Console.WriteLine();

        string selectedCode = Terminal.Input($"{prompt}{Environment.NewLine}Select currency by {Terminal.AccentTextStyle}code{ANSI_RESET}: ", false);
        int index = manager.Rates.FindIndex(x => string.Equals(x.Code, selectedCode, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
        {
            return index;
        }

        else
        {
            return -2;
        }
    }

    public static bool CurrencyConverter(ExchangeManager manager)
    {
        // check exchange manager (if conversions are available)
        if (manager == null || manager.IsReady == false) return false;

        // get source currency code (convert from)
        int sourceIndex = SelectCurrency(manager, "Select source currency (convert from)");
        Console.WriteLine();

        // get target currency code (convert to)
        int targetIndex = SelectCurrency(manager, "Select target currency (convert to)");
        Console.WriteLine();

        // check inputs
        if (sourceIndex < 0 || targetIndex < 0)
        {
            Log.Warning("Invalid currency code.");
            Console.WriteLine("Invalid currency code.");
            return false;
        }

        Console.Clear();

        // get amount to convert (decimal)
        Console.WriteLine($"Converting from {Terminal.AccentTextStyle}{manager.Rates[sourceIndex].Code}{ANSI_RESET} to {Terminal.AccentTextStyle}{manager.Rates[targetIndex].Code}{ANSI_RESET}");
        Console.WriteLine();

        if (int.TryParse(Terminal.Input($"Enter amount in {Terminal.AccentTextStyle}{manager.Rates[sourceIndex].Code}{ANSI_RESET}: ", false), out int amount) == false || amount <= 0)
        {
            Log.Warning("Invalid amount.");
            Console.WriteLine("Invalid amount.");
            return false;
        }

        // perform conversion
        // each rate contains a representation of 1 unit of currency in CZK (no need to multiply by amount)
        decimal result = manager.Rates[sourceIndex].Value / manager.Rates[targetIndex].Value * amount;

        // display result
        Console.WriteLine($"Result: {amount} {manager.Rates[sourceIndex].Code} is {Terminal.AccentTextStyle}{result:F2}{ANSI_RESET} {manager.Rates[targetIndex].Code}");
        return true;
    }

    static ExchangeInfo GetInfoFromName(string filename)
    {
        // eg.  20250510180
        //          - year (4)
        //          - month (2)
        //          - day (2)
        //          - release (1+)

        // invalid file name (too short)
        if (filename.Length < 9) return new ExchangeInfo();

        // fallback object to return
        ExchangeInfo invalid = new ExchangeInfo
        {
            Date = DateOnly.MinValue,
            Release = 0
        };

        // parse the info from the name
        if (int.TryParse(filename[0..4], out int year) == false) return invalid;
        if (int.TryParse(filename[4..6], out int month) == false) return invalid;
        if (int.TryParse(filename[6..8], out int day) == false) return invalid;
        if (int.TryParse(filename[8..], out int release) == false) return invalid;

        // return the info
        return new ExchangeInfo
        {
            Date = new DateOnly(year, month, day),
            Release = release
        };
    }

    public static bool BrowseArchive(ExchangeManager manager)
    {
        // browse the archived exchange reports (so the user can load them instead of fetching a new one)
        if (Directory.Exists(ExchangeManager.ArchiveDirectory) == false)
        {
            Log.Warning("No archive directory found.", nameof(BrowseArchive));
            return false;
        }

        string[] files = Directory.GetFiles(ExchangeManager.ArchiveDirectory);
        if (files.Length == 0)
        {
            Log.Warning("The archive is empty.", nameof(BrowseArchive));
            return false;
        }

        if (manager == null)
        {
            manager = new ExchangeManager();
        }

        Console.Clear();

        // display files
        int batchSize = 5;
        for (int x = 0; x < files.Length; x += batchSize)
        {
            for (int y = 0; y < batchSize; y++)
            {
                if (files.Length <= x+y) break;
                string filename = Path.GetFileNameWithoutExtension(files[x + y]);

                // parse the info from the name
                ExchangeInfo info = GetInfoFromName(filename);
                if (info.Date == DateOnly.MinValue || info.Release == 0)
                {
                    // invalid file name
                    continue;
                }

                Console.Write($"{Terminal.AccentTextStyle}{(x+y):D2}{ANSI_RESET} {info.Date} #{info.Release:D3} ");
            }

            Console.WriteLine();
        }

        if (int.TryParse(Terminal.Input($"{Environment.NewLine}Enter the report {Terminal.AccentTextStyle}index{ANSI_RESET}: ", false), out int index) == false)
        {
            Log.Warning("Invalid index (user-defined).", nameof(BrowseArchive));
            return false;
        }

        string path = files.ElementAtOrDefault(index) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path) == true || File.Exists(path) == false)
        {
            Log.Warning("Invalid index (out of range).", nameof(BrowseArchive));
            return false;
        }

        // load the selected file
        bool result = manager.OpenArchivedRecord(path);
        if (result == true)
        {
            Log.Success("Archived report loaded.", nameof(BrowseArchive));
        }

        else
        {
            Log.Error("Unable to open archived report.", nameof(BrowseArchive));
        }

        return result;
    }
}
