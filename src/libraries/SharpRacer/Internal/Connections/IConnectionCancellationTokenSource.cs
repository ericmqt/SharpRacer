namespace SharpRacer.Internal.Connections;
internal interface IConnectionCancellationTokenSource : IDisposable
{
    CancellationToken Token { get; }

    void Cancel();
    CancellationTokenSource CreateLinkedTokenSource(CancellationToken cancellationToken);
}
