using System;
using System.Text.Json;

using Toolbox;

namespace Mediavax.Core;

/// <summary>
/// Representing the class responsible for parsing YT-DLP JSON output objects.
/// </summary>
public static class YtDlpParser
{
    /// <summary>
    /// Attempts to parse the info about the selected media source and its formats.
    /// </summary>
    /// <param name="json">Input JSON data.</param>
    /// <param name="info">Output <see cref="YtDlpInfo"/> object.</param>
    /// <returns>Operation result.</returns>
    public static bool GetInfo(string json, out YtDlpInfo info)
    {
        try
        {
            info = new YtDlpInfo();
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            var result = JsonSerializer.Deserialize<YtDlpInfo>(json, YtDlpJsonContext.Default.YtDlpInfo);
            if (result == null)
            {
                return false;
            }

            info = result;
            return true;
        }

        catch (Exception ex)
        {
            info = new YtDlpInfo();
            _ = Log.Exception(ex, nameof(GetInfo));
            return false;
        }
    }
}
