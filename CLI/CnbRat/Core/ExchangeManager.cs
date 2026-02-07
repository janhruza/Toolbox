using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Toolbox;

namespace CnbRat.Core;

/// <summary>
/// Representing the exchange manager class.
/// </summary>
public class ExchangeManager
{
    /// <summary>
    /// Creates a new <see cref="ExchangeManager"/> instance.
    /// </summary>
    public ExchangeManager()
    {
        this._rates = [];
        this._ready = false;
        this._rawData = string.Empty;
    }

    /// <summary>
    /// Gets the full path to the application's archive directory.
    /// </summary>
    /// <remarks>The archive directory is located within the application's base directory and is intended for
    /// storing archived files or data. The exact location is determined at runtime based on the application's current
    /// domain.</remarks>
    public static string ArchiveDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Resources.ARCHIVE_DIRECTORY);

    private bool ParseData(string sourceData)
    {
        // check data validity
        if (string.IsNullOrWhiteSpace(sourceData) == true)
        {
            // invalid data
            _ = Log.Error("Invalid data.", nameof(Fetch));
            return false;
        }

        // split the data in lines
        string[] linesData = sourceData.Split('\n', StringSplitOptions.TrimEntries);

        // check if the lines are valid
        if (linesData.Length < 2)
        {
            // no data
            _ = Log.Error("Fetched data contain no valid lines.", nameof(Fetch));
            return false;
        }

        // process the data lines
        // get date and release
        string[] metadata = linesData[0].Split('#', StringSplitOptions.TrimEntries);
        if (metadata.Length != 2)
        {
            // invalid metadata
            _ = Log.Error("Invalid metadata.", nameof(Fetch));
            return false;
        }

        string[] dateInfo = metadata[0].Split('.', StringSplitOptions.TrimEntries);
        if (dateInfo.Length != 3)
        {
            _ = Log.Error("Invalid date format.", nameof(Fetch));
            return false;
        }

        // get the metadata
        int day = Convert.ToInt32(dateInfo[0]);
        int month = Convert.ToInt32(dateInfo[1]);
        int year = Convert.ToInt32(dateInfo[2]);
        int release = Convert.ToInt32(metadata[1]);

        // assign the metadata
        this._info = new ExchangeInfo
        {
            Date = new DateOnly(year, month, day),
            Release = release
        };

        // process individual exchange rate entries
        for (int x = 2; x < linesData.Length; x++)
        {
            // split the columns
            string[] entry = linesData[x].Split('|', StringSplitOptions.TrimEntries);

            // check data size
            if (entry.Length != 5) continue;

            RateInfo rate = new RateInfo
            {
                Country = entry[0],
                Currency = entry[1],
                Amount = Convert.ToInt32(entry[2]),
                Code = entry[3],
                Value = Convert.ToDecimal(entry[4].Replace(',', '.')) // convert decimal commas to periods
            };

            // add the rate to the list
            this._rates.Add(rate);
        }

        // add the CZK entry manually
        RateInfo czk = new RateInfo
        {
            Country = "Česká republika",
            Currency = "koruna",
            Amount = 1,
            Code = "CZK",
            Value = 1.0m
        };
        this._rates.Add(czk);

        this._ready = true;
        return true;
    }

    /// <summary>
    /// Attempts to fetch the current exchange records from the official source defined in <see cref="Resources.URL_EXCHANGE_RATES"/>.
    /// </summary>
    /// <param name="date">
    /// The specific date for which to fetch exchange rates. Use <see cref="DateOnly.MinValue"/> to fetch the latest available rates.
    /// </param>
    /// <returns><see langword="true"/> if the fetch was successful, otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// If this method succeeds, the resulting properties can be found in the <see cref="Exchange"/> and <see cref="Rates"/> properties.
    /// </remarks>
    public bool Fetch(DateOnly date)
    {
        try
        {
            // empty the default data
            this._info = default;
            this._rates.Clear();
            this._rawData = string.Empty;

            string requestUri;
            if (date == DateOnly.MinValue)
            {
                // fetch the latest data
                requestUri = Resources.URL_EXCHANGE_RATES;
            }
            else
            {
                // fetch historical data
                requestUri = Resources.URL_EXCHANGE_RATES_HISTORICAL
                    .Replace("{day}", date.Day.ToString("D2"))
                    .Replace("{month}", date.Month.ToString("D2"))
                    .Replace("{year}", date.Year.ToString("D4"));
            }

            string data = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                var task = client.GetStringAsync(new Uri(requestUri, UriKind.RelativeOrAbsolute));
                task.Wait();

                // check if the task has faulted
                if (task.IsFaulted)
                {
                    _ = Log.Error("Fetching task faulted.", nameof(Fetch));
                    return false;
                }

                // assign read data
                data = task.Result;
                this._rawData = task.Result;
            }

            // parse the recieved data
            return ParseData(data);
        }

        catch (Exception ex)
        {
            _ = Log.Exception(ex, nameof(Fetch));
            return false;
        }
    }

    /// <summary>
    /// Attempts to archive the currently fetched exchange records to a local file defined in <see cref="Resources.ARCHIVE_DIRECTORY"/>.
    /// </summary>
    /// <returns></returns>
    public bool ArchiveReport()
    {
        try
        {
            // output file name
            string path = Path.Combine(ArchiveDirectory, $"{Exchange.Date.ToString("yyyyMMdd")}{Exchange.Release}.txt");

            if (Directory.Exists(Path.GetDirectoryName(path)) == false)
            {
                string dir = Path.GetDirectoryName(path) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(dir) == true)
                {
                    _ = Log.Error("Unable to determine archive directory.", nameof(ArchiveReport));
                    return false;
                }

                else
                {
                    _ = Directory.CreateDirectory(dir);
                }
            }

            if (File.Exists(path) == true)
            {
                _ = Log.Information($"Report was already archived.", nameof(ArchiveReport));
                return true;
            }

            File.WriteAllText(path, this._rawData, Resources.Encoding);

            _ = Log.Success("Report archived.", nameof(ArchiveReport));
            return true;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(ex, nameof(ArchiveReport));
            return false;
        }
    }

    /// <summary>
    /// Attempts to open a local exchange report from a specified file.
    /// </summary>
    /// <param name="filename">Path to previously archived exchange report file.</param>
    /// <returns>Operation result.</returns>
    public bool OpenArchivedRecord(string filename)
    {
        try
        {
            // empty the default data
            this._info = default;
            this._rates.Clear();
            this._rawData = string.Empty;

            // check if file exists
            if (File.Exists(filename) == false) return false;

            string data = File.ReadAllText(filename, Resources.Encoding);
            bool result = ParseData(data);

            if (result == true)
            {
                _ = Log.Success("Local exchange report loaded.", nameof(OpenArchivedRecord));
            }

            else
            {
                _ = Log.Error("Unable to open local exchange report.", nameof(OpenArchivedRecord));
            }

            return result;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(ex, nameof(OpenArchivedRecord));
            return false;
        }
    }

    private ExchangeInfo _info;
    private List<RateInfo> _rates;
    private bool _ready;
    private string _rawData;

    /// <summary>
    /// Representing the recieved exchange info.
    /// </summary>
    public ExchangeInfo Exchange => this._info;

    /// <summary>
    /// Representing the list of available exchange rates.
    /// </summary>
    public List<RateInfo> Rates => this._rates;

    /// <summary>
    /// Determines whether the manager has fetched data successfully at least once.
    /// </summary>
    public bool IsReady => this._ready;
}
