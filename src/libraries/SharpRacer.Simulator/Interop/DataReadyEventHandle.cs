using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace SharpRacer.Simulator.Interop;
internal static class DataReadyEventHandle
{
    internal const string EventName = "Local\\IRSDKDataValidEvent";

    internal static SafeWaitHandle CreateSafeWaitHandle()
    {
        return PInvoke.CreateEventAsSafeWaitHandle(
            lpEventAttributes: null,
            bManualReset: true,
            bInitialState: false,
            lpName: EventName);
    }
}
