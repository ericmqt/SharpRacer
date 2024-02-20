using System.Runtime.Versioning;

namespace SharpRacer.Tools.TelemetryVariables;
internal static class SimulatorPlatformGuard
{
    [SupportedOSPlatformGuard("windows5.1.2600")]
    public static bool IsSupportedPlatform()
    {
        return OperatingSystem.IsOSPlatformVersionAtLeast("windows", 5, 1, 2600);
    }
}
