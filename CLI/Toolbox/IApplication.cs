using System;

namespace Toolbox;

/// <summary>
/// Representing the interface for all Toolbox applications.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Displays the main application banner.
    /// </summary>
    static abstract void DisplayBanner();

    /// <summary>
    /// Performs the post-exit cleanup.
    /// </summary>
    static abstract void PostExitCleanup();

    /// <summary>
    /// Loads the application config (if any) and applies it to the application. For example, set accent colors, modify default styles, etc.
    /// </summary>
    static abstract void LoadConfig();

    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    static abstract Version Version { get; }
}
