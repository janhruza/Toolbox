using Cashly.Core;

namespace Cashly;

/// <summary>
/// Represents the current user session in the Cashly application.
/// </summary>
public static class Session
{
    /// <summary>
    /// Representing the current user profile in the session.
    /// </summary>
    public static UserProfile Profile { get; set; } = new UserProfile();

    /// <summary>
    /// Determines whether a profile is loaded in the current session.
    /// </summary>
    public static bool ProfileLoaded;
}
