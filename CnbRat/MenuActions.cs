using System;

using Toolbox;
using Toolbox.UI;

namespace CnbRat;

internal static class MenuActions
{
    public static bool AboutCnbRat()
    {
        Console.WriteLine($"About {Terminal.AccentTextStyle}CnbRat{ANSI.ANSI_RESET}");
        Console.WriteLine($"Convert currencies with CNB precision. {Terminal.AccentTextStyle}Local. Fast. Reliable.{ANSI.ANSI_RESET}");
        Console.WriteLine();
        Console.WriteLine($"CnbRat is a local-first CLI tool for currency conversion using official daily exchange rates from the Czech National Bank (ČNB). Fetch rates, view supported currencies, and convert money amounts directly in your terminal. Fast, offline-friendly, and configurable via JSON.");
        Console.WriteLine();
        return true;
    }
}
