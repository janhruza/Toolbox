using System.Threading.Tasks;

namespace SystemUpdate.Core.PackageManagers;

/// <summary>
/// Representing the Advanced Package Manager (APT) package manager.
/// </summary>
public class APT() : PackageManager("APT")
{
    private static readonly string _cmd = "apt";
    /// <inheritdoc/>
    public override async Task<bool> Update()
    {
        return await Program.PerformTaskAsync(APT._cmd, "upgrade -y");
    }

    /// <inheritdoc/>
    public override async Task<bool> Sync()
    {
        return await Program.PerformTaskAsync(APT._cmd, "update");
    }

    /// <inheritdoc/>
    public override async Task<bool> Install(string packageId)
    {
        return await Program.PerformTaskAsync(APT._cmd, $"install {packageId}");
    }

    /// <inheritdoc/>
    public override async Task<bool> Remove(string packageId)
    {
        return await Program.PerformTaskAsync(APT._cmd, $"remove {packageId}");
    }
}