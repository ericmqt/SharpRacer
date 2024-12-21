namespace SharpRacer.Internal;
internal interface IConnectionPool
{
    void Connect(SimulatorConnection outerConnection);
    void Connect(SimulatorConnection outerConnection, TimeSpan timeout);
    Task ConnectAsync(SimulatorConnection outerConnection, CancellationToken cancellationToken = default);
    Task ConnectAsync(SimulatorConnection outerConnection, TimeSpan timeout, CancellationToken cancellationToken = default);

    void ReleaseOuterConnection(SimulatorConnection outerConnection);
    void Return(OpenInnerConnection connection);
}
