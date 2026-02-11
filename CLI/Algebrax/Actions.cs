using System;

using Toolbox;
using Toolbox.UI;

namespace Algebrax;

/// <summary>
/// Representing the sub routines.
/// </summary>
public static class Actions
{
    private static void BeginAction(string header)
    {
        Console.WriteLine(header);
        Console.WriteLine(new string('-', 40));
        return;
    }
    private static void EndAction()
    {
        Console.WriteLine(new string('-', 40));
        return;
    }

    /// <summary>
    /// Representing the quadratic equations calculator.
    /// </summary>
    public static bool QuadraticEquation()
    {
        BeginAction("QUADRATIC EQUATION");
        Console.WriteLine("ax^2 +/- bx +/- c");
        Console.WriteLine();

        double a, b, c;
        bool result;

        if (double.TryParse(Terminal.Input("A: ", true), out a) == false)
        {
            Console.WriteLine("Invalid input.");
            return false;
        }

        if (double.TryParse(Terminal.Input("B: ", true), out b) == false)
        {
            Console.WriteLine("Invalid input.");
            return false;
        }

        if (double.TryParse(Terminal.Input("C: ", true), out c) == false)
        {
            Console.WriteLine("Invalid input.");
            return false;
        }

        Console.WriteLine();

        double x1, x2;
        if (AlgMath.QuadraticEquation(a, b, c, out x1, out x2) == true)
        {
            // solution found

            if (x1 == x2)
            {
                // one single solution
                Console.WriteLine($"X1, X2 = {Terminal.AccentTextStyle}{x2}{ANSI.ANSI_RESET}");
            }

            else
            {
                // two solutions
                Console.WriteLine($"X1 = {Terminal.AccentTextStyle}{x1}{ANSI.ANSI_RESET}{Environment.NewLine}X2 = {Terminal.AccentTextStyle}{x2}{ANSI.ANSI_RESET}");
            }
            
            result = true;
        }

        else
        {
            // no solution
            Console.WriteLine("No solution.");
            result = false;
        }

        EndAction();
        return result;
    }
}
