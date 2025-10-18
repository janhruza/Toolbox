using System;
using System.Linq;
using System.Threading;

using Toolbox;
using Toolbox.UI;

using static Toolbox.ANSI;

namespace Cashly.Core;

/// <summary>
/// Representing actions with a loaded user profile.
/// </summary>
/// <remarks>
/// All methods should access the <see cref="Session.Profile"/> for their operations.
/// Login check is compared towards the <see cref="Session.ProfileLoaded"/> property.
/// Methods that display content assume that the screen was cleared by the caller.
/// </remarks>
internal static class SessionActions
{
    /// <summary>
    /// Handles a no profile loaded state.
    /// Use as a return value when a profile is needed but none is loaded.
    /// </summary>
    /// <returns>Always <see langword="false"/>.</returns>
    static bool NoProfileLoaded()
    {
        // log unauthorized access
        Log.Error("Unauthorized, no profile loaded.", nameof(NoProfileLoaded));

        // show the unauthorized screen
        Console.Clear();
        Console.WriteLine($"ACCESS DENIED{Environment.NewLine}No profile loaded. {Terminal.AccentTextStyle}Select{ANSI.ANSI_RESET} a profile or {Terminal.AccentTextStyle}create{ANSI.ANSI_RESET} a new one first.{Environment.NewLine}");

        // pause the execution until a key is pressed
        Terminal.Pause();

        // always returns false
        return false;
    }

    public static bool ShowDashboard()
    {
        if (Session.ProfileLoaded == false)
        {
            // no profile loaded
            return NoProfileLoaded();
        }

        Console.WriteLine("DASHBOARD");

        if (Session.Profile.Transactions.Count == 0)
        {
            // no transactions yet
            Console.WriteLine($"No transactions. Start by creating some!");
            return true;
        }

        // list last 5 incomes and expanses
        // incomes
        int longest = Session.Profile.Transactions.Max(x => x.Description.Length);
        int longestValue = Session.Profile.Transactions.Max(x => x.Amount.ToString().Length);
        var latestTransactions = Session.Profile.Transactions.OrderByDescending(x => x.Date).ToList();  // max first (max date = newest)

        Console.WriteLine("Transaction History");
        for (int x = 0; x < 5; x++)
        {
            if (latestTransactions.Count == x) break;
            Console.Write($"{latestTransactions[x].Description.PadRight(longest)} ");
            switch (latestTransactions[x].Type)
            {
                // income
                case TransactionType.Income:
                    {
                        Console.WriteLine($" {Terminal.Colors.Accent1}{latestTransactions[x].Amount.ToString().PadLeft(longestValue)}{ANSI_RESET}");
                    }
                    break;

                // expanse
                case TransactionType.Expense:
                    {
                        Console.WriteLine($"{Terminal.Colors.Accent4}-{latestTransactions[x].Amount.ToString().PadLeft(longestValue)}{ANSI_RESET}");
                    }
                    break;
            }
        }

        // balance
        {
            Console.WriteLine(new string('-', longest + longestValue + 2));
            Console.Write($"{"Total".PadRight(longest)} ");

            // determine positive or negative balance (for coloring purposes)
            decimal balance = Session.Profile.Balance;
            if (balance >= 0)
            {
                Console.WriteLine($" {Terminal.Colors.Accent1}{balance.ToString().PadLeft(longestValue)}{ANSI_RESET}");
            }

            else
            {
                Console.WriteLine($"{Terminal.Colors.Accent4}-{balance.ToString().PadLeft(longestValue)}{ANSI_RESET}");
            }
        }

        Console.WriteLine();
        return true;
    }
}
