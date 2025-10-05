namespace Toolbox;

/// <summary>
/// Representing the general extensions class.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Determines whether the string <paramref name="value"/> is empty using the <see cref="string.IsNullOrWhiteSpace"/> method.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
