using System;

namespace Algebrax;

/// <summary>
///     Representing the custom math class with various functions.
/// </summary>
public static class AlgMath
{
    /// <summary>
    ///     Calculates the roots of the given quadratic equation.
    /// </summary>
    /// <param name="a">Element A.</param>
    /// <param name="b">Element B.</param>
    /// <param name="c">Constant value.</param>
    /// <param name="x1">First root result.</param>
    /// <param name="x2">Second root result.</param>
    /// <returns>Operation result.</returns>
    public static bool QuadraticEquation(double a, double b, double c, out double x1, out double x2)
    {
        x1 = 0;
        x2 = 0;

        const double Epsilon = 1e-9; // 0.000000001
        double D = b * b - 4 * a * c;

        if (D > Epsilon)
        {
            double sD = Math.Sqrt(d: D);
            x1 = (-b + sD) / (2 * a);
            x2 = (-b - sD) / (2 * a);
            return true;
        }

        if (Math.Abs(value: D) <= Epsilon)
        {
            x1 = -b / (2 * a);
            x2 = x1;
            return true;
        }

        return false;
    }
}