using System;

namespace Toolbox.UI;

/// <summary>
///     Representing all methods associated with the terminal screen,
///     such as displaying particular items to the various parts of the screen.
/// </summary>
public static class Terminal
{
    /// <summary>
    ///     Representing a style of the custom colored text, such as captions, keywords and other, non-highlighted, elements,
    ///     The style is represented as an ANSI escape sequence.
    /// </summary>
    /// <remarks>
    ///     Use this property to customize the color scheme for console applications that support ANSI
    ///     color codes. The value must be a valid ANSI escape sequence recognized by the target terminal.
    /// </remarks>
    public static string AccentTextStyle { get; set; } = "\e[38;5;210m";

    /// <summary>
    ///     Representing a style of the highlighted items - such as highlighted menu item.
    ///     The style is represented as an ANSI escape sequence.
    /// </summary>
    /// ///
    /// <remarks>
    ///     Use this property to customize the color scheme for console applications that support ANSI
    ///     color codes. The value must be a valid ANSI escape sequence recognized by the target terminal.
    /// </remarks>
    public static string AccentHighlightStyle { get; set; } = "\e[48;5;210m\e[38;5;0m";

    #region Accent colors

    /// <summary>
    ///     Representing the active color scheme.
    /// </summary>
    public static ColorScheme Colors { get; set; } = new();

    #endregion

    /// <summary>
    ///     Writes the latest log entry to the bottom of the terminal window.
    ///     If no log entry exists, it does nothing.
    /// </summary>
    /// <remarks>
    ///     This method will return the cursor back to its original position once the log entry is written.
    /// </remarks>
    public static void WriteLastLogEntry()
    {
        // FIXME: correct display of strings longer than terminal columns size

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
        for (int x = 0; x < bottom - entry.Message.Split(separator: Environment.NewLine).Length; x++)
            Console.WriteLine();

        if (entry.Message.Length > 0)
            Console.Write(value: $"{Log.TypeNamesFormatted[key: entry.EntryType]} {entry.Message}");

        // restore cursor position
        Console.SetCursorPosition(left: left, top: top);
    }

    /// <summary>
    ///     Pauses the execution until the enter (return) key is pressed.
    /// </summary>
    public static void Pause()
    {
        Console.Write(value: $"Press {Terminal.AccentTextStyle}enter{ANSI.ANSI_RESET} to continue. . . ");
        _ = Console.ReadLine();
    }

    /// <summary>
    ///     Pauses the execution until the enter (return) key is pressed.
    ///     This method specifies the <paramref name="message" /> tha is printed as the prompt.
    /// </summary>
    /// <param name="message">Custom message prompt.</param>
    public static void Pause(string message)
    {
        Console.Write(value: message);
        _ = Console.ReadLine();
    }

    /// <summary>
    ///     Prompts the user to enter text input.
    /// </summary>
    /// <param name="prompt">The input prompt.</param>
    /// <param name="ensureValue">
    ///     Determines whther the user must provide an input. If <see langword="true" />, the method will
    ///     not return until a non empty string is inputed.
    /// </param>
    /// <returns></returns>
    public static string Input(string prompt, bool ensureValue = true)
    {
        string output = string.Empty;

        if (ensureValue)
        {
            // empty input prohibited
            while (string.IsNullOrWhiteSpace(value: output))
            {
                Console.Write(value: prompt + Terminal.AccentTextStyle);
                output = Console.ReadLine() ?? string.Empty;
                Console.Write(value: ANSI.ANSI_RESET);
            }
        }

        else
        {
            // empty input allowed
            Console.Write(value: prompt + Terminal.AccentTextStyle);
            output = Console.ReadLine() ?? string.Empty;
            Console.Write(value: ANSI.ANSI_RESET);
        }

        return output;
    }
}