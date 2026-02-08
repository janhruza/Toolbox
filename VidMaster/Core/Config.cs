using Avalonia.Styling;

using System;
using System.IO;

namespace VidMaster.Core;

/// <summary>
/// Representing the app config class.
/// </summary>
public class Config
{
    /// <summary>
    /// Gets or sets the destination folder.
    /// </summary>
    public string SaveLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    /// <summary>
    /// Gets or sets the requested theme variant.
    /// </summary>
    public ThemeVariant Theme { get; set; } = ThemeVariant.Default;
}
