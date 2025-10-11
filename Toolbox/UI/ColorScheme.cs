namespace Toolbox.UI;

/// <summary>
/// Representing a color scheme with defined accent colors.
/// </summary>
public class ColorScheme
{
    /// <summary>
    /// Gets or sets the ANSI escape sequence for the first accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent1 { get; set; } = "\e[38;5;40m";

    /// <summary>
    /// Gets or sets the ANSI escape sequence for the second accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent2 { get; set; } = "\e[38;5;76m";

    /// <summary>
    /// Gets or sets the ANSI escape sequence for the third accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent3 { get; set; } = "\e[38;5;112m";

    /// <summary>
    /// Gets or sets the ANSI escape sequence for the fourth accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent4 { get; set; } = "\e[38;5;148m";

    /// <summary>
    /// Gets or sets the ANSI escape sequence for the fifth accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent5 { get; set; } = "\e[38;5;184m";

    /// <summary>
    /// Gets or sets the ANSI escape sequence for the sixth accent color used in console output.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string Accent6 { get; set; } = "\e[38;5;220m";
}
