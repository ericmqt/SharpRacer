namespace SharpRacer.Internal.Connections;
public class ConnectionSignalsTests
{
    [Fact]
    public void Ctor_CreateConnectionSignalInitialStateTest()
    {
        // Not signaled
        using (var signals = new ConnectionSignals(createConnectionSignalInitialState: false))
        {
            var waitHandles = signals.GetWaitHandles(allowCreateConnection: true);

            Assert.Equal(WaitHandle.WaitTimeout, WaitHandle.WaitAny(waitHandles, TimeSpan.Zero));
        }

        // Signaled
        using (var signals = new ConnectionSignals(createConnectionSignalInitialState: true))
        {
            var waitHandles = signals.GetWaitHandles(allowCreateConnection: true);

            Assert.Equal(
                ConnectionSignalWaitResult.CreateConnection, (ConnectionSignalWaitResult)WaitHandle.WaitAny(waitHandles, TimeSpan.Zero));
        }
    }

    [Fact]
    public void ClearConnectionAvailableSignalTest()
    {
        using var signalsObj = new ConnectionSignals();

        signalsObj.ConnectionAvailableSignal.Set();
        Assert.True(signalsObj.ConnectionAvailableSignal.WaitOne(0));

        ((IConnectionSignals)signalsObj).ClearConnectionAvailableSignal();

        Assert.False(signalsObj.ConnectionAvailableSignal.WaitOne(0));
    }

    [Fact]
    public void ClearConnectionExceptionSignalTest()
    {
        using var signalsObj = new ConnectionSignals();

        signalsObj.ConnectionExceptionSignal.Set();
        Assert.True(signalsObj.ConnectionExceptionSignal.WaitOne(0));

        ((IConnectionSignals)signalsObj).ClearConnectionExceptionSignal();

        Assert.False(signalsObj.ConnectionExceptionSignal.WaitOne(0));
    }

    [Fact]
    public void SetConnectionAvailableSignalTest()
    {
        using var signalsObj = new ConnectionSignals();

        signalsObj.ConnectionAvailableSignal.Reset();
        Assert.False(signalsObj.ConnectionAvailableSignal.WaitOne(0));

        ((IConnectionSignals)signalsObj).SetConnectionAvailableSignal();

        Assert.True(signalsObj.ConnectionAvailableSignal.WaitOne(0));
    }

    [Fact]
    public void SetConnectionExceptionSignalTest()
    {
        using var signalsObj = new ConnectionSignals();

        signalsObj.ConnectionExceptionSignal.Reset();
        Assert.False(signalsObj.ConnectionExceptionSignal.WaitOne(0));

        ((IConnectionSignals)signalsObj).SetConnectionExceptionSignal();

        Assert.True(signalsObj.ConnectionExceptionSignal.WaitOne(0));
    }

    [Fact]
    public void SetCreateConnectionSignalTest()
    {
        using var signalsObj = new ConnectionSignals();

        signalsObj.CreateConnectionSignal.Reset();
        Assert.False(signalsObj.CreateConnectionSignal.WaitOne(0));

        ((IConnectionSignals)signalsObj).SetCreateConnectionSignal();

        Assert.True(signalsObj.CreateConnectionSignal.WaitOne(0));

        // Now check that it automatically reset
        Assert.False(signalsObj.CreateConnectionSignal.WaitOne(0));
    }

    [Fact]
    public void Wait_Test()
    {
        using var signals = new ConnectionSignals(createConnectionSignalInitialState: false);

        // Signal something so we can wait on it
        signals.ConnectionAvailableSignal.Set();

        var waitResult = signals.Wait(false, TimeSpan.FromSeconds(1));

        Assert.Equal(ConnectionSignalWaitResult.ConnectionAvailable, waitResult);
    }

    [Fact]
    public void Wait_AllowCreateConnectionTest()
    {
        using var signals = new ConnectionSignals(createConnectionSignalInitialState: true);

        var waitResult = signals.Wait(true, TimeSpan.FromSeconds(1));

        Assert.Equal(ConnectionSignalWaitResult.CreateConnection, waitResult);
    }

    [Fact]
    public void WaitHandleArrayLengthTest()
    {
        using var signals = new ConnectionSignals(createConnectionSignalInitialState: false);

        var waitHandles = signals.GetWaitHandles(allowCreateConnection: true);

        Assert.Equal(3, waitHandles.Length);

        waitHandles = signals.GetWaitHandles(allowCreateConnection: false);

        Assert.Equal(2, waitHandles.Length);
    }

    [Fact]
    public void WaitHandleOrderMatchesWaitResultEnumTest()
    {
        using var signals = new ConnectionSignals(createConnectionSignalInitialState: false);

        var waitHandles = signals.GetWaitHandles(allowCreateConnection: true);

        Assert.Equal(signals.ConnectionAvailableSignal, waitHandles[(int)ConnectionSignalWaitResult.ConnectionAvailable]);
        Assert.Equal(signals.ConnectionExceptionSignal, waitHandles[(int)ConnectionSignalWaitResult.ConnectionException]);
        Assert.Equal(signals.CreateConnectionSignal, waitHandles[(int)ConnectionSignalWaitResult.CreateConnection]);

        waitHandles = signals.GetWaitHandles(allowCreateConnection: false);

        Assert.Equal(signals.ConnectionAvailableSignal, waitHandles[(int)ConnectionSignalWaitResult.ConnectionAvailable]);
        Assert.Equal(signals.ConnectionExceptionSignal, waitHandles[(int)ConnectionSignalWaitResult.ConnectionException]);
    }
}
