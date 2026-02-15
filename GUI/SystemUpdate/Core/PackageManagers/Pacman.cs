using System.Diagnostics;
using System.Threading.Tasks;

namespace SystemUpdate.Core.PackageManagers;

/// <summary>
/// Representing the PacMan package manager.
/// </summary>
public class Pacman() : PackageManager("PacMan")
{
    private static readonly string _cmd = "pacman";

    /// <inheritdoc/>
    public override async Task<bool> Update()
    {
        return await Program.PerformTaskAsync(Pacman._cmd, "-Su");
    }

    /// <inheritdoc/>
    public override async Task<bool> Sync()
    {
        return await Program.PerformTaskAsync(Pacman._cmd, "-Sy");
    }

    /// <inheritdoc/>
    public override async Task<bool> Install(string packageId)
    {
        return await Program.PerformTaskAsync(Pacman._cmd, $"-S {packageId}");
    }

    /// <inheritdoc/>
    public override async Task<bool> Remove(string packageId)
    {
        return await Program.PerformTaskAsync(Pacman._cmd, $"-R {packageId}");
    }
}