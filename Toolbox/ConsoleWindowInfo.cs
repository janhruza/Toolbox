using System;

namespace Toolbox;

/// <summary>
/// Representing a basi console window info struct.
/// </summary>
public struct ConsoleWindowInfo
{
    /// <summary>
    /// Representing the terminal window width in columns.
    /// </summary>
    public int WindowWidth;

    /// <summary>
    /// Representing the terminal window height in rows.
    /// </summary>
    public int WindowHeight;

    /// <summary>
    /// Representing the terminal buffer width in columns.
    /// </summary>
    public int BufferWidth;

    /// <summary>
    /// Representing the terminal buffer height in rows.
    /// </summary>
    public int BufferHeight;

    #region Static code

    /// <summary>
    /// Gets the current console window and buffer information.
    /// </summary>
    /// <param name="cwInfo">Output object pointer.</param>
    /// <returns>Operation result as <see cref="bool"/>.</returns>
    public static bool GetConsoleWindowInfo(ref ConsoleWindowInfo cwInfo)
    {
        cwInfo.WindowWidth = Console.WindowWidth;
        cwInfo.WindowHeight = Console.WindowHeight;
        cwInfo.BufferWidth = Console.BufferWidth;
        cwInfo.BufferHeight = Console.BufferHeight;
        return true;
    }

    /// <summary>
    /// Sets the current console window and buffer information.
    /// </summary>
    /// <param name="cwInfo">Input object pointer.</param>
    /// <returns>Operation result as <see cref="bool"/>.</returns>
    /// <remarks>Works only on Windows.</remarks>
    public static bool SetConsoleWindowInfo(ref ConsoleWindowInfo cwInfo)
    {
#if WINDOWS
        Console.WindowWidth = cwInfo.WindowWidth;
        Console.WindowHeight = cwInfo.WindowHeight;
        Console.BufferWidth = cwInfo.BufferWidth;
        Console.BufferHeight = cwInfo.BufferHeight;
        return true;
#endif

        // always false for unsupported OS
        return false;
    }

    #endregion
}
