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
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
#endif

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
        return true;
    }
}
