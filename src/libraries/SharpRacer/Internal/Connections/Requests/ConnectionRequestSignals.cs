namespace SharpRacer.Internal.Connections.Requests;
internal sealed class ConnectionRequestSignals : IConnectionRequestSignals
{
    public ConnectionRequestSignals()
    {
        AllowAsyncRequestCreationSignal = new ManualResetEventSlim(initialState: true);
        AllowRequestExecutionSignal = new ManualResetEventSlim(initialState: true);
    }

    public ManualResetEventSlim AllowAsyncRequestCreationSignal { get; }
    public ManualResetEventSlim AllowRequestExecutionSignal { get; }

    public void AllowAsyncRequestCreation()
    {
        AllowAsyncRequestCreationSignal.Set();
    }

    public void AllowRequestExecution()
    {
        AllowRequestExecutionSignal.Set();
    }

    public void BlockAsyncRequestCreation()
    {
        AllowAsyncRequestCreationSignal.Reset();
    }

    public void BlockRequestExecution()
    {
        AllowRequestExecutionSignal.Reset();
    }

    public void WaitForAsyncRequestCreation()
    {
        AllowAsyncRequestCreationSignal.Wait();
    }

    public void WaitForRequestExecution()
    {
        AllowRequestExecutionSignal.Wait();
    }
}
