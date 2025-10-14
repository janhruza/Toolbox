using System;

namespace Toolbox.UI;

/// <summary>
/// Representing a class with predefined color schemes represented as arrays of 6 strings each.
/// Each array corresponds to a different color scheme, allowing for easy switching between themes.
/// </summary>
public static class ColorSchemes
{
    /// <summary>
    /// Gets an array of ANSI escape codes representing the colors of the rainbow, suitable for terminal text
    /// formatting.
    /// </summary>
    /// <remarks>The array contains color codes for red, orange, yellow, green, blue, and violet, in that
    /// order. These codes can be used to style console output with vibrant colors in environments that support ANSI
    /// escape sequences.</remarks>
    public static string[] Rainbow => new string[] {
        "\u001b[38;5;196m", // Červená
        "\u001b[38;5;202m", // Oranžová
        "\u001b[38;5;226m", // Žlutá
        "\u001b[38;5;46m",  // Zelená
        "\u001b[38;5;21m",  // Modrá
        "\u001b[38;5;201m"  // Fialová
    };

    /// <summary>
    /// Provides a set of ANSI escape codes for pastel foreground colors, suitable for styling console output.
    /// </summary>
    /// <remarks>The array includes escape codes for light blue, turquoise, light green, light yellow, pink,
    /// and lavender colors. These codes can be used to format text in terminals that support 256-color ANSI sequences.
    /// The order of colors in the array corresponds to their intended visual grouping.</remarks>
    public static string[] Pastel => new string[] {
        "\u001b[38;5;153m", // Světle modrá
        "\u001b[38;5;159m", // Tyrkysová
        "\u001b[38;5;151m", // Světle zelená
        "\u001b[38;5;186m", // Světle žlutá
        "\u001b[38;5;218m", // Růžová
        "\u001b[38;5;183m"  // Levandulová
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a selection of neon foreground colors for console output.
    /// </summary>
    /// <remarks>The array includes escape codes for bright blue, neon purple, bright yellow, bright green,
    /// bright red, and magenta. These codes can be used to style console text with vibrant colors on terminals that
    /// support 256-color ANSI sequences.</remarks>
    public static string[] Neon => new string[] {
        "\u001b[38;5;51m",  // Azurová
        "\u001b[38;5;201m", // Neon fialová
        "\u001b[38;5;226m", // Jasně žlutá
        "\u001b[38;5;46m",  // Jasně zelená
        "\u001b[38;5;196m", // Jasně červená
        "\u001b[38;5;93m"   // Magenta
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a predefined set of elegant color styles for console output.
    /// </summary>
    /// <remarks>The colors included in this set are various shades of gray, blue, green, and brown, chosen to
    /// provide a subtle and refined appearance in console applications. These escape codes can be used to format text
    /// output with consistent styling across supported terminals.</remarks>
    public static string[] Elegant => new string[] {
        "\u001b[38;5;240m", // Tmavě šedá
        "\u001b[38;5;244m", // Střední šedá
        "\u001b[38;5;250m", // Světle šedá
        "\u001b[38;5;31m",  // Tmavě modrá
        "\u001b[38;5;65m",  // Olivově zelená
        "\u001b[38;5;137m"  // Hnědá
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a fire and ice color gradient for terminal output.
    /// </summary>
    /// <remarks>The array includes colors ranging from fiery red and orange to icy blue shades. These escape
    /// codes can be used to style text in terminals that support 256-color ANSI sequences.</remarks>
    public static string[] FireIce => new string[] {
        "\u001b[38;5;196m", // Ohnivě červená
        "\u001b[38;5;202m", // Oranžová
        "\u001b[38;5;226m", // Žlutá
        "\u001b[38;5;51m",  // Ledově modrá
        "\u001b[38;5;39m",  // Tyrkysová
        "\u001b[38;5;21m"   // Hluboká modrá
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a gradient of green foreground colors for console output.
    /// </summary>
    /// <remarks>The array provides a sequence of green shades, ranging from dark to bright, suitable for
    /// creating fade effects in terminal applications. The escape codes use 256-color mode and are intended for
    /// environments that support ANSI color sequences.</remarks>
    public static string[] GreenFade => new string[] {
        "\u001b[38;5;22m",  // tmavě zelená
        "\u001b[38;5;28m",  // zelená2
        "\u001b[38;5;34m",  // zelená3
        "\u001b[38;5;40m",  // zelená4
        "\u001b[38;5;46m",  // jasná zelená
        "\u001b[38;5;82m"   // neonově zelená
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a gradient of blue foreground colors for console output.
    /// </summary>
    /// <remarks>The array provides a sequence of blue shades, ranging from dark blue to lighter azure tones.
    /// These codes can be used to style console text with varying blue colors, typically for visual effects or
    /// highlighting.</remarks>
    public static string[] BlueFade => new string[] {
        "\u001b[38;5;17m",  // tmavě modrá
        "\u001b[38;5;18m",  // modrá2
        "\u001b[38;5;19m",  // modrá3
        "\u001b[38;5;20m",  // modrá4
        "\u001b[38;5;21m",  // jasná modrá
        "\u001b[38;5;27m"   // světlejší azurová
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a gradient of red foreground colors for console output.
    /// </summary>
    /// <remarks>The array provides color codes ranging from dark red to orange-red, suitable for creating
    /// fade effects in text-based applications. The codes are intended for use in environments that support 256-color
    /// ANSI escape sequences, such as modern terminals.</remarks>
    public static string[] RedFade => new string[] {
        "\u001b[38;5;52m",  // tmavě červená
        "\u001b[38;5;88m",  // vínová
        "\u001b[38;5;124m", // červená
        "\u001b[38;5;160m", // jasná červená
        "\u001b[38;5;196m", // ohnivá červená
        "\u001b[38;5;202m"  // oranžovo-červená
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a gradient of purple colors for console text formatting.
    /// </summary>
    /// <remarks>The array contains color codes ranging from dark purple to neon purple, suitable for creating
    /// fade effects in console applications. The codes use 256-color ANSI sequences and can be combined to style text
    /// output with a purple gradient.</remarks>
    public static string[] PurpleFade => new string[] {
        "\u001b[38;5;55m",  // tmavě fialová
        "\u001b[38;5;91m",  // purpurová
        "\u001b[38;5;127m", // fialová
        "\u001b[38;5;163m", // jasná fialová
        "\u001b[38;5;199m", // růžovo-fialová
        "\u001b[38;5;201m"  // neon fialová
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a gradient of gray foreground colors, ranging from dark to
    /// light.
    /// </summary>
    /// <remarks>The array can be used to apply varying shades of gray to console text output, enabling visual
    /// effects such as fading or highlighting. The escape codes are ordered from the darkest shade to the
    /// lightest.</remarks>
    public static string[] GrayFade => new string[] {
        "\u001b[38;5;232m", // černá/velmi tmavě šedá
        "\u001b[38;5;236m", // tmavě šedá
        "\u001b[38;5;240m", // střední šedá
        "\u001b[38;5;244m", // světlejší šedá
        "\u001b[38;5;248m", // světlá šedá
        "\u001b[38;5;252m"  // skoro bílá
    };

    /// <summary>
    /// Gets an array of ANSI escape codes representing a sunset fade color gradient.
    /// </summary>
    /// <remarks>The array contains color codes that transition from dark purple to yellow, suitable for
    /// creating visually appealing text effects in console applications that support ANSI colors.</remarks>
    public static string[] SunsetFade => new string[] {
        "\u001b[38;5;55m",   // tmavě fialová
        "\u001b[38;5;91m",   // purpurová
        "\u001b[38;5;127m",  // růžová
        "\u001b[38;5;163m",  // světle růžová
        "\u001b[38;5;202m",  // oranžová
        "\u001b[38;5;226m"   // žlutá
    };
}
