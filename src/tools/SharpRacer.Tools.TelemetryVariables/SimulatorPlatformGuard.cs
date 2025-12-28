using System.Runtime.Versioning;

namespace SharpRacer.Tools.TelemetryVariables;

internal static class SimulatorPlatformGuard
{
    [SupportedOSPlatformGuard("windows")]
    public static bool IsSupportedPlatform()
    {
        return OperatingSystem.IsWindows();
    }
}
