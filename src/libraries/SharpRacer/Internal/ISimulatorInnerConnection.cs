using SharpRacer.IO;

namespace SharpRacer.Internal;
internal interface ISimulatorInnerConnection : IDisposable
{
    int ConnectionId { get; }
    ReadOnlySpan<byte> Data { get; }
    ISimulatorDataFile DataFile { get; }
    TimeSpan IdleTimeout { get; set; }
    SimulatorConnectionState State { get; }

    bool WaitForDataReady(CancellationToken cancellationToken);
    ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken);
}
