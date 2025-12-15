using SharpRacer.Telemetry;

namespace SharpRacer.Internal;
public class ConnectionDataVariableInfoProviderTests
{
    [Fact]
    public void NotifyDataVariableActivated_QueueCallbackTest()
    {
        var provider = new ConnectionDataVariableInfoProvider();

        provider.NotifyDataVariableActivated("Foo", onCallbackInvoked);

        void onCallbackInvoked(DataVariableInfo variableInfo)
        {
            Assert.Fail("Callback invoked immediately instead of queued.");
        }
    }

    [Fact]
    public void NotifyDataVariableActivated_ThrowsOnNullArgumentsTest()
    {
        var provider = new ConnectionDataVariableInfoProvider();

        Assert.Throws<ArgumentException>(() => provider.NotifyDataVariableActivated(string.Empty, v => { }));
        Assert.Throws<ArgumentNullException>(() => provider.NotifyDataVariableActivated(null!, v => { }));
        Assert.Throws<ArgumentNullException>(() => provider.NotifyDataVariableActivated("Foo", null!));
    }
}
