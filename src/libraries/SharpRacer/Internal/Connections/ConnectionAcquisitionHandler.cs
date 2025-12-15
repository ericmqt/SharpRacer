using System.Runtime.Versioning;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;

internal sealed class ConnectionAcquisitionHandler : IConnectionAcquisitionHandler
{
    private readonly IConnectionSignals _connectionSignals;
    private readonly IConnectionObjectManager _objectManager;
    private readonly IConnectionRequestManager _requestManager;

    public ConnectionAcquisitionHandler(
        IConnectionObjectManager connectionObjectManager,
        IConnectionRequestManager requestManager,
        IConnectionSignals connectionSignals)
    {
        _objectManager = connectionObjectManager ?? throw new ArgumentNullException(nameof(connectionObjectManager));
        _requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
        _connectionSignals = connectionSignals ?? throw new ArgumentNullException(nameof(connectionSignals));
    }

    [SupportedOSPlatform("windows")]
    public IConnectionAcquisitionWorker CreateWorker(
        IOpenInnerConnectionOwner connectionOwner,
        IOpenInnerConnectionFactory connectionFactory,
        IConnectionProvider connectionProvider,
        IDataReadyEventFactory dataReadyEventFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionOwner);

        return new ConnectionAcquisitionWorker(
            this, connectionFactory, _requestManager, connectionProvider, connectionOwner, dataReadyEventFactory);
    }

    public void SetConnection(IConnectionProvider connectionProvider, IOpenInnerConnection innerConnection)
    {
        _objectManager.SetConnection(innerConnection);

        // Ensure every pending request gets processed. Otherwise, if the queue was already in the middle of being processed, there is a
        // small chance that some requests would be left uncompleted because the queue is no longer being periodically processed while
        // DataReadyCallback waits for the event to fire.
        _requestManager.ProcessAsyncRequestQueue(connectionProvider, force: true);

        innerConnection.StartWorkerThread();
    }

    public void SetConnectionException(IConnectionProvider connectionProvider, SimulatorConnectionException connectionException)
    {
        // Pause new connections while exception is propagated to current set of connection waiters
        using var pauseNewRequestsScope = _requestManager.CreateRequestBlockingScope();

        _objectManager.SetConnectionException(connectionException);

        // Ensure every pending request gets processed. Otherwise, if the queue was already in the middle of being processed, there is a
        // small chance that some requests would be left uncompleted because the queue is no longer being periodically processed while
        // DataReadyCallback waits for the event to fire.
        _requestManager.ProcessAsyncRequestQueue(connectionProvider, force: true);

        // Synchronous Connect calls should have cleared by now, but wait if we have to anyways just in case
        if (_requestManager.HasPendingRequests())
        {
            // Wait for pending connections to drop to zero
            var spinner = new SpinWait();

            do
            {
                spinner.SpinOnce();
            }
            while (_requestManager.HasPendingRequests());
        }

        // Everybody is out, stop signaling an exception
        _objectManager.ClearConnectionException();

        // Allow another connection attempt
        _connectionSignals.SetCreateConnectionSignal();
    }

    public bool TryAbort()
    {
        using var newRequestsLockout = _requestManager.CreateRequestBlockingScope();

        // Double-check there are still no pending requests now that we've stopped new requests from entering the queue
        if (!_requestManager.HasPendingRequests())
        {
            // Allow the connection creation process to start again because this thread is about to exit and will not be available to
            // handle future new requests.

            _connectionSignals.SetCreateConnectionSignal();

            return true;
        }

        return false;
    }
}
