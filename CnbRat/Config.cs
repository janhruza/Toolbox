using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using Toolbox;
using Toolbox.UI;

namespace CnbRat;

/// <summary>
/// Representing the configuration class.
/// </summary>
public class Config : Toolbox.UI.IConfig
{
    /// <summary>
    /// Creates a new <see cref="Config"/> instance.
    /// </summary>
    public Config()
    {
        AccentTextStyle = "\e[38;5;76m";
        AccentHighlightStyle = "\e[48;5;40m\e[38;5;0m";
        Colors = new ColorScheme();
    }

    /// <summary>
    /// Representing a style of the custom colored text, such as captions, keywords and other, non-highlighted, elements,
    /// </summary>
    public string AccentTextStyle { get; set; }

    /// <summary>
    /// Representing a style of the highlighted items - such as highlighted menu item.
    /// </summary>
    public string AccentHighlightStyle { get; set; }

    /// <summary>
    /// Representing a custom color scheme definition.
    /// </summary>
    public ColorScheme Colors { get; set; }

    #region Static code

    private const string DEFAULT_CONFIG_FILE = "config.json";

    /// <summary>
    /// Attempts to load the default configuration from the predefined configuration file.
    /// </summary>
    /// <remarks>If the configuration file does not exist or an error occurs during loading, a new default
    /// configuration is provided and the method returns false.</remarks>
    /// <param name="config">When this method returns, contains the loaded configuration if successful; otherwise, contains a new default
    /// configuration instance.</param>
    /// <returns>true if the default configuration was loaded successfully; otherwise, false.</returns>
    public static bool LoadDefault(out Config config)
    {
        Config defCfg = new Config();
        if (File.Exists(DEFAULT_CONFIG_FILE) == false)
        {
            config = defCfg;
            return true;
        }

        try
        {
            string json = File.ReadAllText(DEFAULT_CONFIG_FILE, Resources.Encoding);
            config = JsonSerializer.Deserialize(json, ConfigInfo.Default.Config) ?? new Config();
            _ = Log.Information("Config loaded.", nameof(LoadDefault));
            return true;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(ex, nameof(LoadDefault));
            config = defCfg;
            return false;
        }
    }

    /// <summary>
    /// Attempts to write the specified configuration to the default configuration file in JSON format.
    /// </summary>
    /// <remarks>If an error occurs during serialization or file writing, the method logs the exception and
    /// returns false. The configuration is saved using the default encoding specified in the resources.</remarks>
    /// <param name="config">The configuration object to be serialized and saved to the default configuration file.</param>
    /// <returns>true if the configuration was successfully written; otherwise, false.</returns>
    public static bool WriteDefault(Config config)
    {
        try
        {
            string json = JsonSerializer.Serialize(config, ConfigInfo.Default.Config);
            File.WriteAllText(DEFAULT_CONFIG_FILE, json, Resources.Encoding);
            _ = Log.Information("Default config saved.", nameof(WriteDefault));
            return true;
        }
        catch (Exception ex)
        {
            _ = Log.Exception(ex, nameof(WriteDefault));
            return false;
        }
    }

    #endregion
}

/// <summary>
/// Describes the serialization options for the <see cref="CnbRat.Config"/> class.
/// </summary>
[JsonSerializable(typeof(Config))]
public partial class ConfigInfo : JsonSerializerContext
{
}