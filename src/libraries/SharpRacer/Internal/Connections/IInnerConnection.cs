using SharpRacer.IO;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
internal interface IInnerConnection : IDisposable
{
    int ConnectionId { get; }
    IConnectionDataFile DataFile { get; }
    TimeSpan IdleTimeout { get; set; }
    SimulatorConnectionState State { get; }

    void CloseOuterConnection(IOuterConnection outerConnection);

    /// <summary>
    /// Stops tracking the outer connection. Used when outer connection doesn't need to transition to Closed state (because it's Disposing)
    /// </summary>
    /// <param name="outerConnection"></param>
    void Detach(IOuterConnection outerConnection);
    IConnectionDataHandle AcquireDataHandle();
    ConnectionDataSpanHandle AcquireDataSpanHandle();
    bool WaitForDataReady(CancellationToken cancellationToken);
    ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken);
}
