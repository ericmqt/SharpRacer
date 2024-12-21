namespace SharpRacer.Internal;

internal sealed class AsyncConnectionRequest
{
    private readonly long _requestCreationTimestamp;
    private readonly TimeProvider _timeProvider;

    public AsyncConnectionRequest(ISimulatorOuterConnection outerConnection, TaskCompletionSource completion, TimeSpan timeout)
        : this(outerConnection, completion, timeout, TimeProvider.System)
    {

    }

    public AsyncConnectionRequest(
        ISimulatorOuterConnection outerConnection,
        TaskCompletionSource completion,
        TimeSpan timeout,
        TimeProvider timeProvider)
    {
        OuterConnection = outerConnection ?? throw new ArgumentNullException(nameof(outerConnection));
        Completion = completion ?? throw new ArgumentNullException(nameof(completion));
        Timeout = timeout;

        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _requestCreationTimestamp = _timeProvider.GetTimestamp();
    }

    public TaskCompletionSource Completion { get; }
    public ISimulatorOuterConnection OuterConnection { get; }
    public TimeSpan Timeout { get; }

    public bool IsTimedOut()
    {
        if (Timeout == System.Threading.Timeout.InfiniteTimeSpan)
        {
            return false;
        }

        return _timeProvider.GetElapsedTime(_requestCreationTimestamp) >= Timeout;
    }
}
