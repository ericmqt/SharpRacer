namespace SharpRacer.Internal;
partial class ConnectionPool
{
    private sealed class ConnectionWaitHandles
    {
        private readonly WaitHandle[] _waitHandlesAllowCreateConnection;
        private readonly WaitHandle[] _waitHandlesConnectionTest;

        public ConnectionWaitHandles()
        {
            CreateConnectionSignal = new AutoResetEvent(initialState: true);
            ConnectionAvailableSignal = new ManualResetEvent(initialState: false);
            ConnectionExceptionSignal = new ManualResetEvent(initialState: false);

            // Order of wait handles is important. If two or more are signaled during a call to WaitHandle.WaitAny or WaitAll, the handle with
            // the lowest index is returned.

            _waitHandlesAllowCreateConnection = new WaitHandle[3];
            _waitHandlesAllowCreateConnection[ConnectionAvailableWaitIndex] = ConnectionAvailableSignal;
            _waitHandlesAllowCreateConnection[ConnectionExceptionWaitIndex] = ConnectionExceptionSignal;
            _waitHandlesAllowCreateConnection[CreateConnectionWaitIndex] = CreateConnectionSignal;

            // Exclude CreateConnectionSignal from the connection test wait handle array
            _waitHandlesConnectionTest = new WaitHandle[2];
            _waitHandlesConnectionTest[ConnectionAvailableWaitIndex] = ConnectionAvailableSignal;
            _waitHandlesConnectionTest[ConnectionExceptionWaitIndex] = ConnectionExceptionSignal;
        }

        public const int ConnectionAvailableWaitIndex = 0;
        public const int ConnectionExceptionWaitIndex = 1;
        public const int CreateConnectionWaitIndex = 2;

        public ManualResetEvent ConnectionAvailableSignal { get; }
        public ManualResetEvent ConnectionExceptionSignal { get; }
        public AutoResetEvent CreateConnectionSignal { get; }

        public int WaitAny(bool allowCreateConnection, TimeSpan waitTimeout)
        {
            return allowCreateConnection
                ? WaitHandle.WaitAny(_waitHandlesAllowCreateConnection, waitTimeout)
                : WaitHandle.WaitAny(_waitHandlesConnectionTest, waitTimeout);
        }
    }
}
