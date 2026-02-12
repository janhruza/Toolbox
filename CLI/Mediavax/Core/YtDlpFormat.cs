namespace Mediavax.Core;

/// <summary>
///     Representing the JSON format object structure.
/// </summary>
public class YtDlpFormat
{
    /// <summary>
    ///     Representing a value of no codec.
    /// </summary>
    public const string NO_CODEC = "none";

    /// <summary>
    ///     Representing the format_id property.
    /// </summary>
    public string format_id { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the ext property.
    /// </summary>
    public string ext { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the format_note property.
    /// </summary>
    public string format_note { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the audio codec property.
    /// </summary>
    public string acodec { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the video codec property.
    /// </summary>
    public string vcodec { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the width property.
    /// </summary>
    public int? width { get; set; }

    /// <summary>
    ///     Representing the height property.
    /// </summary>
    public int? height { get; set; }

    /// <summary>
    ///     Representing the filesize property.
    /// </summary>
    public long? filesize { get; set; }

    /// <summary>
    ///     Representing the url property.
    /// </summary>
    public string url { get; set; } = string.Empty;

    /// <summary>
    ///     Determines whether the format has video track.
    /// </summary>
    public bool HasVideo => this.vcodec != YtDlpFormat.NO_CODEC;

    /// <summary>
    ///     Determines whether the format has audio track.
    /// </summary>
    public bool HasAudio => this.acodec != YtDlpFormat.NO_CODEC;

    /// <summary>
    ///     Determines whether the format is not an actual media, but thumbnail or preview.
    /// </summary>
    public bool IsThumbnail => this.vcodec == "images";
}