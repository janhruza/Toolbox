using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Toolbox;

namespace CnbRat.Core;

/// <summary>
///     Representing the exchange manager class.
/// </summary>
public class ExchangeManager
{
    private string _rawData;

    /// <summary>
    ///     Creates a new <see cref="ExchangeManager" /> instance.
    /// </summary>
    public ExchangeManager()
    {
        this.Rates = [];
        this.IsReady = false;
        this._rawData = string.Empty;
    }

    /// <summary>
    ///     Gets the full path to the application's archive directory.
    /// </summary>
    /// <remarks>
    ///     The archive directory is located within the application's base directory and is intended for
    ///     storing archived files or data. The exact location is determined at runtime based on the application's current
    ///     domain.
    /// </remarks>
    public static string ArchiveDirectory =>
        Path.Combine(path1: AppDomain.CurrentDomain.BaseDirectory, path2: Resources.ARCHIVE_DIRECTORY);

    /// <summary>
    ///     Representing the recieved exchange info.
    /// </summary>
    public ExchangeInfo Exchange { get; private set; }

    /// <summary>
    ///     Representing the list of available exchange rates.
    /// </summary>
    public List<RateInfo> Rates { get; }

    /// <summary>
    ///     Determines whether the manager has fetched data successfully at least once.
    /// </summary>
    public bool IsReady { get; private set; }

    private bool ParseData(string sourceData)
    {
        // check data validity
        if (string.IsNullOrWhiteSpace(value: sourceData))
        {
            // invalid data
            _ = Log.Error(message: "Invalid data.", tag: nameof(ExchangeManager.Fetch));
            return false;
        }

        // split the data in lines
        string[] linesData = sourceData.Split(separator: '\n', options: StringSplitOptions.TrimEntries);

        // check if the lines are valid
        if (linesData.Length < 2)
        {
            // no data
            _ = Log.Error(message: "Fetched data contain no valid lines.", tag: nameof(ExchangeManager.Fetch));
            return false;
        }

        // process the data lines
        // get date and release
        string[] metadata = linesData[0].Split(separator: '#', options: StringSplitOptions.TrimEntries);
        if (metadata.Length != 2)
        {
            // invalid metadata
            _ = Log.Error(message: "Invalid metadata.", tag: nameof(ExchangeManager.Fetch));
            return false;
        }

        string[] dateInfo = metadata[0].Split(separator: '.', options: StringSplitOptions.TrimEntries);
        if (dateInfo.Length != 3)
        {
            _ = Log.Error(message: "Invalid date format.", tag: nameof(ExchangeManager.Fetch));
            return false;
        }

        // get the metadata
        int day = Convert.ToInt32(value: dateInfo[0]);
        int month = Convert.ToInt32(value: dateInfo[1]);
        int year = Convert.ToInt32(value: dateInfo[2]);
        int release = Convert.ToInt32(value: metadata[1]);

        // assign the metadata
        this.Exchange = new ExchangeInfo
        {
            Date = new DateOnly(year: year, month: month, day: day),
            Release = release
        };

        // process individual exchange rate entries
        for (int x = 2; x < linesData.Length; x++)
        {
            // split the columns
            string[] entry = linesData[x].Split(separator: '|', options: StringSplitOptions.TrimEntries);

            // check data size
            if (entry.Length != 5) continue;

            RateInfo rate = new()
            {
                Country = entry[0],
                Currency = entry[1],
                Amount = Convert.ToInt32(value: entry[2]),
                Code = entry[3],
                Value = Convert.ToDecimal(value: entry[4].Replace(oldChar: ',',
                    newChar: '.')) // convert decimal commas to periods
            };

            // add the rate to the list
            this.Rates.Add(item: rate);
        }

        // add the CZK entry manually
        RateInfo czk = new()
        {
            Country = "Česká republika",
            Currency = "koruna",
            Amount = 1,
            Code = "CZK",
            Value = 1.0m
        };
        this.Rates.Add(item: czk);

        this.IsReady = true;
        return true;
    }

    /// <summary>
    ///     Attempts to fetch the current exchange records from the official source defined in
    ///     <see cref="Resources.URL_EXCHANGE_RATES" />.
    /// </summary>
    /// <param name="date">
    ///     The specific date for which to fetch exchange rates. Use <see cref="DateOnly.MinValue" /> to fetch the latest
    ///     available rates.
    /// </param>
    /// <returns><see langword="true" /> if the fetch was successful, otherwise <see langword="false" />.</returns>
    /// <remarks>
    ///     If this method succeeds, the resulting properties can be found in the <see cref="Exchange" /> and
    ///     <see cref="Rates" /> properties.
    /// </remarks>
    public bool Fetch(DateOnly date)
    {
        try
        {
            // empty the default data
            this.Exchange = default;
            this.Rates.Clear();
            this._rawData = string.Empty;

            string requestUri;
            if (date == DateOnly.MinValue)
                // fetch the latest data
                requestUri = Resources.URL_EXCHANGE_RATES;
            else
                // fetch historical data
                requestUri = Resources.URL_EXCHANGE_RATES_HISTORICAL
                    .Replace(oldValue: "{day}", newValue: date.Day.ToString(format: "D2"))
                    .Replace(oldValue: "{month}", newValue: date.Month.ToString(format: "D2"))
                    .Replace(oldValue: "{year}", newValue: date.Year.ToString(format: "D4"));

            string data = string.Empty;
            using (HttpClient client = new())
            {
                Task<string> task =
                    client.GetStringAsync(requestUri: new Uri(uriString: requestUri,
                        uriKind: UriKind.RelativeOrAbsolute));
                task.Wait();

                // check if the task has faulted
                if (task.IsFaulted)
                {
                    _ = Log.Error(message: "Fetching task faulted.", tag: nameof(ExchangeManager.Fetch));
                    return false;
                }

                // assign read data
                data = task.Result;
                this._rawData = task.Result;
            }

            // parse the recieved data
            return this.ParseData(sourceData: data);
        }

        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex, tag: nameof(ExchangeManager.Fetch));
            return false;
        }
    }

    /// <summary>
    ///     Attempts to archive the currently fetched exchange records to a local file defined in
    ///     <see cref="Resources.ARCHIVE_DIRECTORY" />.
    /// </summary>
    /// <returns></returns>
    public bool ArchiveReport()
    {
        try
        {
            // output file name
            string path = Path.Combine(path1: ExchangeManager.ArchiveDirectory,
                path2: $"{this.Exchange.Date.ToString(format: "yyyyMMdd")}{this.Exchange.Release}.txt");

            if (!Directory.Exists(path: Path.GetDirectoryName(path: path)))
            {
                string dir = Path.GetDirectoryName(path: path) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(value: dir))
                {
                    _ = Log.Error(message: "Unable to determine archive directory.",
                        tag: nameof(ExchangeManager.ArchiveReport));
                    return false;
                }

                _ = Directory.CreateDirectory(path: dir);
            }

            if (File.Exists(path: path))
            {
                _ = Log.Information(message: "Report was already archived.",
                    tag: nameof(ExchangeManager.ArchiveReport));
                return true;
            }

            File.WriteAllText(path: path, contents: this._rawData, encoding: Resources.Encoding);

            _ = Log.Success(message: "Report archived.", tag: nameof(ExchangeManager.ArchiveReport));
            return true;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex, tag: nameof(ExchangeManager.ArchiveReport));
            return false;
        }
    }

    /// <summary>
    ///     Attempts to open a local exchange report from a specified file.
    /// </summary>
    /// <param name="filename">Path to previously archived exchange report file.</param>
    /// <returns>Operation result.</returns>
    public bool OpenArchivedRecord(string filename)
    {
        try
        {
            // empty the default data
            this.Exchange = default;
            this.Rates.Clear();
            this._rawData = string.Empty;

            // check if file exists
            if (!File.Exists(path: filename)) return false;

            string data = File.ReadAllText(path: filename, encoding: Resources.Encoding);
            bool result = this.ParseData(sourceData: data);

            if (result)
                _ = Log.Success(message: "Local exchange report loaded.",
                    tag: nameof(ExchangeManager.OpenArchivedRecord));

            else
                _ = Log.Error(message: "Unable to open local exchange report.",
                    tag: nameof(ExchangeManager.OpenArchivedRecord));

            return result;
        }

        catch (Exception ex)
        {
            _ = Log.Exception(exception: ex, tag: nameof(ExchangeManager.OpenArchivedRecord));
            return false;
        }
    }
}