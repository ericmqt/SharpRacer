namespace SharpRacer.Internal.Connections;

internal sealed class OuterConnectionTracker : IOuterConnectionTracker
{
    private readonly bool _closeOnEmpty;
    private bool _isClosed;
    private readonly List<IOuterConnection> _outerConnections;
    private readonly object _lock = new object();
    private int _trackedConnectionCount;

    public OuterConnectionTracker(bool closeOnEmpty)
    {
        _closeOnEmpty = closeOnEmpty;

        _outerConnections = [];
    }

    public bool CloseOnEmpty => _closeOnEmpty;
    public bool HasTrackedConnections => Interlocked.CompareExchange(ref _trackedConnectionCount, 0, 0) != 0;
    public bool IsClosed => _isClosed;
    public bool IsEmpty => Interlocked.CompareExchange(ref _trackedConnectionCount, 0, 0) == 0;

    public bool Attach(IOuterConnection outerConnection)
    {
        lock (_lock)
        {
            if (_isClosed)
            {
                return false;
            }

            if (!_outerConnections.Contains(outerConnection))
            {
                // Begin tracking the outer connection
                _outerConnections.Add(outerConnection);

                Interlocked.Increment(ref _trackedConnectionCount);
            }

            // Return true even if the outer connection is already tracked
            return true;
        }
    }

    public void Close()
    {
        lock (_lock)
        {
            _isClosed = true;
        }
    }

    public bool Detach(IOuterConnection connection)
    {
        lock (_lock)
        {
            if (!_outerConnections.Remove(connection))
            {
                return false;
            }

            Interlocked.Decrement(ref _trackedConnectionCount);

            if (_closeOnEmpty && !_isClosed && _outerConnections.Count == 0)
            {
                _isClosed = true;
            }

            return true;
        }
    }

    public IEnumerable<IOuterConnection> DetachAll()
    {
        lock (_lock)
        {
            if (_closeOnEmpty)
            {
                _isClosed = true;
            }

            while (_outerConnections.Count > 0)
            {
                var outer = _outerConnections.First();

                _outerConnections.Remove(outer);
                Interlocked.Decrement(ref _trackedConnectionCount);

                yield return outer;
            }
        }
    }
}
