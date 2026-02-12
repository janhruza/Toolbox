namespace Cashly.Core;

/// <summary>
///     Represents the type of a financial transaction.
/// </summary>
public enum TransactionType : byte
{
    /// <summary>
    ///     Represents an undefined transaction type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    ///     Represents an income transaction.
    /// </summary>
    Income,

    /// <summary>
    ///     Represents an expense transaction.
    /// </summary>
    Expense
}