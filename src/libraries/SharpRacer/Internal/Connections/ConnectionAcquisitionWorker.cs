using System.Runtime.Versioning;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;

[SupportedOSPlatform("windows")]
internal sealed class ConnectionAcquisitionWorker : IConnectionAcquisitionWorker
{
    public static string ThreadName = "SharpRacer_ConnectionAcquisitionWorker";

    private readonly IOpenInnerConnectionFactory _connectionFactory;
    private readonly IConnectionProvider _connectionProvider;
    private readonly IDataReadyEventFactory _dataReadyEventFactory;
    private readonly IOpenInnerConnectionOwner _innerConnectionOwner;
    private readonly IConnectionRequestManager _requestManager;
    private readonly IConnectionAcquisitionHandler _workerHandler;

    public ConnectionAcquisitionWorker(
        IConnectionAcquisitionHandler workerHandler,
        IOpenInnerConnectionFactory connectionFactory,
        IConnectionRequestManager requestManager,
        IConnectionProvider connectionProvider,
        IOpenInnerConnectionOwner innerConnectionOwner,
        IDataReadyEventFactory dataReadyEventFactory)
    {
        _workerHandler = workerHandler;
        _connectionFactory = connectionFactory;
        _requestManager = requestManager;
        _connectionProvider = connectionProvider;
        _innerConnectionOwner = innerConnectionOwner;
        _dataReadyEventFactory = dataReadyEventFactory;

        Thread = new Thread(OnThreadStart)
        {
            Name = ThreadName,
            IsBackground = true
        };
    }

    public TimeSpan DataReadyWaitTimeout { get; set; } = TimeSpan.FromMilliseconds(32);
    public Thread Thread { get; }

    public void Start()
    {
        Thread.Start();
    }

    internal static void Run(
        IConnectionAcquisitionHandler handler,
        IOpenInnerConnectionFactory connectionFactory,
        IOpenInnerConnectionOwner connectionOwner,
        IConnectionRequestManager requestManager,
        IConnectionProvider connectionProvider,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeSpan dataReadyWaitTimeout)
    {
        // Wait on the data-ready event for as long as we have pending connection requests waiting
        using (var dataReadyEvent = dataReadyEventFactory.CreateAutoResetEvent())
        {
            while (!dataReadyEvent.WaitOne(dataReadyWaitTimeout))
            {
                if (requestManager.HasPendingRequests())
                {
                    requestManager.ProcessAsyncRequestQueue(connectionProvider, force: false);
                }
                else if (handler.TryAbort())
                {
                    return;
                }
            }
        }

        // Data-ready event signaled, so create a connection and exit
        try
        {
            var connection = connectionFactory.Create(connectionOwner);

            handler.SetConnection(connectionProvider, connection);
        }
        catch (Exception ex)
        {
            var connectionException = ConnectionExceptionHelper.GetConnectionException(
                "Failed to connect to the simulator. See inner exception for details.", ex);

            handler.SetConnectionException(connectionProvider, connectionException);
        }
    }

    private void OnThreadStart()
    {
        Run(_workerHandler,
            _connectionFactory,
            _innerConnectionOwner,
            _requestManager,
            _connectionProvider,
            _dataReadyEventFactory,
            DataReadyWaitTimeout);
    }
}
