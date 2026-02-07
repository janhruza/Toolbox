using System.Collections.Generic;

namespace RSShell.Core;

/// <summary>
/// Representing a RSS channel.
/// </summary>
public struct RssChannel
{
    /// <summary>
    /// Representing the channel's title.
    /// </summary>
    public string Title;

    /// <summary>
    /// Representing the channel's link.
    /// </summary>
    public string Link;

    /// <summary>
    /// Representing the channel's description.
    /// </summary>
    public string Description;

    /// <summary>
    /// Representing the list of channel items.
    /// </summary>
    public List<RssItem> Items;
}
