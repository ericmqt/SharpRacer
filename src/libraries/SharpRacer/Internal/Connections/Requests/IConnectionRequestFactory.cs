namespace SharpRacer.Internal.Connections.Requests;
internal interface IConnectionRequestFactory
{
    IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        CancellationToken cancellationToken = default);

    IAsyncConnectionRequest CreateAsyncRequest(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    ConnectionRequest CreateRequest(IOuterConnection outerConnection);
    ConnectionRequest CreateRequest(IOuterConnection outerConnection, TimeSpan timeout);
}
