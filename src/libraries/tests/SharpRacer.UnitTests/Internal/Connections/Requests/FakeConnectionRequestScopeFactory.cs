using Moq;

namespace SharpRacer.Internal.Connections.Requests;
internal class FakeConnectionRequestScopeFactory : IConnectionRequestScopeFactory
{
    private Action? _onScopeEntering;
    private Action? _onExitingScope;
    private Action? _onScopeEntered;
    private Action? _onScopeExited;
    private int _openScopeCount;
    private readonly IPendingRequestCounter _requestCounter;
    private int _totalScopesClosed;
    private int _totalScopesOpened;

    public FakeConnectionRequestScopeFactory(MockRepository mockRepository)
    {
        var mock = mockRepository.Create<IPendingRequestCounter>();

        mock.Setup(x => x.DecrementExecutingSynchronousRequestCount())
            .Callback(OnDecrementExecutingSynchronousRequestCount);

        mock.Setup(x => x.IncrementExecutingSynchronousRequestCount())
            .Callback(OnIncrementExecutingSynchronousRequestCount);

        _requestCounter = mock.Object;
    }

    public int OpenScopeCount => _openScopeCount;
    public int TotalScopesClosed => _totalScopesClosed;
    public int TotalScopesOpened => _totalScopesOpened;

    public AsyncConnectionRequestScope CreateAsyncScope()
    {
        throw new NotImplementedException();
    }

    public ConnectionRequest.Scope CreateScope()
    {
        return new ConnectionRequest.Scope(_requestCounter);
    }

    public FakeConnectionRequestScopeFactory OnScopeEntered(Action enterScopeAction)
    {
        _onScopeEntered = enterScopeAction;

        return this;
    }

    public FakeConnectionRequestScopeFactory OnScopeEntering(Action beforeEnterScopeAction)
    {
        _onScopeEntering = beforeEnterScopeAction;

        return this;
    }

    public FakeConnectionRequestScopeFactory OnScopeExited(Action exitScopeAction)
    {
        _onScopeExited = exitScopeAction;

        return this;
    }

    public FakeConnectionRequestScopeFactory OnScopeExiting(Action beforeExitScopeAction)
    {
        _onExitingScope = beforeExitScopeAction;

        return this;
    }

    private void OnDecrementExecutingSynchronousRequestCount()
    {
        _onExitingScope?.Invoke();

        Interlocked.Decrement(ref _openScopeCount);
        Interlocked.Increment(ref _totalScopesClosed);

        _onScopeExited?.Invoke();
    }

    private void OnIncrementExecutingSynchronousRequestCount()
    {
        _onScopeEntering?.Invoke();

        Interlocked.Increment(ref _openScopeCount);
        Interlocked.Increment(ref _totalScopesOpened);

        _onScopeEntered?.Invoke();
    }
}
