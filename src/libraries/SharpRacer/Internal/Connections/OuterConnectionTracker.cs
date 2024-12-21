namespace SharpRacer.Internal.Connections;

internal sealed class OuterConnectionTracker : IOuterConnectionTracker
{
    private bool _canAttach;
    private readonly List<ISimulatorOuterConnection> _outerConnections;
    private readonly object _lock = new object();

    public OuterConnectionTracker()
    {
        _outerConnections = [];
    }

    public bool CanAttach
    {
        get => _canAttach;
        set => _canAttach = value;
    }

    public bool Attach(ISimulatorOuterConnection outerConnection)
    {
        lock (_lock)
        {
            if (!_canAttach)
            {
                return false;
            }

            if (!_outerConnections.Contains(outerConnection))
            {
                // Begin tracking the outer connection
                _outerConnections.Add(outerConnection);
            }

            // Return true even if owner already exists
            return true;
        }
    }

    public bool Detach(ISimulatorOuterConnection connection, out bool isInnerConnectionOrphaned)
    {
        lock (_lock)
        {
            isInnerConnectionOrphaned = false;

            // We only care about inner connection becoming an orphan when we successfully stop tracking an outer connection, if it's not
            // in the collection then we can't really say anything about whether the inner connection is orphaned or not.
            if (_outerConnections.Remove(connection))
            {
                isInnerConnectionOrphaned = _outerConnections.Count == 0;

                return true;
            }

            return false;
        }
    }

    public IEnumerable<ISimulatorOuterConnection> DetachAll()
    {
        lock (_outerConnections)
        {
            while (_outerConnections.Count > 0)
            {
                var outer = _outerConnections.First();

                _outerConnections.Remove(outer);

                yield return outer;
            }
        }
    }
}
