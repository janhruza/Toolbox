using System;
using System.Text.Json;
using Toolbox;

namespace Mediavax.Core;

/// <summary>
///     Representing the class responsible for parsing YT-DLP JSON output objects.
/// </summary>
public static class YtDlpParser
{
    /// <summary>
    ///     Attempts to parse the info about the selected media source and its formats.
    /// </summary>
    /// <param name="json">Input JSON data.</param>
    /// <param name="info">Output <see cref="YtDlpInfo" /> object.</param>
    /// <returns>Operation result.</returns>
    public static bool GetInfo(string json, out YtDlpInfo info)
    {
        try
        {
            info = new YtDlpInfo();
            if (string.IsNullOrWhiteSpace(value: json)) return false;

            YtDlpInfo? result =
                JsonSerializer.Deserialize<YtDlpInfo>(json: json, jsonTypeInfo: YtDlpJsonContext.Default.YtDlpInfo);
            if (result == null) return false;

            info = result;
            return true;
        }

        catch (Exception ex)
        {
            info = new YtDlpInfo();
            _ = Log.Exception(exception: ex, tag: nameof(YtDlpParser.GetInfo));
            return false;
        }
    }
}