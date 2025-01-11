namespace SharpRacer.Internal.Connections;
internal enum ConnectionSignalWaitResult : ushort
{
    ConnectionAvailable = 0,
    ConnectionException = 1,
    CreateConnection = 2,
    WaitTimeout = WaitHandle.WaitTimeout
}
