using System;
using System.IO;
using System.Text;
using Toolbox;

namespace Mediavax.Core;

/// <summary>
///     Representing a single media item.
///     This item contains information about the media which will be downloaded using the yt-dlp tool.
/// </summary>
public class MediaItem
{
    internal const string BROWSER_BRAVE = "brave";
    internal const string BROWSER_CHROME = "chrome";
    internal const string BROWSER_CHROMIUM = "chromium";
    internal const string BROWSER_EDGE = "edge";
    internal const string BROWSER_FIREFOX = "firefox";
    internal const string BROWSER_OPERA = "opera";
    internal const string BROWSER_VIVALDI = "vivaldi";
    internal const string BROWSER_SAFARI = "safari";
    internal const string BROWSER_WHALE = "whale";

    static MediaItem()
    {
        MediaItem.Current ??= new MediaItem();
    }

    /// <summary>
    ///     Creates a new <see cref="MediaItem" /> instance with default parameters.
    /// </summary>
    public MediaItem()
    {
        this.Address = string.Empty;
        this.Format = string.Empty;
        this.Location = Path.Combine(path1: Environment.GetFolderPath(folder: Environment.SpecialFolder.UserProfile),
            path2: "Downloads");
        this.BrowserCookies = string.Empty;
        this.CustomOptions = string.Empty;
        MediaItem.Current = this;
    }

    /// <summary>
    ///     Representing the media source (URL address).
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///     Representing the download format, such as: MP4, MP3, etc.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    ///     Representing a directory for the downloaded media.
    ///     Default is the user's Downloads directory.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    ///     Representing a browser to import cookies from.
    ///     Can be <see cref="string.Empty" />.
    /// </summary>
    public string BrowserCookies { get; set; }

    /// <summary>
    ///     Representing additional command line arguments for the YT-DLP.
    /// </summary>
    public string CustomOptions { get; set; }

    /// <summary>
    ///     Representing the curremt <see cref="MediaItem" /> instance.
    /// </summary>
    public static MediaItem Current { get; set; }

    /// <summary>
    ///     Determines whether the item can be downloaded.
    /// </summary>
    /// <returns><see langword="true" /> if all the required fields are filled with data, otherwise <see langword="false" />.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(value: this.Address);
    }

    /// <summary>
    ///     Builds the final command for yt-dlp.
    /// </summary>
    /// <returns></returns>
    public string BuildCommand()
    {
        if (this.Address.IsEmpty()) return string.Empty;

        StringBuilder sb = new();
        _ = sb.Append(value: this.Address);

        if (!this.Format.IsEmpty())
            // custom format
            _ = sb.Append(handler: $" -f {this.Format}");

        if (Directory.Exists(path: this.Location))
            // custom download folder
            _ = sb.Append(handler: $" -o {Path.Combine(path1: this.Location, path2: "%(title)s.%(ext)s")}");

        if (!this.BrowserCookies.IsEmpty())
            // import cookies
            _ = sb.Append(handler: $" --cookies-from-browser {this.BrowserCookies}");

        if (!this.CustomOptions.IsEmpty())
            // additional custom arguments
            _ = sb.Append(handler: $" {this.CustomOptions}");

        return sb.ToString();
    }
}