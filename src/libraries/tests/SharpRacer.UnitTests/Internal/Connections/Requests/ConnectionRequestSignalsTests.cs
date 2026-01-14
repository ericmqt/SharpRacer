namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestSignalsTests
{
    [Fact]
    public void Ctor_Test()
    {
        var signals = new ConnectionRequestSignals();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));
    }

    [Fact]
    public void AsyncRequestCreationSignal_Test()
    {
        var signals = new ConnectionRequestSignals();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));

        signals.BlockAsyncRequestCreation();

        Assert.False(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));

        signals.AllowAsyncRequestCreation();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));
    }

    [Fact]
    public void RequestExecutionSignal_Test()
    {
        var signals = new ConnectionRequestSignals();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));

        signals.BlockRequestExecution();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.False(signals.AllowRequestExecutionSignal.Wait(0));

        signals.AllowRequestExecution();

        Assert.True(signals.AllowAsyncRequestCreationSignal.Wait(0));
        Assert.True(signals.AllowRequestExecutionSignal.Wait(0));
    }

    [Fact]
    public void WaitForAsyncRequestCreation_Test()
    {
        var signals = new ConnectionRequestSignals();

        var waitStartedSignal = new ManualResetEventSlim(initialState: false);
        var waitCompletedSignal = new ManualResetEventSlim(initialState: false);

        signals.BlockAsyncRequestCreation();

        ThreadPool.QueueUserWorkItem(_ =>
        {
            waitStartedSignal.Set();

            signals.WaitForAsyncRequestCreation();

            waitCompletedSignal.Set();
        }, null);

        waitStartedSignal.Wait();
        Assert.False(waitCompletedSignal.Wait(0));

        signals.AllowAsyncRequestCreation();
        Assert.True(waitCompletedSignal.Wait(TimeSpan.FromSeconds(1)));
    }

    [Fact]
    public void WaitForRequestExecution_Test()
    {
        var signals = new ConnectionRequestSignals();

        var waitStartedSignal = new ManualResetEventSlim(initialState: false);
        var waitCompletedSignal = new ManualResetEventSlim(initialState: false);

        signals.BlockRequestExecution();

        ThreadPool.QueueUserWorkItem(_ =>
        {
            waitStartedSignal.Set();

            signals.WaitForRequestExecution();

            waitCompletedSignal.Set();
        }, null);

        waitStartedSignal.Wait();
        Assert.False(waitCompletedSignal.Wait(0));

        signals.AllowRequestExecution();
        Assert.True(waitCompletedSignal.Wait(TimeSpan.FromSeconds(1)));
    }
}
