namespace SharpRacer.Internal.Connections.Requests;
internal interface IAsyncConnectionRequest : IDisposable
{
    bool CanBeginConnectionAttempt { get; }
    TaskCompletionSource Completion { get; }
    IOuterConnection OuterConnection { get; }
    TimeSpan Timeout { get; }

    bool IsTimedOut();
    bool TryComplete(IConnectionProvider connectionProvider);
}
