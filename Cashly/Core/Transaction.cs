using System;
using System.Runtime.InteropServices;

namespace Cashly.Core;

/// <summary>
/// Represents a financial transaction in the Cashly application.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Transaction
{
    /// <summary>
    /// Represents the date of the transaction.
    /// </summary>
    public DateTime Date;

    /// <summary>
    /// Represents the description of the transaction.
    /// </summary>
    public string Description;

    /// <summary>
    /// Represents the amount of the transaction.
    /// </summary>
    public decimal Amount;

    /// <summary>
    /// Represents the category of the transaction.
    /// </summary>
    public string Category;

    /// <summary>
    /// Represents the type of the transaction (Income or Expense).
    /// </summary>
    public TransactionType Type;

    /// <summary>
    /// Creates a new transaction with default values.
    /// </summary>
    public Transaction()
    {
        this.Date = DateTime.Now;
        this.Description = string.Empty;
        this.Amount = 0m;
        this.Category = string.Empty;
        this.Type = TransactionType.Undefined;
    }

    /// <summary>
    /// Crates a new transaction.
    /// </summary>
    /// <param name="date">Date of the transaction.</param>
    /// <param name="description">Description of the transaction.</param>
    /// <param name="amount">Transaction amount. It gets automatically converted to its absolute value using the <see cref="decimal.Abs(decimal)"/> method.</param>
    /// <param name="category">Category of the transaction.</param>
    /// <param name="type">Type of the transaction (Income or Expense) or <see cref="TransactionType.Undefined"/> by default.</param>
    public Transaction(DateTime date, string description, decimal amount, string category, TransactionType type)
    {
        this.Date = date;
        this.Description = description;
        this.Amount = decimal.Abs(amount);
        this.Category = category;
        this.Type = type;
    }

    /// <summary>
    /// Returns a string representation of the transaction.
    /// </summary>
    /// <returns>
    /// The string representation of the transaction.
    /// </returns>
    public override string ToString()
    {
        return $"{this.Date.ToShortDateString()} | {this.Description} | {this.Amount:2F} | {this.Category}";
    }
}
