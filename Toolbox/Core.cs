using System;
using System.Diagnostics;
using System.IO;

namespace Toolbox;

/// <summary>
/// Representing shared, core, methods.
/// </summary>
public static class Core
{
    /// <summary>
    /// Starts a new shell process.
    /// </summary>
    /// <param name="command">Command or file name.</param>
    /// <param name="arguments">Command line arguments. Optional.</param>
    /// <param name="process">Output <see cref="Process"/> object.</param>
    /// <returns><see langword="true"/> if a new <see cref="Process"/> is created, otherwise <see langword="false"/>.</returns>
    public static bool CreateProcess(string command, string arguments, out Process? process)
    {
        try
        {
            process = Process.Start(command, arguments);
            return process != null;
        }

        catch (FileNotFoundException)
        {
            process = null;
            Log.Error($"Command \'{command}\' not found.", nameof(CreateProcess));
            return false;
        }

        catch (Exception ex)
        {
            process = null;
            Log.Exception(ex, nameof(CreateProcess));
            return false;
        }
    }
}
