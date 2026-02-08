using System.Text.Json.Serialization;

namespace VidMaster.Core;

/// <summary>
/// Representing a media format information.
/// </summary>
public class FormatInfo
{
    /// <summary>
    /// Representing the format ID.
    /// </summary>
    [JsonPropertyName("format_id")]
    public string FormatId { get; set; } = string.Empty;

    /// <summary>
    /// Representing the file extension.
    /// </summary>
    [JsonPropertyName("ext")]
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Representing the resolution in the {width}x{height} format.
    /// </summary>
    [JsonPropertyName("resolution")]
    public string? Resolution { get; set; }

    /// <summary>
    /// Representing the video codec.
    /// </summary>
    [JsonPropertyName("vcodec")]
    public string? VideoCodec { get; set; }

    /// <summary>
    /// Representing the audio codec.
    /// </summary>
    [JsonPropertyName("acodec")]
    public string? AudioCodec { get; set; }

    /// <summary>
    /// Representing the file size.
    /// </summary>
    [JsonPropertyName("filesize")]
    public long? FileSize { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{FormatId}: {Resolution ?? "No video"}, {Extension}";
    }
}
