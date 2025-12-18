using SharpRacer.IO;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;

internal sealed class ClosedInnerConnection : IClosedInnerConnection
{
    private object _closeLock = new();
    private bool _isClosed;
    private bool _isDisposed;
    private readonly IOuterConnectionTracker _outerConnectionTracker;

    public ClosedInnerConnection(IOpenInnerConnection openInnerConnection, IOuterConnectionTracker outerConnectionTracker)
        : this(openInnerConnection.DataFile, outerConnectionTracker, openInnerConnection.ConnectionId)
    {

    }

    public ClosedInnerConnection(IConnectionDataFile dataFile, IOuterConnectionTracker outerConnectionTracker)
        : this(dataFile, outerConnectionTracker, connectionId: 0)
    {

    }

    public ClosedInnerConnection(IConnectionDataFile dataFile, IOuterConnectionTracker outerConnectionTracker, int connectionId)
    {
        DataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        _outerConnectionTracker = outerConnectionTracker ?? throw new ArgumentNullException(nameof(outerConnectionTracker));

        if (!_outerConnectionTracker.CloseOnEmpty)
        {
            throw new ArgumentException(
                $"'{nameof(outerConnectionTracker)}' property '{nameof(IOuterConnectionTracker.CloseOnEmpty)}' must be true.", nameof(outerConnectionTracker));
        }

        ConnectionId = connectionId;
    }

    public int ConnectionId { get; }
    public ReadOnlySpan<byte> Data => DataFile.Memory.Span;
    public IConnectionDataFile DataFile { get; }
    public TimeSpan IdleTimeout { get; set; } = Timeout.InfiniteTimeSpan;
    public SimulatorConnectionState State { get; } = SimulatorConnectionState.Closed;

    public void Close()
    {
        // We are no longer accepting new outer connections beause OpenInnerConnection is closing, which will trigger new connection
        // requests to create a new one. At this point, ClosedInnerConnection just needs to sit around until its outer connections have
        // also closed/disposed so we can dispose ourselves.

        lock (_closeLock)
        {
            if (_isDisposed || _isClosed)
            {
                return;
            }

            _isClosed = true;

            _outerConnectionTracker.Close();

            // If we don't have any outer connections, we can begin disposing ourselves. Otherwise, we wait.
            if (_outerConnectionTracker.IsEmpty)
            {
                Dispose();
            }
        }
    }

    public void CloseOuterConnection(IOuterConnection outerConnection)
    {
        // There's no need to set a closed inner connection on the outer connection unlike in OpenInnerConnection, so just forward it
        Detach(outerConnection);
    }

    public void Detach(IOuterConnection outerConnection)
    {
        if (!_outerConnectionTracker.Detach(outerConnection))
        {
            return;
        }

        if (_outerConnectionTracker.IsClosed && _outerConnectionTracker.IsEmpty)
        {
            // No connections remain and it is not possible to attach more. We can now dispose.
            Dispose();
        }
    }

    public IConnectionDataHandle RentDataFileMemory()
    {
        throw new InvalidOperationException("The connection is closed.");
    }

    public ConnectionDataSpanHandle RentDataFileSpan()
    {
        throw new InvalidOperationException("The connection is closed.");
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        return false;
    }

    public ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(false);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                DataFile.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
