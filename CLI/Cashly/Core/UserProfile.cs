using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Toolbox;

// <-- Tento using je klíčový

namespace Cashly.Core;

/// <summary>
///     Represents the JSON serialization context for the <see cref="Cashly.Core.UserProfile" /> class.
/// </summary>
[JsonSerializable(type: typeof(UserProfile))]
public partial class UserProfileTypeInfo : JsonSerializerContext;

/// <summary>
///     Represents a user profile in the Cashly application.
/// </summary>
public class UserProfile
{
    /// <summary>
    ///     Creates a new instance of the <see cref="UserProfile" /> class.
    /// </summary>
    public UserProfile()
    {
        this.Id = Guid.CreateVersion7();
        this.Created = DateTime.Now;
        this.Name = Environment.UserName;
        this.Transactions = new List<Transaction>();
        this.TransactionCategories = new List<string>();
    }

    /// <summary>
    ///     Representing the unique identifier of the user profile.
    /// </summary>
    [JsonInclude] // <-- OPRAVA: Povolí deserializaci této vlastnosti
    public Guid Id { get; internal set; }

    /// <summary>
    ///     Representing the name of the user profile,
    ///     <see cref="Environment.UserName" /> by default.
    /// </summary>
    public string Name { get; set; } // <-- Tato byla v pořádku (public set)

    /// <summary>
    ///     Representing the list of all transactions associated with the user profile.
    ///     This list contains both income and expense transactions.
    /// </summary>
    [JsonInclude] // <-- OPRAVA: Povolí deserializaci této vlastnosti
    public List<Transaction> Transactions { get; internal set; }

    /// <summary>
    ///     Representing the date and time when this profile was created.
    /// </summary>
    [JsonInclude] // <-- OPRAVA: Povolí deserializaci této vlastnosti
    public DateTime Created { get; internal set; } // <-- OPRAVA: Přidán 'internal set'

    #region Methods

    private decimal GetBalance()
    {
        decimal sum = 0m;
        foreach (Transaction transaction in this.Transactions)
            switch (transaction.Type)
            {
                default:
                case TransactionType.Undefined: break;

                case TransactionType.Income:
                    sum += transaction.Amount;
                    break;

                case TransactionType.Expense:
                    sum -= transaction.Amount;
                    break;
            }

        return sum;
    }

    #endregion

    #region Get only properties

    /// <summary>
    ///     Gets the user's current balance.
    /// </summary>
    public decimal Balance => this.GetBalance();

    /// <summary>
    ///     Representing a list of income-only transactions.
    /// </summary>
    public List<Transaction> Incomes =>
        this.Transactions.Where(predicate: x => x.Type == TransactionType.Income).ToList();

    /// <summary>
    ///     Representing a list of expanse-only transactions.
    /// </summary>
    public List<Transaction> Expanses =>
        this.Transactions.Where(predicate: x => x.Type == TransactionType.Expense).ToList();

    /// <summary>
    ///     Representing a list of user-defined transaction categories.
    /// </summary>
    [JsonInclude] // <-- OPRAVA: Povolí deserializaci této vlastnosti
    public List<string> TransactionCategories { get; internal set; }

    #endregion

    #region Static code

    /// <summary>
    ///     Representing a default, invalid user profile structure.
    /// </summary>
    public static readonly UserProfile Empty = new()
    {
        Id = Guid.Empty,
        Transactions = new List<Transaction>(),
        Name = string.Empty,
        TransactionCategories = new List<string>()
    };

    private static string ProfilesFolder =>
        Path.Combine(path1: AppDomain.CurrentDomain.BaseDirectory, path2: "Profiles");

    /// <summary>
    ///     Attempts to save the user profile to a file in JSON format.
    /// </summary>
    /// <param name="profile">Input user profile object.</param>
    /// <returns><see langword="true" /> if the saving is successful, otherwise <see langword="false" />.</returns>
    /// <remarks>
    ///     This method saves the profile to a file named with the profile's <see cref="Id" /> in the "Profiles" directory
    ///     located in the application's base directory.
    ///     The format of the name is "{Id}.json" where Id is a hexadecimal repesentation of the Id property.
    ///     Uses the <see cref="Encoding.UTF8" /> encoding to write the file.
    /// </remarks>
    public static bool Save(UserProfile profile)
    {
        // save profile to file (JSON)
        if (!Directory.Exists(path: UserProfile.ProfilesFolder))
            if (!Directory.CreateDirectory(path: UserProfile.ProfilesFolder).Exists)
            {
                _ = Log.Error(message: "Failed to create profiles folder.", tag: nameof(UserProfile.Save));
                return false;
            }

        if (profile.Id == Guid.Empty) profile.Id = Guid.CreateVersion7();

        // create file name (hex representation of the Id property)
        string filename = Path.Combine(path1: UserProfile.ProfilesFolder,
            path2: $"{Convert.ToHexString(inArray: profile.Id.ToByteArray())}.json");
        string json = JsonSerializer.Serialize(value: profile, jsonTypeInfo: UserProfileTypeInfo.Default.UserProfile);

        // write to file
        File.WriteAllText(path: filename, contents: json, encoding: Encoding.UTF8);
        _ = Log.Information(message: "User profile saved successfully.", tag: nameof(UserProfile.Save));

        return true;
    }

    /// <summary>
    ///     Atempts to load the user profile from a file in JSON format.
    /// </summary>
    /// <param name="filename">Input file name.</param>
    /// <param name="profile">Output user profile object.</param>
    /// <returns><see langword="true" /> if the saving is successful, otherwise <see langword="false" />.</returns>
    /// <remarks>
    ///     If the method fails, the <paramref name="profile" /> parameter will be set to a new instance of the
    ///     <see cref="UserProfile" /> class with default values.
    ///     The <paramref name="filename" /> parameter should be a full path to the file.
    ///     Uses the <see cref="Encoding.UTF8" /> encoding to read the file.
    /// </remarks>
    public static bool Load(string filename, out UserProfile profile)
    {
        // load profile from file (JSON)
        profile = new UserProfile(); // <-- Toto je v pořádku, ale bude přepsáno

        if (!File.Exists(path: filename))
        {
            _ = Log.Error(message: "Profile file doesn't exist.", tag: nameof(UserProfile.Load));
            return false;
        }

        string json = File.ReadAllText(path: filename, encoding: Encoding.UTF8);

        // Díky [JsonInclude] teď 'loadedObject' bude mít správné Id, transakce atd.
        UserProfile? loadedObject =
            JsonSerializer.Deserialize(json: json, jsonTypeInfo: UserProfileTypeInfo.Default.UserProfile);
        if (loadedObject == null)
        {
            _ = Log.Error(message: "Failed to deserialize the profile file.", tag: nameof(UserProfile.Load));
            return false;
        }

        profile = loadedObject; // 'profile' teď obsahuje správně načtená data
        _ = Log.Information(message: "User profile loaded successfully.", tag: nameof(UserProfile.Load));
        return true;
    }

    /// <summary>
    ///     Enumerates all available user profiles by listing all files in the default "Profiles" directory.
    /// </summary>
    /// <param name="profiles">
    ///     Output list of profile file names (full paths).
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the operation is successful, otherwise <see langword="false" />.
    /// </returns>
    public static bool ListProfiles(out List<string> profiles)
    {
        profiles = [];

        if (!Directory.Exists(path: UserProfile.ProfilesFolder))
        {
            _ = Log.Error(message: "Profiles folder doesn't exist.", tag: nameof(UserProfile.ListProfiles));
            return false;
        }

        foreach (string file in Directory.EnumerateFiles(path: UserProfile.ProfilesFolder, searchPattern: "*.json",
                     searchOption: SearchOption.TopDirectoryOnly))
            profiles.Add(item: file);

        _ = Log.Information(message: $"Found {profiles.Count} profile(s).", tag: nameof(UserProfile.ListProfiles));
        return true;
    }

    /// <summary>
    ///     Enumerates all profiles in the default "Profiles" directory.
    /// </summary>
    /// <param name="profiles">Output list of available profiles.</param>
    /// <returns><see langword="true" /> if the operation is successful, otherwise <see langword="false" />.</returns>
    public static bool EnumProfiles(out List<UserProfile> profiles)
    {
        profiles = new List<UserProfile>();
        if (UserProfile.ListProfiles(profiles: out List<string> files))
        {
            foreach (string file in files)
                if (UserProfile.Load(filename: file, profile: out UserProfile profile))
                    profiles.Add(item: profile);

            return true;
        }

        return false;
    }

    #endregion
}