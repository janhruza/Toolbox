namespace CnbRat.Core;

/// <summary>
/// Representing a single rate entry information.
/// </summary>
public struct RateInfo
{
    /// <summary>
    /// Representing the currency name in Czech.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Representinmg the country name.
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Representing the amount of the currency.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Representing the value of a single currency in Czech Crowns (CZK).
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Representing the currency code.
    /// </summary>
    public string Code { get; set; }
}
