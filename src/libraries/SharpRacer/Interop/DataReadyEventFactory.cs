using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;
using Windows.Win32;

namespace SharpRacer.Interop;

[SupportedOSPlatform("windows")]
internal sealed class DataReadyEventFactory : IDataReadyEventFactory
{
    public const string EventName = "Local\\IRSDKDataValidEvent";

    public static IDataReadyEventFactory Default { get; } = new DataReadyEventFactory();

    public AutoResetEvent CreateAutoResetEvent(bool initialState = false)
    {
        return new AutoResetEvent(initialState)
        {
            SafeWaitHandle = CreateSafeWaitHandle()
        };
    }

    internal static SafeWaitHandle CreateSafeWaitHandle()
    {
        return PInvoke.CreateEventAsSafeWaitHandle(
                lpEventAttributes: null,
                bManualReset: true,
                bInitialState: false,
                lpName: EventName);
    }
}
