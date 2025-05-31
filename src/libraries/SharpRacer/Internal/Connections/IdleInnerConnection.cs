using SharpRacer.IO;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
internal sealed class IdleInnerConnection : IInnerConnection
{
    public IdleInnerConnection()
        : this(new EmptyConnectionDataFile())
    {

    }

    public IdleInnerConnection(IConnectionDataFile dataFile)
    {
        DataFile = dataFile;
    }

    public int ConnectionId { get; } = -1;
    public ReadOnlySpan<byte> Data => DataFile.Memory.Span;
    public IConnectionDataFile DataFile { get; }
    public TimeSpan IdleTimeout { get; set; }
    public SimulatorConnectionState State { get; } = SimulatorConnectionState.None;

    public void CloseOuterConnection(IOuterConnection outerConnection)
    {

    }

    public void Detach(IOuterConnection outerConnection)
    {

    }

    public void Dispose()
    {
        DataFile.Dispose();
    }

    public IDataFileMemoryOwner RentDataFileMemory()
    {
        throw new InvalidOperationException("The connection is not open.");
    }

    public DataFileSpanOwner RentDataFileSpan()
    {
        throw new InvalidOperationException("The connection is not open.");
    }

    public bool WaitForDataReady(CancellationToken cancellationToken)
    {
        // TODO: Or throw...?
        return false;
    }

    public ValueTask<bool> WaitForDataReadyAsync(CancellationToken cancellationToken)
    {
        // TODO: Or throw...?
        return ValueTask.FromResult(false);
    }
}
