using System.Collections.Generic;

namespace Mediavax.Core;

/// <summary>
/// Representing the YT-DLP info JSON object structure.
/// </summary>
public class YtDlpInfo
{
    /// <summary>
    /// Representing the id property.
    /// </summary>
    public string id { get; set; } = string.Empty;

    /// <summary>
    /// Representing the title property.
    /// </summary>
    public string title { get; set; } = string.Empty;

    /// <summary>
    /// Representing the list of available formats.
    /// </summary>
    public List<YtDlpFormat> formats { get; set; } = new();
}
