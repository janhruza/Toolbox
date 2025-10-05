using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Toolbox;

namespace RSShell;

/// <summary>
/// Representing the type descriptor of the <see cref="RSShell.Config"/> type for the JSON serialization.
/// </summary>
[JsonSerializable(typeof(Config))]
public partial class ConfigContext : JsonSerializerContext
{
}

/// <summary>
/// Representing the RSShell config file.
/// </summary>
public class Config
{
    /// <summary>
    /// Creates an empty instance of the <see cref="Config"/> class.
    /// </summary>
    public Config()
    {
        Feeds = [];
    }

    /// <summary>
    /// Representing the list of saved RSS feed sources.
    /// </summary>
    public List<string> Feeds { get; set; }

    #region Static code

    static string Path => "settings.json";

    /// <summary>
    /// Saves the configuration file.
    /// </summary>
    /// <param name="config">Config to be saved.</param>
    /// <returns>operation result.</returns>
    public static bool Save(Config config)
    {
        try
        {
            string data = JsonSerializer.Serialize(config, ConfigContext.Default.Config);
            File.WriteAllText(Path, data, Encoding.Unicode);
            return true;
        }

        catch (Exception ex)
        {
            Log.Exception(ex, nameof(Save));
            return false;
        }
    }

    /// <summary>
    /// Loads the curremtly saved configuration file.
    /// </summary>
    /// <param name="config">Config object (if any).</param>
    /// <returns>Operation result.</returns>
    public static bool Load(out Config config)
    {
        config = new Config();

        try
        {
            if (File.Exists(Path) == false)
            {
                return false;
            }

            string data = File.ReadAllText(Path, Encoding.Unicode);
            config = (Config)JsonSerializer.Deserialize<Config>(data) ?? new Config();
            return true;
        }

        catch (Exception ex)
        {
            Log.Exception(ex, nameof(Load));
            return false;
        }
    }

    /// <summary>
    /// Representing the current config instance.
    /// </summary>
    public static Config Current { get; set; } = new Config();

    #endregion
}
