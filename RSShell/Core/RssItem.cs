namespace RSShell.Core;

/// <summary>
/// Representing a single RSS feed item.
/// </summary>
public struct RssItem
{
    /// <summary>
    /// Representing the item's title.
    /// </summary>
    public string Title;

    /// <summary>
    /// Representing the item's description.
    /// </summary>
    public string Description;

    /// <summary>
    /// Representing the item's link.
    /// </summary>
    public string Link;

    /// <summary>
    /// Representing the author of the item.
    /// </summary>
    public string Author;

    /// <summary>
    /// Representing the publication date and time of the item.
    /// </summary>
    public string Date;

    /// <summary>
    /// Representing the identification of the item.
    /// </summary>
    public string Guid;
}
