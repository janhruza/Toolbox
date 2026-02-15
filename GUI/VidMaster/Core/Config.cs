using System;
using System.IO;
using Avalonia.Styling;

namespace VidMaster.Core;

/// <summary>
///     Representing the app config class.
/// </summary>
public class Config
{
    /// <summary>
    ///     Gets or sets the destination folder.
    /// </summary>
    public string SaveLocation { get; set; } =
        Path.Combine(Environment.GetFolderPath(folder: Environment.SpecialFolder.MyVideos));

    /// <summary>
    ///     Gets or sets the requested theme variant.
    /// </summary>
    public ThemeVariant Theme { get; set; } = ThemeVariant.Default;
}