using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Interop;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
public class InnerConnectionFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        var factory = new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, timeProvider);

        Assert.NotNull(factory);
    }

    [Fact]
    public void Ctor_ThrowsOnNullArgumentsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        Assert.Throws<ArgumentNullException>(() =>
            new InnerConnectionFactory(null!, dataReadyEventFactoryMock.Object, timeProvider));

        Assert.Throws<ArgumentNullException>(() =>
            new InnerConnectionFactory(dataFileFactoryMock.Object, null!, timeProvider));

        Assert.Throws<ArgumentNullException>(() =>
            new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, null!));
    }

    [Fact]
    public void CreateClosedInnerConnectionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        var factory = new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, timeProvider);

        var dataFileMock = mocks.Create<IConnectionDataFile>();

        var closedInnerConnection = factory.CreateClosedInnerConnection(dataFileMock.Object);

        Assert.NotNull(closedInnerConnection);
        Assert.Equal(dataFileMock.Object, closedInnerConnection.DataFile);
    }

    [Fact]
    public void CreateClosedInnerConnection_FromOpenInnerConnectionTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        int connectionId = 79;

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileFactoryMock = mocks.Create<IConnectionDataFileFactory>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var timeProvider = new FakeTimeProvider();

        var factory = new InnerConnectionFactory(dataFileFactoryMock.Object, dataReadyEventFactoryMock.Object, timeProvider);

        var dataFileMock = mocks.Create<IConnectionDataFile>();
        dataFileMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var openInnerConnectionMock = mocks.Create<IOpenInnerConnection>();
        openInnerConnectionMock.SetupGet(x => x.ConnectionId).Returns(connectionId);
        openInnerConnectionMock.SetupGet(x => x.DataFile).Returns(dataFileMock.Object);

        var closedInnerConnection = factory.CreateClosedInnerConnection(openInnerConnectionMock.Object);

        Assert.NotNull(closedInnerConnection);
        Assert.Equal(dataFileMock.Object, closedInnerConnection.DataFile);
        Assert.Equal(connectionId, closedInnerConnection.ConnectionId);
    }
}
