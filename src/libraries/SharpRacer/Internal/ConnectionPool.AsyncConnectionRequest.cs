namespace SharpRacer.Internal;
partial class ConnectionPool
{
    private sealed class AsyncConnectionRequest
    {
        private readonly long _requestCreationTimestamp;

        public AsyncConnectionRequest(
            SimulatorConnection outerConnection,
            TaskCompletionSource completion,
            TimeSpan timeout)
        {
            OuterConnection = outerConnection ?? throw new ArgumentNullException(nameof(outerConnection));
            Completion = completion ?? throw new ArgumentNullException(nameof(completion));
            Timeout = timeout;

            _requestCreationTimestamp = TimeProvider.System.GetTimestamp();
        }

        public TaskCompletionSource Completion { get; }
        public SimulatorConnection OuterConnection { get; }
        public TimeSpan Timeout { get; }

        public bool IsTimedOut()
        {
            if (Timeout == System.Threading.Timeout.InfiniteTimeSpan)
            {
                return false;
            }

            return TimeProvider.System.GetElapsedTime(_requestCreationTimestamp) >= Timeout;
        }
    }
}
