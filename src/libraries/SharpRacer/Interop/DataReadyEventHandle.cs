using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace SharpRacer.Interop;
internal static class DataReadyEventHandle
{
    internal const string EventName = "Local\\IRSDKDataValidEvent";

    [SupportedOSPlatform("windows5.1.2600")]
    internal static SafeWaitHandle CreateSafeWaitHandle()
    {
        return PInvoke.CreateEventAsSafeWaitHandle(
            lpEventAttributes: null,
            bManualReset: true,
            bInitialState: false,
            lpName: EventName);
    }
}
