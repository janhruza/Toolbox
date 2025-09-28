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

    internal const string BROWSER_BRAVE = "brave";
    internal const string BROWSER_CHROME = "chrome";
    internal const string BROWSER_CHROMIUM = "chromium";
    internal const string BROWSER_EDGE = "edge";
    internal const string BROWSER_FIREFOX = "firefox";
    internal const string BROWSER_OPERA = "opera";
    internal const string BROWSER_VIVALDI = "vivaldi";
    internal const string BROWSER_SAFARI = "safari";
    internal const string BROWSER_WHALE = "whale";

    /// <summary>
    /// Creates a new <see cref="MediaItem"/> instance with default parameters.
    /// </summary>
    public MediaItem()
    {
        Address = string.Empty;
        Mode = string.Empty;
        Quality = string.Empty;
        Format = string.Empty;
        Location = string.Empty;
        BrowserCookies = string.Empty;
        CustomOptions = string.Empty;
        Current = this;
    }

    /// <summary>
    /// Representing the media source (URL address).
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Representing the download mode: video-only, audio-only or both.
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// Representing the download quality.
    /// </summary>
    public string Quality { get; set; }

    /// <summary>
    /// Representing the download format, such as: MP4, MP3, etc.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Representing a directory for the downloaded media.
    /// Default is the user's Downloads directory.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Representing a browser to import cookies from.
    /// Can be <see cref="string.Empty"/>.
    /// </summary>
    public string BrowserCookies { get; set; }

    /// <summary>
    /// Representing additional command line arguments for the YT-DLP.
    /// </summary>
    public string CustomOptions { get; set; }

    /// <summary>
    /// Determines whether the item can be downloaded.
    /// </summary>
    /// <returns><see langword="true"/> if all the required fields are filled with data, otherwise <see langword="false"/>.</returns>
    public bool IsValid()
    {
        return string.IsNullOrEmpty(Address) == false;
    }

    /// <summary>
    /// Representing the curremt <see cref="MediaItem"/> instance.
    /// </summary>
    public static MediaItem Current { get; set; }
}
