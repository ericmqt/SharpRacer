namespace SharpRacer.Internal;
internal interface IConnectionPool
{
    void Connect(ISimulatorOuterConnection outerConnection);
    void Connect(ISimulatorOuterConnection outerConnection, TimeSpan timeout);
    Task ConnectAsync(ISimulatorOuterConnection outerConnection, CancellationToken cancellationToken = default);
    Task ConnectAsync(ISimulatorOuterConnection outerConnection, TimeSpan timeout, CancellationToken cancellationToken = default);

    void ReleaseOuterConnection(ISimulatorOuterConnection outerConnection);
    void Return(IOpenInnerConnection connection);
}
