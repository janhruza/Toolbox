using System;

namespace CnbRat.Core;

/// <summary>
///     Representing a single exchange report.
/// </summary>
public struct ExchangeInfo
{
    /// <summary>
    ///     Representing the date of the report.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    ///     Representing the release number of the report.
    /// </summary>
    public int Release { get; set; }
}