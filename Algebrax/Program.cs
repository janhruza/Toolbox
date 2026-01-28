using System;

using Toolbox;
using Toolbox.UI;

namespace Algebrax;

/// <summary>
/// Main program class.
/// </summary>
public class Program : IApplication
{
    /// <inheritdoc/>
    public static Version Version => new Version(1, 0, 0, 0);

    /// <inheritdoc/>
    public static void DisplayBanner()
    {
        return;
    }

    /// <inheritdoc/>
    public static void LoadConfig()
    {
        return;
    }

    /// <inheritdoc/>
    public static void PostExitCleanup()
    {
        return;
    }

    static int Main(string[] args)
    {
        DisplayBanner();

        Console.WriteLine("QUADRATIC EQUATION");
        Console.WriteLine("ax^2 +/- bx +/- c");
        // x^2 + x - 6 => 2, -3
        double a, b, c;

        a = double.Parse(Terminal.Input("A: ", true));
        b = double.Parse(Terminal.Input("B: ", true));
        c = double.Parse(Terminal.Input("C: ", true));

        Console.WriteLine();

        double x1, x2;
        if (AlgMath.QuadraticEquation(a, b, c, out x1, out x2) == true)
        {
            // solution found
            Console.WriteLine($"X1 = {x1}{Environment.NewLine}X2 = {x2}");
        }

        else
        {
            // no solution
            Console.WriteLine("No solution.");
        }

        PostExitCleanup();
        return 0;
    }
}
