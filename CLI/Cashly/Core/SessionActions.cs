using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox;
using Toolbox.UI;

namespace Cashly.Core;

/// <summary>
///     Representing actions with a loaded user profile.
/// </summary>
/// <remarks>
///     All methods should access the <see cref="Session.Profile" /> for their operations.
///     Login check is compared towards the <see cref="Session.ProfileLoaded" /> property.
///     Methods that display content assume that the screen was cleared by the caller.
/// </remarks>
internal static class SessionActions
{
    /// <summary>
    ///     Handles a no profile loaded state.
    ///     Use as a return value when a profile is needed but none is loaded.
    /// </summary>
    /// <returns>Always <see langword="false" />.</returns>
    private static bool NoProfileLoaded()
    {
        // log unauthorized access
        _ = Log.Error(message: "Unauthorized, no profile loaded.", tag: nameof(SessionActions.NoProfileLoaded));

        // show the unauthorized screen
        Console.Clear();
        Console.WriteLine(
            value:
            $"ACCESS DENIED{Environment.NewLine}No profile loaded. {Terminal.AccentTextStyle}Select{ANSI.ANSI_RESET} a profile or {Terminal.AccentTextStyle}create{ANSI.ANSI_RESET} a new one first.{Environment.NewLine}");

        // pause the execution until a key is pressed
        Terminal.Pause();

        // always returns false
        return false;
    }

    public static bool ShowDashboard()
    {
        if (!Session.ProfileLoaded)
            // no profile loaded
            return SessionActions.NoProfileLoaded();

        Console.WriteLine(value: "DASHBOARD");

        if (Session.Profile.Transactions.Count == 0)
        {
            // no transactions yet
            Console.WriteLine(value: "No transactions. Start by creating some!");
            return true;
        }

        // list last 5 incomes and expanses
        // incomes
        int longest = Session.Profile.Transactions.Max(selector: x => x.Description.Length);
        int longestValue = Session.Profile.Transactions.Max(selector: x => x.Amount.ToString().Length);
        List<Transaction> latestTransactions =
            Session.Profile.Transactions.OrderByDescending(keySelector: x => x.Date)
                .ToList(); // max first (max date = newest)

        Console.WriteLine(value: "Transaction History");
        for (int x = 0; x < 5; x++)
        {
            if (latestTransactions.Count == x) break;
            Console.Write(value: $"{latestTransactions[index: x].Description.PadRight(totalWidth: longest)} ");
            switch (latestTransactions[index: x].Type)
            {
                // income
                case TransactionType.Income:
                {
                    Console.WriteLine(
                        value:
                        $" {Terminal.Colors.Accent1}{latestTransactions[index: x].Amount.ToString().PadLeft(totalWidth: longestValue)}{ANSI.ANSI_RESET}");
                }
                    break;

                // expanse
                case TransactionType.Expense:
                {
                    Console.WriteLine(
                        value:
                        $"{Terminal.Colors.Accent4}-{latestTransactions[index: x].Amount.ToString().PadLeft(totalWidth: longestValue)}{ANSI.ANSI_RESET}");
                }
                    break;
            }
        }

        // balance
        {
            Console.WriteLine(value: new string(c: '-', count: longest + longestValue + 2));
            Console.Write(value: $"{"Total".PadRight(totalWidth: longest)} ");

            // determine positive or negative balance (for coloring purposes)
            decimal balance = Session.Profile.Balance;
            if (balance >= 0)
                Console.WriteLine(
                    value:
                    $" {Terminal.Colors.Accent1}{balance.ToString().PadLeft(totalWidth: longestValue)}{ANSI.ANSI_RESET}");

            else
                Console.WriteLine(
                    value:
                    $"{Terminal.Colors.Accent4}-{balance.ToString().PadLeft(totalWidth: longestValue)}{ANSI.ANSI_RESET}");
        }

        Console.WriteLine();
        return true;
    }
}