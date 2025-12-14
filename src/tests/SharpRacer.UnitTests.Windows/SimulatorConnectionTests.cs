using Moq;
using SharpRacer.Internal.Connections;

namespace SharpRacer;
public class SimulatorConnectionTests
{
    [Fact]
    public void Ctor_Test()
    {
        var connection = new SimulatorConnection();

        Assert.Equal(SimulatorConnectionState.None, connection.State);
        Assert.False(connection.CanRead);
        Assert.Equal(0, connection.Data.Length);
        Assert.True(connection.Data.SequenceEqual([]));
        Assert.Empty(connection.DataVariables);
    }

    [Fact]
    public void Ctor_ConnectionManagerTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionManagerMock = mocks.Create<IConnectionManager>();

        var connection = new SimulatorConnection(connectionManagerMock.Object);

        Assert.Equal(SimulatorConnectionState.None, connection.State);
        Assert.False(connection.CanRead);
        Assert.Equal(0, connection.Data.Length);
        Assert.True(connection.Data.SequenceEqual([]));
        Assert.Empty(connection.DataVariables);
    }

    [Fact]
    public void Ctor_ThrowsOnNullConnectionManagerTest()
    {
        Assert.Throws<ArgumentNullException>(() => new SimulatorConnection(connectionManager: null!));
    }

    [Fact]
    public void Close_ThrowsIfStateEqualsNoneTest()
    {
        var connection = new SimulatorConnection();

        Assert.Throws<InvalidOperationException>(() => connection.Close());
    }

    [Fact]
    public void CreateDataReader_ThrowsIfNotOpenTest()
    {
        var connection = new SimulatorConnection();

        Assert.Throws<InvalidOperationException>(() => connection.CreateDataReader());
    }

    [Fact]
    public void WaitForDataReady_ThrowsIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        Assert.Throws<ObjectDisposedException>(() => connection.WaitForDataReady());
    }

    [Fact]
    public async Task WaitForDataReadyAsync_ThrowsIfDisposedTest()
    {
        var connection = new SimulatorConnection();
        connection.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await connection.WaitForDataReadyAsync());
    }
}
