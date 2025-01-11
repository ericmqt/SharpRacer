namespace SharpRacer.Internal.Connections;

internal sealed class OuterConnectionTracker : IOuterConnectionTracker
{
    private bool _canAttach;
    private readonly List<IOuterConnection> _outerConnections;
    private readonly object _lock = new object();

    public OuterConnectionTracker()
    {
        _outerConnections = [];

        _canAttach = true;
    }

    public bool CanAttach => _canAttach;

    public bool Attach(IOuterConnection outerConnection)
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

    public bool Detach(IOuterConnection connection, out bool isInnerConnectionOrphaned)
    {
        lock (_lock)
        {
            // We only care about inner connection becoming an orphan when we successfully stop tracking an outer connection, if it's not
            // in the collection then we can't really say anything about whether the inner connection is orphaned or not.
            if (_outerConnections.Remove(connection))
            {
                isInnerConnectionOrphaned = _outerConnections.Count == 0;

                // Disable attaching only on success, otherwise a failed detach for a non-tracked connection could disable us when we
                // haven't attached to a single outer connection yet
                _canAttach = _outerConnections.Count > 0;

                return true;
            }

            isInnerConnectionOrphaned = false;
            return false;
        }
    }

    public IEnumerable<IOuterConnection> DetachAll()
    {
        lock (_lock)
        {
            _canAttach = false;

            while (_outerConnections.Count > 0)
            {
                var outer = _outerConnections.First();

                _outerConnections.Remove(outer);

                yield return outer;
            }
        }
    }

    public void DisableAttach()
    {
        lock (_lock)
        {
            _canAttach = false;
        }
    }
}
