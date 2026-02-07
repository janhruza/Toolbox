namespace Algebrax.Data;

/// <summary>
/// Representing the types of discontnuity.
/// </summary>
public enum DiscontinuityType
{
    /// <summary>
    /// A removable discontinuity.
    /// </summary>
    Removable,    // Odstranitelná (díra v grafu)

    /// <summary>
    /// First-type (type I.) discontinuity.
    /// </summary>
    Jump,

    /// <summary>
    /// Second-type ( type II.) discontinuity.
    /// </summary>
    Essential
}
