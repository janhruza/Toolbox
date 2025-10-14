using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cashly.Core;

/// <summary>
/// Represents the JSON serialization context for the <see cref="Cashly.Core.UserProfile"/> class.
/// </summary>
[JsonSerializable(typeof(UserProfile))]
public partial class UserProfileTypeInfo : JsonSerializerContext;

/// <summary>
/// Represents a user profile in the Cashly application.
/// </summary>
public class UserProfile
{
    /// <summary>
    /// Creates a new instance of the <see cref="UserProfile"/> class.
    /// </summary>
    public UserProfile()
    {
        Id = Guid.CreateVersion7();
        Name = Environment.UserName;
        Transactions = new List<Transaction>();
    }

    /// <summary>
    /// Representing the unique identifier of the user profile.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Representing the name of the user profile,
    /// <see cref="Environment.UserName"/> by default.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Representing the list of all transactions associated with the user profile.
    /// This list contains both income and expense transactions.
    /// </summary>
    public List<Transaction> Transactions { get; }

    #region Static code

    /// <summary>
    /// Attempts to save the user profile to a file in JSON format.
    /// </summary>
    /// <param name="profile">Input user profile object.</param>
    /// <returns><see langword="true"/> if the saving is successful, otherwise <see langword="false"/>.</returns>
    public static bool Save(UserProfile profile)
    {
        // save profile to file (JSON)
        return true;
    }

    /// <summary>
    /// Atempts to load the user profile from a file in JSON format.
    /// </summary>
    /// <param name="filename">Input file name.</param>
    /// <param name="profile">Output user profile object.</param>
    /// <returns><see langword="true"/> if the saving is successful, otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// If the method fails, the <paramref name="profile"/> parameter will be set to a new instance of the <see cref="UserProfile"/> class with default values.
    /// </remarks>
    public static bool Load(string filename, out UserProfile profile)
    {
        // load profile from file (JSON)
        profile = new UserProfile();
        return true;
    }

    #endregion
}
