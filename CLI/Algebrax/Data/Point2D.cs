namespace Algebrax.Data;

/// <summary>
/// Representing the point in a 2-D plane.
/// </summary>
public struct Point2D
{
    /// <summary>
    /// Creates a new <see cref="Point2D"/>.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Point2D(double x, double y)
    {
        this.X = x;
        this.Y = y;
        return;
    }

    /// <summary>
    /// Representing the X coordinate.
    /// </summary>
    public double X;

    /// <summary>
    /// Representing the Y coordinate.
    /// </summary>
    public double Y;
}
