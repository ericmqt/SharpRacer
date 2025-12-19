using System.Runtime.InteropServices;
using Moq;
using SharpRacer.Interop;
using SharpRacer.IO;

namespace SharpRacer.Internal.Connections;

internal class FakeConnectionWorkerThreadOwner : IConnectionWorkerThreadOwner
{
    private readonly Mock<IConnectionWorkerThreadOwner> _mock;
    private readonly Memory<byte> _data;

    public FakeConnectionWorkerThreadOwner(Mock<IConnectionWorkerThreadOwner> mock)
    {
        _data = new byte[128];

        SetSimulatorStatus(0);
        _mock = mock;
    }

    public TimeSpan IdleTimeout => _mock.Object.IdleTimeout;

    public ConnectionDataSpanHandle AcquireDataSpanHandle()
    {
        return ConnectionDataSpanHandle.Ownerless(_data.Span);
    }

    public void OnDataReady()
        => _mock.Object.OnDataReady();

    public void OnWorkerThreadExit(bool canceled)
        => _mock.Object.OnWorkerThreadExit(canceled);

    public void SetSimulatorStatus(int status)
    {
        MemoryMarshal.Write(_data.Span[DataFileHeader.FieldOffsets.Status..], status);
    }
}
