namespace Toolbox.UI;

/// <summary>
///     Representing a configuration interface for UI elements.
/// </summary>
public interface IConfig
{
    /// <summary>
    ///     Gets or sets the style name or identifier used for accent text rendering.
    /// </summary>
    string AccentTextStyle { get; set; }

    /// <summary>
    ///     Gets or sets the style name used to highlight accent elements in the user interface.
    /// </summary>
    /// <remarks>
    ///     Specify a valid style name to customize the appearance of accent highlights. The value should
    ///     correspond to a style defined in the application's resources or theme.
    /// </remarks>
    string AccentHighlightStyle { get; set; }
}