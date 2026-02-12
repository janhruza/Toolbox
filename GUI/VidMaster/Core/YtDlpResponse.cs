using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VidMaster.Core;

/// <summary>
///     Custom JSON context.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(type: typeof(YtDlpResponse))]
[JsonSerializable(type: typeof(FormatInfo))]
public partial class AppJsonContext : JsonSerializerContext
{
}

/// <summary>
///     Representing the yt-dlp response.
/// </summary>
public class YtDlpResponse
{
    /// <summary>
    ///     Representing the list of all fetched formats.
    /// </summary>
    [JsonPropertyName(name: "formats")]
    public List<FormatInfo> Formats { get; set; } = [];
}