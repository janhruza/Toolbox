namespace CnbRat;

/// <summary>
/// Representing application resources.
/// </summary>
public static class Resources
{
    /// <summary>
    /// Representing an URL address pointing to the CNB exchange rates CSV file.
    /// This link has no parameters - it fetches the current exchange rates.
    /// </summary>
    public const string URL_EXCHANGE_RATES = "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt";

    /// <summary>
    /// Represents the URL template for retrieving historical exchange rates from the Czech National Bank.
    /// </summary>
    /// <remarks>The URL contains placeholders for day, month, and year, which must be replaced with the
    /// desired date values in the format specified by the Czech National Bank. The resulting URL can be used to
    /// download daily exchange rate data for a specific date.</remarks>
    public const string URL_EXCHANGE_RATES_HISTORICAL = "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt?date={day}.{month}.{year}";

    /// <summary>
    /// Representing the source data culture. This data includes the reports downloaded from the internet.
    /// </summary>
    public const string Culture = "cs-CZ";
}