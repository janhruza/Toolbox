using System;
using Toolbox;
using Toolbox.UI;

namespace Algebrax;

/// <summary>
///     Main program class.
/// </summary>
public class Program : IApplication
{
    /// <inheritdoc />
    public static Version Version => new(major: 1, minor: 0, build: 0, revision: 0);

    /// <inheritdoc />
    public static void DisplayBanner()
    {
    }

    /// <inheritdoc />
    public static void LoadConfig()
    {
    }

    /// <inheritdoc />
    public static void PostExitCleanup()
    {
    }

    private static int Main(string[] args)
    {
        Program.DisplayBanner();

        Console.WriteLine(value: "QUADRATIC EQUATION");
        Console.WriteLine(value: "ax^2 +/- bx +/- c");
        // x^2 + x - 6 => 2, -3
        double a, b, c;

        a = double.Parse(s: Terminal.Input(prompt: "A: "));
        b = double.Parse(s: Terminal.Input(prompt: "B: "));
        c = double.Parse(s: Terminal.Input(prompt: "C: "));

        Console.WriteLine();

        double x1, x2;
        if (AlgMath.QuadraticEquation(a: a, b: b, c: c, x1: out x1, x2: out x2))
            // solution found
            Console.WriteLine(value: $"X1 = {x1}{Environment.NewLine}X2 = {x2}");

        else
            // no solution
            Console.WriteLine(value: "No solution.");

        Program.PostExitCleanup();
        return 0;
    }
}