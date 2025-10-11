namespace Toolbox;

/// <summary>
/// Contains ANSI escape codes.
/// </summary>
public static class ANSI
{
    /// <summary>
    /// Representing the ANSI reset escape sequence.
    /// </summary>
    public const string ANSI_RESET = "\e[0m";

    /// <summary>
    /// Represents the ANSI escape sequence for reverse video mode, which swaps foreground and background colors in
    /// terminal output.
    /// </summary>
    /// <remarks>Use this constant to format console or terminal text with reverse video, enhancing visibility
    /// or highlighting specific output. Support for ANSI escape sequences may vary depending on the terminal or console
    /// environment.</remarks>
    public const string ANSI_REVERSE = "\e[7m";
}
