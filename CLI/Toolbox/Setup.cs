using System;

#if WINDOWS
using System.Runtime.InteropServices;
#endif

namespace Toolbox;

/// <summary>
/// Representing the setup class with various initialization methods.
/// </summary>
public static class Setup
{
#if WINDOWS
    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
#endif

    /// <summary>
    /// Representing the startup
    /// </summary>
    public static ConsoleWindowInfo StartupWindowInfo;

    /// <summary>
    /// Initializes the working environment. Contains platform-specific code.
    /// </summary>
    /// <returns>Operation result.</returns>
    /// <remarks>
    /// Enables the virtual terminal processing on the Windows platform.
    /// </remarks>
    public static bool Initialize()
    {
#if WINDOWS
        {
            // enable ANSI escape codes to older terminals
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(handle, out uint mode))
            {
                Console.WriteLine("Failed to get console mode.");
                return false;
            }

            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            if (!SetConsoleMode(handle, mode))
            {
                Console.WriteLine("Failed to set console mode.");
                return false;
            }
        }
#endif

        // get the initial terminal window size
        ConsoleWindowInfo.GetConsoleWindowInfo(ref StartupWindowInfo);

        return true;
    }

    /// <summary>
    /// Destroys the initialized setup.
    /// Works as an exit-cleanup call.
    /// </summary>
    /// <returns>Operation result as <see cref="bool"/>.</returns>
    public static bool Destroy()
    {
        // restore the startup window and buffer size
        _ = ConsoleWindowInfo.SetConsoleWindowInfo(ref StartupWindowInfo);
        return true;
    }
}
