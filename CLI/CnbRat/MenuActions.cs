using System;
using System.IO;
using System.Linq;
using CnbRat.Core;
using Toolbox;
using Toolbox.UI;

namespace CnbRat;

internal static class MenuActions
{
    public static bool ErrorNoReport()
    {
        _ = Log.Warning(message: "No report available.");
        Console.WriteLine(
            value: $"No report available. {Terminal.AccentTextStyle}Fetch{ANSI.ANSI_RESET} a report first.");
        return true;
    }

    public static bool AboutCnbRat()
    {
        Console.WriteLine(value: $"About {Terminal.AccentTextStyle}CnbRat{ANSI.ANSI_RESET}");
        Console.WriteLine(
            value:
            $"Convert currencies with CNB precision. {Terminal.AccentTextStyle}Local. Fast. Reliable.{ANSI.ANSI_RESET}");
        Console.WriteLine();
        Console.WriteLine(
            value:
            "CnbRat is a local-first CLI tool for currency conversion using official daily exchange rates from the Czech National Bank (ČNB). Fetch rates, view supported currencies, and convert money amounts directly in your terminal. Fast, offline-friendly, and configurable via JSON.");
        return true;
    }

    public static bool DisplayLastReport(ExchangeManager manager)
    {
        // check if previous report is valid
        if (!manager.IsReady) return false;

        // display lates report info
        Console.WriteLine(value: $"{Terminal.Colors.Accent4}LATEST REPORT{ANSI.ANSI_RESET}");
        Console.WriteLine(value: $"Date:       {Terminal.AccentTextStyle}{manager.Exchange.Date}{ANSI.ANSI_RESET}\n" +
                                 $"Release:    {Terminal.AccentTextStyle}{manager.Exchange.Release}{ANSI.ANSI_RESET}\n" +
                                 $"Currencies: {Terminal.AccentTextStyle}{manager.Rates.Count}{ANSI.ANSI_RESET}");

        return true;
    }

    public static bool FetchReport(ExchangeManager manager)
    {
        // check if can fetch
        if (manager == null) return false;

        Console.Write(value: "Fetching data... ");
        bool result = manager.Fetch(date: DateOnly.MinValue);
        Console.WriteLine();

        if (!result)
        {
            Console.WriteLine(
                value:
                $"Unable to fetch report data. See the {Terminal.AccentTextStyle}log file{ANSI.ANSI_RESET} for more information.");
        }

        else
        {
            _ = manager.ArchiveReport();
            Console.WriteLine(value: $"Data {Terminal.AccentTextStyle}fetched successfully{ANSI.ANSI_RESET}.");
        }

        return result;
    }

    public static bool FetchReportToDate(ExchangeManager manager)
    {
        // check if can fetch
        if (manager == null) return false;

        // get date from user
        string input = Terminal.Input(prompt: "Enter date (YYYY-MM-DD): ", ensureValue: false);
        if (!DateOnly.TryParse(s: input, result: out DateOnly date))
        {
            _ = Log.Warning(message: "Invalid date format.");
            Console.WriteLine(value: "Invalid date format.");
            return false;
        }

        // fetch data to date
        Console.Write(value: "Fetching data... ");
        bool result = manager.Fetch(date: date);
        Console.WriteLine();

        if (!result)
        {
            Console.WriteLine(
                value:
                $"Unable to fetch report data. See the {Terminal.AccentTextStyle}log file{ANSI.ANSI_RESET} for more information.");
        }

        else
        {
            _ = manager.ArchiveReport();
            Console.WriteLine(value: $"Data {Terminal.AccentTextStyle}fetched successfully{ANSI.ANSI_RESET}.");
        }

        return result;
    }

    private static int GetLongerValue(int val1, int val2)
    {
        return val1 > val2 ? val1 : val2;
    }

    public static bool ViewReport(ExchangeManager manager)
    {
        // check exchange manager
        if (!manager.IsReady) return false;
        if (manager.Rates.Count == 0) return false;

        // display report (currencies, values, etc.)
        int lcountry, lcurrency, lamount, lcode, lrate;

        string COUNTRY = "Country";
        string CURRENCY = "Currency";
        string AMOUNT = "Amount";
        string CODE = "Code";
        string RATE = "Rate (CZK)";

        // get data size (for display purposes)
        lcountry = MenuActions.GetLongerValue(val1: manager.Rates.Max(selector: x => x.Country.Length),
            val2: COUNTRY.Length);
        lcurrency = MenuActions.GetLongerValue(val1: manager.Rates.Max(selector: x => x.Currency.Length),
            val2: CURRENCY.Length);
        lamount = MenuActions.GetLongerValue(val1: manager.Rates.Max(selector: x => x.Amount.ToString().Length),
            val2: AMOUNT.Length);
        lcode = MenuActions.GetLongerValue(val1: manager.Rates.Max(selector: x => x.Code.Length), val2: CODE.Length);
        lrate = MenuActions.GetLongerValue(val1: manager.Rates.Max(selector: x => x.Value.ToString().Length),
            val2: RATE.Length);

        // first rows
        string firstLine =
            $"{COUNTRY.PadRight(totalWidth: lcountry)} {CURRENCY.PadRight(totalWidth: lcurrency)} {AMOUNT.PadRight(totalWidth: lamount)} {CODE.PadRight(totalWidth: lcode)} {RATE.PadRight(totalWidth: lrate)}";
        Console.WriteLine(value: $"{Terminal.AccentTextStyle}{firstLine}{ANSI.ANSI_RESET}");
        Console.WriteLine(value: new string(c: '-', count: firstLine.Length));

        // data rows
        for (int x = 0; x < manager.Rates.Count; x++)
        {
            RateInfo rate = manager.Rates[index: x];

            // TODO: fix line coloring
            Console.WriteLine(
                value:
                $"{(x % 2 == 0 ? Terminal.Colors.Accent6 : ANSI.ANSI_RESET)}{rate.Country.PadRight(totalWidth: lcountry)} {rate.Currency.PadRight(totalWidth: lcurrency)} {rate.Amount.ToString().PadLeft(totalWidth: lamount)} {rate.Code.PadRight(totalWidth: lcode)} {rate.Value.ToString().PadLeft(totalWidth: lrate)}{ANSI.ANSI_RESET}");
        }

        return true;
    }

    private static int SelectCurrency(ExchangeManager manager, string prompt)
    {
        // check exchange manager (if conversions are available)
        if (manager == null || !manager.IsReady) return -1;

        // list currencies and let user select one
        int longest = manager.Rates.Max(selector: x => x.Country.Length);

        Console.Clear();
        for (int x = 0; x < manager.Rates.OrderBy(keySelector: x => x.Code.Normalize()).Count(); x += 2)
        {
            RateInfo rate1 = manager.Rates[index: x];
            RateInfo rate2;

            if (manager.Rates.Count > x + 1)
                rate2 = manager.Rates[index: x + 1];
            else
                rate2 = new RateInfo { Code = "N/A" };

            Console.Write(
                value:
                $"{Terminal.AccentTextStyle}{rate1.Code:D2}{ANSI.ANSI_RESET} - {rate1.Country.Normalize().PadRight(totalWidth: longest)} | ");

            if (rate2.Code != "N/A")
                Console.WriteLine(
                    value:
                    $"{Terminal.AccentTextStyle}{rate2.Code:D2}{ANSI.ANSI_RESET} - {rate2.Country.Normalize().PadRight(totalWidth: longest)}");

            else
                Console.WriteLine(); // line break
        }

        Console.WriteLine();

        string selectedCode =
            Terminal.Input(
                prompt:
                $"{prompt}{Environment.NewLine}Select currency by {Terminal.AccentTextStyle}code{ANSI.ANSI_RESET}: ",
                ensureValue: false);
        int index =
            manager.Rates.FindIndex(match: x =>
                string.Equals(a: x.Code, b: selectedCode, comparisonType: StringComparison.OrdinalIgnoreCase));
        if (index >= 0) return index;

        return -2;
    }

    public static bool CurrencyConverter(ExchangeManager manager)
    {
        // check exchange manager (if conversions are available)
        if (manager == null || !manager.IsReady) return false;

        // get source currency code (convert from)
        int sourceIndex = MenuActions.SelectCurrency(manager: manager, prompt: "Select source currency (convert from)");
        Console.WriteLine();

        // get target currency code (convert to)
        int targetIndex = MenuActions.SelectCurrency(manager: manager, prompt: "Select target currency (convert to)");
        Console.WriteLine();

        // check inputs
        if (sourceIndex < 0 || targetIndex < 0)
        {
            _ = Log.Warning(message: "Invalid currency code.");
            Console.WriteLine(value: "Invalid currency code.");
            return false;
        }

        Console.Clear();

        // get amount to convert (decimal)
        Console.WriteLine(
            value:
            $"Converting from {Terminal.AccentTextStyle}{manager.Rates[index: sourceIndex].Code}{ANSI.ANSI_RESET} to {Terminal.AccentTextStyle}{manager.Rates[index: targetIndex].Code}{ANSI.ANSI_RESET}");
        Console.WriteLine();

        if (!int.TryParse(
                s: Terminal.Input(
                    prompt:
                    $"Enter amount in {Terminal.AccentTextStyle}{manager.Rates[index: sourceIndex].Code}{ANSI.ANSI_RESET}: ",
                    ensureValue: false), result: out int amount) || amount <= 0)
        {
            _ = Log.Warning(message: "Invalid amount.");
            Console.WriteLine(value: "Invalid amount.");
            return false;
        }

        // perform conversion
        // FIXED: not all currencies are represented correctly! eg: thailands currency is described in amount of 100 not 1
        //decimal result = manager.Rates[sourceIndex].Value / manager.Rates[targetIndex].Value * amount;

        // correct conversion formula
        decimal sourceToCzk =
            manager.Rates[index: sourceIndex].Value / manager.Rates[index: sourceIndex].Amount * amount;
        decimal result = sourceToCzk /
                         (manager.Rates[index: targetIndex].Value / manager.Rates[index: targetIndex].Amount);

        // display result
        Console.WriteLine(
            value:
            $"Result: {amount} {manager.Rates[index: sourceIndex].Code} is {Terminal.AccentTextStyle}{result:F2}{ANSI.ANSI_RESET} {manager.Rates[index: targetIndex].Code}");
        return true;
    }

    private static ExchangeInfo GetInfoFromName(string filename)
    {
        // eg.  20250510180
        //          - year (4)
        //          - month (2)
        //          - day (2)
        //          - release (1+)

        // invalid file name (too short)
        if (filename.Length < 9) return new ExchangeInfo();

        // fallback object to return
        ExchangeInfo invalid = new()
        {
            Date = DateOnly.MinValue,
            Release = 0
        };

        // parse the info from the name
        if (!int.TryParse(s: filename[..4], result: out int year)) return invalid;
        if (!int.TryParse(s: filename[4..6], result: out int month)) return invalid;
        if (!int.TryParse(s: filename[6..8], result: out int day)) return invalid;
        if (!int.TryParse(s: filename[8..], result: out int release)) return invalid;

        // return the info
        return new ExchangeInfo
        {
            Date = new DateOnly(year: year, month: month, day: day),
            Release = release
        };
    }

    public static bool BrowseArchive(ExchangeManager manager)
    {
        // browse the archived exchange reports (so the user can load them instead of fetching a new one)
        if (!Directory.Exists(path: ExchangeManager.ArchiveDirectory))
        {
            _ = Log.Warning(message: "No archive directory found.", tag: nameof(MenuActions.BrowseArchive));
            return false;
        }

        string[] files = Directory.GetFiles(path: ExchangeManager.ArchiveDirectory);
        if (files.Length == 0)
        {
            _ = Log.Warning(message: "The archive is empty.", tag: nameof(MenuActions.BrowseArchive));
            return false;
        }

        if (manager == null) manager = new ExchangeManager();

        Console.Clear();

        // display files
        int batchSize = 5;
        for (int x = 0; x < files.Length; x += batchSize)
        {
            for (int y = 0; y < batchSize; y++)
            {
                if (files.Length <= x + y) break;
                string filename = Path.GetFileNameWithoutExtension(path: files[x + y]);

                // parse the info from the name
                ExchangeInfo info = MenuActions.GetInfoFromName(filename: filename);
                if (info.Date == DateOnly.MinValue || info.Release == 0)
                    // invalid file name
                    continue;

                Console.Write(
                    value: $"{Terminal.AccentTextStyle}{x + y:D2}{ANSI.ANSI_RESET} {info.Date} #{info.Release:D3} ");
            }

            Console.WriteLine();
        }

        if (!int.TryParse(
                s: Terminal.Input(
                    prompt: $"{Environment.NewLine}Enter the report {Terminal.AccentTextStyle}index{ANSI.ANSI_RESET}: ",
                    ensureValue: false), result: out int index))
        {
            _ = Log.Warning(message: "Invalid index (user-defined).", tag: nameof(MenuActions.BrowseArchive));
            return false;
        }

        string path = files.ElementAtOrDefault(index: index) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value: path) || !File.Exists(path: path))
        {
            _ = Log.Warning(message: "Invalid index (out of range).", tag: nameof(MenuActions.BrowseArchive));
            return false;
        }

        // load the selected file
        bool result = manager.OpenArchivedRecord(filename: path);
        if (result)
            _ = Log.Success(message: "Archived report loaded.", tag: nameof(MenuActions.BrowseArchive));

        else
            _ = Log.Error(message: "Unable to open archived report.", tag: nameof(MenuActions.BrowseArchive));

        return result;
    }
}