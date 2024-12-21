namespace SharpRacer.Internal.Connections;
internal interface IConnectionManager
{
    void Connect(ISimulatorOuterConnection outerConnection);
    void Connect(ISimulatorOuterConnection outerConnection, TimeSpan timeout);
    Task ConnectAsync(ISimulatorOuterConnection outerConnection, CancellationToken cancellationToken = default);
    Task ConnectAsync(ISimulatorOuterConnection outerConnection, TimeSpan timeout, CancellationToken cancellationToken = default);
    void Disconnect(ISimulatorOuterConnection outerConnection);
    void Return(IOpenInnerConnection connection);
}
