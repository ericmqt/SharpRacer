namespace SharpRacer.Internal.Connections;
internal interface IConnectionManager
{
    void Connect(IOuterConnection outerConnection);
    void Connect(IOuterConnection outerConnection, TimeSpan timeout);
    Task ConnectAsync(IOuterConnection outerConnection, CancellationToken cancellationToken = default);
    Task ConnectAsync(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);
}
