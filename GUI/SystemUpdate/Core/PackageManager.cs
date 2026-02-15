using System.Threading.Tasks;

namespace SystemUpdate.Core;

/// <summary>
/// Representing the base package manager class.
/// </summary>
public abstract class PackageManager : IPackageManager
{
    /// <summary>
    /// Creates the new <see cref="PackageManager"/> instance.
    /// </summary>
    /// <param name="name">Display name of the package manager.</param>
    protected PackageManager(string name)
    {
        this.Name = name;
    }
    
    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public abstract Task<bool> Update();

    /// <inheritdoc/>
    public abstract Task<bool> Sync();

    /// <inheritdoc/>
    public abstract Task<bool> Install(string packageId);

    /// <inheritdoc/>
    public abstract Task<bool> Remove(string packageId);
}