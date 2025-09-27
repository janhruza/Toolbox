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
}
