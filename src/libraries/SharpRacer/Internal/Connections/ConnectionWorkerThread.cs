using System.Runtime.InteropServices;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
internal sealed class ConnectionWorkerThread : IConnectionWorkerThread
{
    public static string ThreadName = "SharpRacer_ConnectionWorkerThread";

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IDataReadyEventFactory _dataReadyEventFactory;
    private bool _isDisposed;
    private readonly IConnectionWorkerThreadOwner _owner;
    private readonly TimeProvider _timeProvider;

    public ConnectionWorkerThread(
        IConnectionWorkerThreadOwner owner,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeProvider timeProvider)
    {
        _owner = owner;
        _dataReadyEventFactory = dataReadyEventFactory;
        _timeProvider = timeProvider;

        Thread = new Thread(OnThreadStart)
        {
            Name = ThreadName,
            IsBackground = true
        };

        _cancellationTokenSource = new CancellationTokenSource();
    }

    public Thread Thread { get; }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        Thread.Start();
    }

    public void Stop()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _cancellationTokenSource.Cancel();
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _cancellationTokenSource.Cancel();

                // Wait for thread to finish up if it is running. Otherwise, disposing the cancellation source can throw an
                // ObjectDisposedException in the thread.
                if ((Thread.ThreadState & ThreadState.Unstarted) != ThreadState.Unstarted)
                {
                    Thread.Join();
                }

                _cancellationTokenSource.Dispose();
            }

            _isDisposed = true;
        }
    }

    private void OnThreadStart()
    {
        Run(_owner, _dataReadyEventFactory, _timeProvider, _cancellationTokenSource.Token);
    }

    internal static void Run(
        IConnectionWorkerThreadOwner owner,
        IDataReadyEventFactory dataReadyEventFactory,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Wait at most two frames before checking if sim has closed and beginning to evaluate the idle timeout.
        const int DataReadyWaitTimeout = 2 * 16;

        // Order of wait handles ensures cancellation takes precedence over data-ready signal
        const int WaitIndex_Cancellation = 0;
        const int WaitIndex_DataReady = 1;

        var lastDataReadyTimestamp = timeProvider.GetTimestamp();
        int simulatorStatus;

        // Set up the wait handles
        using var dataReadyEvent = dataReadyEventFactory.CreateAutoResetEvent();

        int waitIndex;

        var waitHandles = new WaitHandle[2];

        waitHandles[WaitIndex_Cancellation] = cancellationToken.WaitHandle;
        waitHandles[WaitIndex_DataReady] = dataReadyEvent;

        while (true)
        {
            waitIndex = WaitHandle.WaitAny(waitHandles, DataReadyWaitTimeout);

            simulatorStatus = MemoryMarshal.Read<int>(owner.Data.Slice(DataFileHeader.FieldOffsets.Status, sizeof(int)));

            if (waitIndex == WaitIndex_DataReady)
            {
                lastDataReadyTimestamp = timeProvider.GetTimestamp();

                owner.OnDataReady();
            }
            else if (waitIndex == WaitIndex_Cancellation)
            {
                // Cancellation was requested.

                owner.OnWorkerThreadExit(canceled: true);

                return;
            }
            else if (waitIndex == WaitHandle.WaitTimeout)
            {
                // Exit the thread and ultimately close the connection if the simulator indicates it has exited, or if the idle timeout
                // period has elapsed without detecting the simulator has exited (e.g. simulator froze for too long or crashed without
                // updating the field). If neither apply, let the loop continue until conditions are met or data-ready resumes.

                if (simulatorStatus == 0 || timeProvider.GetElapsedTime(lastDataReadyTimestamp) >= owner.IdleTimeout)
                {
                    owner.OnWorkerThreadExit(canceled: false);

                    return;
                }
            }
        }
    }
}
