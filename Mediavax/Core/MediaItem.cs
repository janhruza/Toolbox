namespace Mediavax.Core;

/// <summary>
/// Representing a single media item.
/// This item contains information about the media which will be downloaded using the yt-dlp tool.
/// </summary>
public class MediaItem
{
    static MediaItem()
    {
        Current ??= new MediaItem();
    }

    /// <summary>
    /// Creates a new <see cref="MediaItem"/> instance with default parameters.
    /// </summary>
    public MediaItem()
    {
        Address = string.Empty;
        Current = this;
    }

    /// <summary>
    /// Representing the media source (URL address).
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Representing the curremt <see cref="MediaItem"/> instance.
    /// </summary>
    public static MediaItem Current { get; set; }
}
