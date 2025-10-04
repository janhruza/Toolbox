namespace Mediavax.Core;

/// <summary>
/// Representing the JSON format object structure.
/// </summary>
public class YtDlpFormat
{
    /// <summary>
    /// Representing the format_id property.
    /// </summary>
    public string format_id { get; set; } = string.Empty;

    /// <summary>
    /// Representing the ext property.
    /// </summary>
    public string ext { get; set; } = string.Empty;

    /// <summary>
    /// Representing the format_note property.
    /// </summary>
    public string format_note { get; set; } = string.Empty;

    /// <summary>
    /// Representing the audio codec property.
    /// </summary>
    public string acodec { get; set; } = string.Empty;

    /// <summary>
    /// Representing the video codec property.
    /// </summary>
    public string vcodec { get; set; } = string.Empty;

    /// <summary>
    /// Representing the width property.
    /// </summary>
    public int? width { get; set; }

    /// <summary>
    /// Representing the height property.
    /// </summary>
    public int? height { get; set; }

    /// <summary>
    /// Representing the filesize property.
    /// </summary>
    public long? filesize { get; set; }

    /// <summary>
    /// Representing the url property.
    /// </summary>
    public string url { get; set; } = string.Empty;
}