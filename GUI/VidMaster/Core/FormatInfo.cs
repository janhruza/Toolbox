using System.Text.Json.Serialization;

namespace VidMaster.Core;

/// <summary>
///     Representing a media format information.
/// </summary>
public class FormatInfo
{
    /// <summary>
    ///     Representing the format ID.
    /// </summary>
    [JsonPropertyName(name: "format_id")]
    public string FormatId { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the file extension.
    /// </summary>
    [JsonPropertyName(name: "ext")]
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    ///     Representing the resolution in the {width}x{height} format.
    /// </summary>
    [JsonPropertyName(name: "resolution")]
    public string? Resolution { get; set; }

    /// <summary>
    ///     Representing the video codec.
    /// </summary>
    [JsonPropertyName(name: "vcodec")]
    public string? VideoCodec { get; set; }

    /// <summary>
    ///     Representing the audio codec.
    /// </summary>
    [JsonPropertyName(name: "acodec")]
    public string? AudioCodec { get; set; }

    /// <summary>
    ///     Representing the file size.
    /// </summary>
    [JsonPropertyName(name: "filesize")]
    public long? FileSize { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.FormatId}: {this.Resolution ?? "No video"}, {this.Extension}";
    }
}