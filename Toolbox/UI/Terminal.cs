using System;

namespace Toolbox.UI;

/// <summary>
/// Representing all methods associated with the terminal screen,
/// such as displaying particular items to the various parts of the screen.
/// </summary>
public static class Terminal
{
    /// <summary>
    /// Writes the latest log entry to the bottom of the terminal window.
    /// If no log entry exists, it does nothing.
    /// </summary>
    /// <remarks>
    /// This method will return the cursor back to its original position once the log entry is written.
    /// </remarks>
    public static void WriteLastLogEntry()
    {
        Log.Entry entry = Log.GetLastEntry();
        if (entry.EntryType == Log.LogType.Other)
        {
            // no log entries
            return;
        }

        // go to the bottom of the visible screen
        (int left, int top) = Console.GetCursorPosition();
        int bottom = Console.WindowHeight - top;

        // keep the height of the message in mind while counting the lines
        for (int x = 0; x < bottom - entry.Message.Split(Environment.NewLine).Length; x++)
        {
            Console.WriteLine();
        }

        Console.Write($"{Log.TypeNamesFormatted[entry.EntryType]} {entry.Message.PadRight(50)}");

        // restore cursor position
        Console.SetCursorPosition(left, top);
        return;
    }
}
