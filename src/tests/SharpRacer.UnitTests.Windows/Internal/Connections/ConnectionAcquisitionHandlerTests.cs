using Moq;
using SharpRacer.Internal.Connections.Requests;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionAcquisitionHandlerTests
{
    [Fact]
    public void Ctor_ThrowsArgumentNullExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionSignalsMock = mocks.Create<IConnectionSignals>();

        Assert.Throws<ArgumentNullException>(() =>
            new ConnectionAcquisitionHandler(null!, requestManagerMock.Object, connectionSignalsMock.Object));

        Assert.Throws<ArgumentNullException>(() =>
            new ConnectionAcquisitionHandler(objectManagerMock.Object, null!, connectionSignalsMock.Object));

        Assert.Throws<ArgumentNullException>(() =>
            new ConnectionAcquisitionHandler(objectManagerMock.Object, requestManagerMock.Object, null!));
    }

    [Fact]
    public void CreateWorker_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var objectManagerMock = mocks.Create<IConnectionObjectManager>();
        var requestManagerMock = mocks.Create<IConnectionRequestManager>();
        var connectionSignalsMock = mocks.Create<IConnectionSignals>();

        var handler = new ConnectionAcquisitionHandler(objectManagerMock.Object, requestManagerMock.Object, connectionSignalsMock.Object);

        var connectionFactoryMock = mocks.Create<IOpenInnerConnectionFactory>();
        var connectionOwnerMock = mocks.Create<IOpenInnerConnectionOwner>();
        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var worker = handler.CreateWorker(
            connectionOwnerMock.Object,
            connectionFactoryMock.Object,
            connectionProviderMock.Object,
            dataReadyEventFactoryMock.Object);

        Assert.NotNull(worker);
    }
}
