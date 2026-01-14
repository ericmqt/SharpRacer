using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
public class InnerConnectionFactoryTests
{
    [Fact]
    public void Create_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        var factory = new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, timeProvider);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        int connectionId = 79;

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.SetupGet(x => x.Memory).Returns(memoryObj);

        dataFileFactoryMock.Setup(x => x.Create()).Returns(dataFileMock.Object);

        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        connectionOwnerMock.Setup(x => x.NewConnectionId()).Returns(connectionId);

        var connection = factory.Create(connectionOwnerMock.Object);

        Assert.NotNull(connection);
        Assert.Equal(connectionId, connection.ConnectionId);
        Assert.Equal(dataFileMock.Object, connection.DataFile);
        Assert.Equal(SimulatorConnectionState.Open, connection.State);
    }

    [Fact]
    public void Create_DisposesDataFileOnExceptionThrownTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        var factory = new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, timeProvider);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.SetupGet(x => x.Memory).Returns(memoryObj);
        dataFileMock.Setup(x => x.Dispose());

        dataFileFactoryMock.Setup(x => x.Create()).Returns(dataFileMock.Object);

        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();

        // Throw an exception from within the OpenInnerConnection constructor, causing the Create() method to dispose the data file
        connectionOwnerMock.Setup(x => x.NewConnectionId()).Throws(new InvalidOperationException());

        Assert.Throws<InvalidOperationException>(() => factory.Create(connectionOwnerMock.Object));

        dataFileMock.Verify(x => x.Dispose(), Times.Once);
    }
}
