using System.Threading.Tasks;

namespace SystemUpdate.Core;

/// <summary>
/// Representing the package manager contract.
/// </summary>
public interface IPackageManager
{
    /// <summary>
    /// Display name.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Updates all packages.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the update process started, otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> Update();
    
    /// <summary>
    /// Synchronizes the package database.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the sync process started, otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> Sync();
    
    /// <summary>
    /// Installs the given package <paramref name="packageId"/>.
    /// </summary>
    /// <param name="packageId"></param>
    /// <returns>
    /// <see langword="true"/> if the installation process started, otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> Install(string packageId);
    
    /// <summary>
    /// Removes the given package <paramref name="packageId"/>.
    /// </summary>
    /// <param name="packageId"></param>
    /// <returns>
    /// <see langword="true"/> if the removal process started, otherwise <see langword="false"/>.
    /// </returns>
    Task<bool> Remove(string packageId);
}