using System;
using System.Diagnostics;
using System.IO;

namespace Toolbox;

/// <summary>
///     Representing shared, core, methods.
/// </summary>
public static class Core
{
    /// <summary>
    ///     Starts a new shell process.
    /// </summary>
    /// <param name="command">Command or file name.</param>
    /// <param name="arguments">Command line arguments. Optional.</param>
    /// <param name="process">Output <see cref="Process" /> object.</param>
    /// <returns><see langword="true" /> if a new <see cref="Process" /> is created, otherwise <see langword="false" />.</returns>
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
            _ = Log.Error($"Command \'{command}\' not found.", nameof(CreateProcess));
            return false;
        }

        catch (Exception ex)
        {
            process = null;
            _ = Log.Exception(ex, nameof(CreateProcess));
            return false;
        }
    }

    /// <summary>
    ///     Starts a new process.
    ///     This method is the extended version of the basic <see cref="CreateProcess(string, string, out Process?)" /> method.
    /// </summary>
    /// <param name="command">Command or file name.</param>
    /// <param name="arguments">Command line arguments. Optional.</param>
    /// <param name="process">Output <see cref="Process" /> object.</param>
    /// <param name="shellExec">Determines whether the shell will start the process.</param>
    /// <param name="directory">Specifies the <paramref name="process" /> working directory.</param>
    /// <returns><see langword="true" /> if a new <see cref="Process" /> is created, otherwise <see langword="false" />.</returns>
    public static bool CreateProcess(string command, string arguments, out Process? process, bool shellExec,
        string directory)
    {
        try
        {
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = shellExec,
                    WorkingDirectory = directory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                }
            };

            return process.Start();
        }

        catch (FieldAccessException)
        {
            process = null;
            _ = Log.Error($"File or command \'{command}\' not found.", nameof(CreateProcess));
            return false;
        }

        catch (Exception ex)
        {
            process = null;
            _ = Log.Exception(ex);
            return false;
        }
    }
}