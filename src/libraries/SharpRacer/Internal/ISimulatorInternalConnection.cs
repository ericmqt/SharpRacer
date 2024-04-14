namespace SharpRacer.Internal;
internal interface ISimulatorInternalConnection : IDisposable
{
    int ConnectionId { get; }
    ReadOnlySpan<byte> Data { get; }
    TimeSpan IdleTimeout { get; set; }
    SimulatorConnectionState State { get; }

    bool WaitForDataReady(CancellationToken cancellationToken);
    ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken);
}
