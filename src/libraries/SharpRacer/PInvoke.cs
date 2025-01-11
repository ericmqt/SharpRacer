using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Windows.Win32.Foundation;
using Windows.Win32.Security;

namespace Windows.Win32;

internal static partial class PInvoke
{
    public const string MinOSPlatform = "windows5.1.2600";

    [SupportedOSPlatform("windows")]
    internal static unsafe SafeWaitHandle CreateEventAsSafeWaitHandle(SECURITY_ATTRIBUTES? lpEventAttributes, BOOL bManualReset, BOOL bInitialState, string lpName)
    {
        fixed (char* lpNameLocal = lpName)
        {
            SECURITY_ATTRIBUTES lpEventAttributesLocal = lpEventAttributes ?? default;

            // .NET 8 is not supported on Windows 5.1.2600 or earlier so SupportedOSPlatform 'windows' is sufficient.
#pragma warning disable CA1416 // Validate platform compatibility
            var handle = CreateEvent(
                lpEventAttributes.HasValue ? &lpEventAttributesLocal : null,
                bManualReset,
                bInitialState,
                lpNameLocal);
#pragma warning restore CA1416 // Validate platform compatibility

            return new SafeWaitHandle(handle, ownsHandle: true);
        }
    }
}
