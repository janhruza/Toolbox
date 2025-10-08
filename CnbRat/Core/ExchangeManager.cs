using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

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
        _rates = [];
    }

    /// <summary>
    /// Attempts to fetch the current exchange records from the official source defined in <see cref="Resources.URL_EXCHANGE_RATES"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the fetch was successful, otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// If this method succeeds, the resulting properties can be found in the <see cref="Exchange"/> and <see cref="Rates"/> properties.
    /// </remarks>
    public bool Fetch()
    {
        try
        {
            // empty the default data
            _info = default;
            _rates.Clear();

            string data = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                var task = client.GetStringAsync(new Uri(Resources.URL_EXCHANGE_RATES, UriKind.RelativeOrAbsolute));
                task.Wait();

                // check if the task has faulted
                if (task.IsFaulted)
                {
                    Log.Error("Fetching task faulted.", nameof(Fetch));
                    return false;
                }

                // assign read data
                data = task.Result;
            }

            // check data validity
            if (string.IsNullOrWhiteSpace(data) == true)
            {
                // invalid data
                Log.Error("Invalid data.", nameof(Fetch));
                return false;
            }

            // split the data in lines
            string[] linesData = data.Split('\n', StringSplitOptions.TrimEntries);

            // check if the lines are valid
            if (linesData.Length < 2)
            {
                // no data
                Log.Error("Fetched data contain no valid lines.", nameof(Fetch));
                return false;
            }

            // process the data lines
            // get date and release
            string[] metadata = linesData[0].Split('#', StringSplitOptions.TrimEntries);
            if (metadata.Length != 2)
            {
                // invalid metadata
                Log.Error("Invalid metadata.", nameof(Fetch));
                return false;
            }

            string[] dateInfo = metadata[0].Split('.', StringSplitOptions.TrimEntries);
            if (dateInfo.Length != 3)
            {
                Log.Error("Invalid date format.", nameof(Fetch));
                return false;
            }

            // get the metadata
            int day = Convert.ToInt32(dateInfo[0]);
            int month = Convert.ToInt32(dateInfo[1]);
            int year = Convert.ToInt32(dateInfo[2]);
            int release = Convert.ToInt32(metadata[1]);

            // assign the metadata
            _info = new ExchangeInfo
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
                _rates.Add(rate);
            }

            return true;
        }

        catch(Exception ex)
        {
            Log.Exception(ex, nameof(Fetch));
            return false;
        }
    }

    private ExchangeInfo _info;
    private List<RateInfo> _rates;

    /// <summary>
    /// Representing the recieved exchange info.
    /// </summary>
    public ExchangeInfo Exchange => _info;

    /// <summary>
    /// Representing the list of available exchange rates.
    /// </summary>
    public List<RateInfo> Rates => _rates;
}
