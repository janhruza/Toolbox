using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Toolbox;
using Toolbox.UI;

namespace CnbRat;

/// <summary>
///     Representing the configuration class.
/// </summary>
public class Config : IConfig
{
    /// <summary>
    ///     Creates a new <see cref="Config" /> instance.
    /// </summary>
    public Config()
    {
        this.AccentTextStyle = "\e[38;5;76m";
        this.AccentHighlightStyle = "\e[48;5;40m\e[38;5;0m";
        this.Colors = new ColorScheme();
    }

    /// <summary>
    ///     Representing a custom color scheme definition.
    /// </summary>
    public ColorScheme Colors { get; set; }

    /// <summary>
    ///     Representing a style of the custom colored text, such as captions, keywords and other, non-highlighted, elements,
    /// </summary>
    public string AccentTextStyle { get; set; }

    /// <summary>
    ///     Representing a style of the highlighted items - such as highlighted menu item.
    /// </summary>
    public string AccentHighlightStyle { get; set; }

    #region Static code

    private const string DEFAULT_CONFIG_FILE = "config.json";

    /// <summary>
    ///     Attempts to load the default configuration from the predefined configuration file.
    /// </summary>
    /// <remarks>
    ///     If the configuration file does not exist or an error occurs during loading, a new default
    ///     configuration is provided and the method returns false.
    /// </remarks>
    /// <param name="config">
    ///     When this method returns, contains the loaded configuration if successful; otherwise, contains a new default
    ///     configuration instance.
    /// </param>
    /// <returns>true if the default configuration was loaded successfully; otherwise, false.</returns>
    public static bool LoadDefault(out Config config)
    {
        Config defCfg = new();
        if (!File.Exists(path: Config.DEFAULT_CONFIG_FILE))
        {
            config = defCfg;
            return true;
        }

        try
        {
            string json = File.ReadAllText(path: Config.DEFAULT_CONFIG_FILE, encoding: Resources.Encoding);
            config = JsonSerializer.Deserialize(json: json, jsonTypeInfo: ConfigInfo.Default.Config) ?? new Config();
            _ = Log.Information(message: "Config loaded.", tag: nameof(Config.LoadDefault));
            return true;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex, tag: nameof(Config.LoadDefault));
            config = defCfg;
            return false;
        }
    }

    /// <summary>
    ///     Attempts to write the specified configuration to the default configuration file in JSON format.
    /// </summary>
    /// <remarks>
    ///     If an error occurs during serialization or file writing, the method logs the exception and
    ///     returns false. The configuration is saved using the default encoding specified in the resources.
    /// </remarks>
    /// <param name="config">The configuration object to be serialized and saved to the default configuration file.</param>
    /// <returns>true if the configuration was successfully written; otherwise, false.</returns>
    public static bool WriteDefault(Config config)
    {
        try
        {
            string json = JsonSerializer.Serialize(value: config, jsonTypeInfo: ConfigInfo.Default.Config);
            File.WriteAllText(path: Config.DEFAULT_CONFIG_FILE, contents: json, encoding: Resources.Encoding);
            _ = Log.Information(message: "Default config saved.", tag: nameof(Config.WriteDefault));
            return true;
        }
        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex, tag: nameof(Config.WriteDefault));
            return false;
        }
    }

    #endregion
}

/// <summary>
///     Describes the serialization options for the <see cref="CnbRat.Config" /> class.
/// </summary>
[JsonSerializable(type: typeof(Config))]
public partial class ConfigInfo : JsonSerializerContext
{
}