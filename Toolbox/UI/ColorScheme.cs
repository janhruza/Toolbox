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

    /// <summary>
    /// Represents the ANSI escape sequence for gray text formatting.
    /// </summary>
    /// <remarks>Use this property to customize the color scheme for console applications that support ANSI
    /// color codes. The value should be a valid ANSI escape sequence recognized by the target terminal.</remarks>
    public string GrayText { get; set; } = "\e[38;5;243m";

    #region Static code

    /// <summary>
    /// Creates a new ColorScheme instance from an array of color values.
    /// </summary>
    /// <param name="colors">An array of six strings representing the accent colors. Each element corresponds to Accent1 through Accent6,
    /// respectively. If the array does not contain exactly six elements, a default ColorScheme is returned.</param>
    /// <returns>A ColorScheme initialized with the specified accent colors if the array contains six elements; otherwise, a
    /// default ColorScheme instance.</returns>
    public static ColorScheme FromArray(string[] colors)
    {
        if (colors.Length != 6)
        {
            return new ColorScheme();
        }

        else
        {
            ColorScheme scheme = new ColorScheme
            {
                Accent1 = colors[5],
                Accent2 = colors[4],
                Accent3 = colors[3],
                Accent4 = colors[2],
                Accent5 = colors[1],
                Accent6 = colors[0]
            };

            return scheme;
        }
    }

    #endregion
}
