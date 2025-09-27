using System;
using System.ComponentModel.DataAnnotations;

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
            // assign initial entry
            entry.EntryType = Log.LogType.Other;
            entry.Message = "No log entries so far.";
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

    /// <summary>
    /// Pauses the execution until the enter (return) key is pressed.
    /// </summary>
    public static void Pause()
    {
        Console.Write("Press enter to continue. . . ");
        Console.ReadLine();
        return;
    }

    /// <summary>
    /// Pauses the execution until the enter (return) key is pressed.
    /// This method specifies the <paramref name="message"/> tha is printed as the prompt.
    /// </summary>
    /// <param name="message">Custom message prompt.</param>
    public static void Pause(string message)
    {
        Console.Write(message);
        Console.ReadLine();
        return;
    }

    /// <summary>
    /// Prompts the user to enter text input.
    /// </summary>
    /// <param name="prompt">The input prompt.</param>
    /// <param name="ansiStyle">Custom input style specified using the ANSI escape code.</param>
    /// <param name="ensureValue">Determines whther the user must provide an input. If <see langword="true"/>, the method will not return until a non empty string is inputed.</param>
    /// <returns></returns>
    public static string Input(string prompt, string ansiStyle = "\e[38;5;200m", bool ensureValue = true)
    {
        string output = string.Empty;

        if (ensureValue)
        {
            // empty input prohibited
            while (string.IsNullOrWhiteSpace(output))
            {
                Console.Write(prompt + ansiStyle);
                output = Console.ReadLine() ?? string.Empty;
                Console.Write("\e[0m");
            }
        }

        else
        {
            // empty input allowed
            Console.Write(prompt + ansiStyle);
            output = Console.ReadLine() ?? string.Empty;
            Console.Write("\e[0m");
        }

        return output;
    }
}
