using Avalonia;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SystemUpdate;

/// <summary>
/// Main program class.
/// </summary>
internal static class Program
{
    #if WINDOWS
    #else
    [DllImport("libc")]
    private static extern uint getuid();
    #endif

    internal static bool IsRoot()
    {
        #if WINDOWS
        return true;
        #else
        return Program.getuid() == 0;
        #endif
    }

    /// <summary>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    /// yet and stuff might break.
    /// </summary>
    /// <param name="args">Application arguments.</param>
    [STAThread]
    private static int Main(string[] args)
    {
        return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Avalonia configuration, don't remove; also used by visual designer.
    /// </summary>
    /// <returns></returns>
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    /// <summary>
    /// Creates a new shell process.
    /// </summary>
    /// <param name="command">Command to be executed.</param>
    /// <param name="args">Command line arguments.</param>
    /// <param name="proc">Output process object.</param>
    /// <returns>
    /// <see langword="true"/> if the process has started successfully, otherwise <see langword="false"/>.
    /// </returns>
    private static bool CreateProcess(string command, string args, out Process proc)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            UseShellExecute = true
        };

        proc = new Process
        {
            StartInfo = psi
        };
        
        return proc.Start();
    }

    /// <summary>
    /// Creates a new process, waits for its exit and returns whether the exit code is 0 or not.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <param name="args">Command line arguments.</param>
    /// <returns>
    /// <see langword="true"/> if the process exit code is 0, otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method returns <see langword="true"/> only if the command completed successfully.
    /// </remarks>
    private static bool PerformTask(string command, string args)
    {
        if (CreateProcess(command, args, out Process proc) == true)
        {
            proc.WaitForExit();
            return proc.ExitCode == 0;
        }
        
        return false;
    }

    /// <summary>
    /// Creates a new process, waits for its exit and returns whether the exit code is 0 or not.
    /// This method is asynchronous.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    /// <param name="args">Command line arguments.</param>
    /// <returns>
    /// <see langword="true"/> if the process exit code is 0, otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This method returns <see langword="true"/> only if the command completed successfully.
    /// </remarks>
    public static async Task<bool> PerformTaskAsync(string command, string args)
    {
        bool result = await Task.Run<bool>(() => Program.PerformTask(command, args));
        return result;
    }
}