using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class BlockConnectionRequestsScopeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var signalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        signalsMock.Setup(x => x.BlockAsyncRequestCreation());
        signalsMock.Setup(x => x.BlockRequestExecution());

        var scope = new BlockConnectionRequestsScope(signalsMock.Object);

        signalsMock.Verify(x => x.BlockAsyncRequestCreation(), Times.Once);
        signalsMock.Verify(x => x.BlockRequestExecution(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Dispose_Test()
    {
        var allowAsyncRequestCreation = true;
        var allowRequestExecution = true;

        var signalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        signalsMock.Setup(x => x.AllowAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = true);

        signalsMock.Setup(x => x.AllowRequestExecution())
            .Callback(() => allowRequestExecution = true);

        signalsMock.Setup(x => x.BlockAsyncRequestCreation())
            .Callback(() => allowAsyncRequestCreation = false);

        signalsMock.Setup(x => x.BlockRequestExecution())
            .Callback(() => allowRequestExecution = false);

        var scope = new BlockConnectionRequestsScope(signalsMock.Object);

        Assert.False(allowAsyncRequestCreation);
        Assert.False(allowRequestExecution);

        scope.Dispose();

        Assert.True(allowAsyncRequestCreation);
        Assert.True(allowRequestExecution);

        signalsMock.Verify(x => x.AllowAsyncRequestCreation(), Times.Once);
        signalsMock.Verify(x => x.AllowRequestExecution(), Times.Once);
        signalsMock.Verify(x => x.BlockAsyncRequestCreation(), Times.Once);
        signalsMock.Verify(x => x.BlockRequestExecution(), Times.Once);
    }

    [Fact]
    public void Dispose_MultipleDisposalsUnblockOnlyOnceTest()
    {
        var signalsMock = new Mock<IConnectionRequestSignals>(MockBehavior.Strict);

        signalsMock.Setup(x => x.AllowAsyncRequestCreation());
        signalsMock.Setup(x => x.AllowRequestExecution());
        signalsMock.Setup(x => x.BlockAsyncRequestCreation());
        signalsMock.Setup(x => x.BlockRequestExecution());

        var scope = new BlockConnectionRequestsScope(signalsMock.Object);

        scope.Dispose();
        scope.Dispose();
        scope.Dispose();

        signalsMock.Verify(x => x.AllowAsyncRequestCreation(), Times.Once);
        signalsMock.Verify(x => x.AllowRequestExecution(), Times.Once);
        signalsMock.Verify(x => x.BlockAsyncRequestCreation(), Times.Once);
        signalsMock.Verify(x => x.BlockRequestExecution(), Times.Once);
    }
}
