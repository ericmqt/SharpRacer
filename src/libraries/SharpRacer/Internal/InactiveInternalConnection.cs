
using SharpRacer.IO;

namespace SharpRacer.Internal;
internal class InactiveInternalConnection : ISimulatorInnerConnection
{
    private readonly ISimulatorDataFile _dataFile;

    public InactiveInternalConnection(SimulatorConnectionState state)
        : this(new FrozenDataFile([]), state)
    {

    }

    public InactiveInternalConnection(ISimulatorDataFile dataFile, SimulatorConnectionState state)
    {
        _dataFile = dataFile ?? throw new ArgumentNullException(nameof(dataFile));
        State = state;
    }

    public int ConnectionId { get; } = -1;
    public ReadOnlySpan<byte> Data => _dataFile.Span;
    public ISimulatorDataFile DataFile => _dataFile;
    public TimeSpan IdleTimeout { get; set; }
    public SimulatorConnectionState State { get; }

    public void Dispose()
    {
        _dataFile.Dispose();
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        return false;
    }

    public ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(false);
    }
}
