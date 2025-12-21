using SharpRacer.Telemetry;

namespace SharpRacer.Internal;

public class ConnectionTelemetryVariableInfoProviderTests
{
    [Fact]
    public void NotifyTelemetryVariableActivated_QueueCallbackTest()
    {
        var provider = new ConnectionTelemetryVariableInfoProvider();

        provider.NotifyTelemetryVariableActivated("Foo", onCallbackInvoked);

        void onCallbackInvoked(TelemetryVariableInfo variableInfo)
        {
            Assert.Fail("Callback invoked immediately instead of queued.");
        }
    }

    [Fact]
    public void NotifyTelemetryVariableActivated_ThrowsOnNullArgumentsTest()
    {
        var provider = new ConnectionTelemetryVariableInfoProvider();

        Assert.Throws<ArgumentException>(() => provider.NotifyTelemetryVariableActivated(string.Empty, v => { }));
        Assert.Throws<ArgumentNullException>(() => provider.NotifyTelemetryVariableActivated(null!, v => { }));
        Assert.Throws<ArgumentNullException>(() => provider.NotifyTelemetryVariableActivated("Foo", null!));
    }
}
