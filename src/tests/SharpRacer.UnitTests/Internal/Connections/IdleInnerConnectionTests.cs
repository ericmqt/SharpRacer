using Moq;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
public class IdleInnerConnectionTests
{
    [Fact]
    public void Ctor_Test()
    {
        var idleConnection = new IdleInnerConnection();

        Assert.Equal(-1, idleConnection.ConnectionId);
        Assert.Equal(0, idleConnection.Data.Length);
        Assert.NotNull(idleConnection.DataFile);
        Assert.Equal(default, idleConnection.IdleTimeout);
        Assert.Equal(SimulatorConnectionState.None, idleConnection.State);
    }

    [Fact]
    public void CloseOuterConnectionTest()
    {
        var idleConnection = new IdleInnerConnection();

        idleConnection.CloseOuterConnection(null!);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.Setup(x => x.Dispose());

        var idleConnection = new IdleInnerConnection(dataFileMock.Object);
        idleConnection.Dispose();

        dataFileMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void RentDataFileMemoryTest()
    {
        var idleConnection = new IdleInnerConnection();

        Assert.Throws<InvalidOperationException>(() => idleConnection.RentDataFileMemory());
    }

    [Fact]
    public void RentDataFileSpanTest()
    {
        var idleConnection = new IdleInnerConnection();

        Assert.Throws<InvalidOperationException>(() => idleConnection.RentDataFileSpan());
    }

    [Fact]
    public void WaitForDataReadyTest()
    {
        var idleConnection = new IdleInnerConnection();

        Assert.False(idleConnection.WaitForDataReady(default));
    }

    [Fact]
    public async Task WaitForDataReadyAsyncTest()
    {
        var idleConnection = new IdleInnerConnection();

        var waitResult = await idleConnection.WaitForDataReadyAsync(default);
        Assert.False(waitResult);
    }
}
