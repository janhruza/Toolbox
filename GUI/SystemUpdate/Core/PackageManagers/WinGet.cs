using System.Threading.Tasks;

namespace SystemUpdate.Core.PackageManagers;

/// <summary>
/// Representing the Window WinGet package manager.
/// </summary>
public class WinGet() : PackageManager("WinGet")
{
    private static readonly string _cmd = "winget";
    
    /// <inheritdoc/>
    public override async Task<bool> Update()
    {
        return await Program.PerformTaskAsync(WinGet._cmd, "upgrade --all");
    }

    /// <inheritdoc/>
    public override async Task<bool> Sync()
    {
        return await Program.PerformTaskAsync(WinGet._cmd, "update");
    }

    /// <inheritdoc/>
    public override async Task<bool> Install(string packageId)
    {
        return await Program.PerformTaskAsync(WinGet._cmd, $"install {packageId}");
    }

    /// <inheritdoc/>
    public override async Task<bool> Remove(string packageId)
    {
        return await Program.PerformTaskAsync(WinGet._cmd, $"remove {packageId}");
    }
}