using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class AsyncConnectionRequestScopeTests
{
    [Fact]
    public void Ctor_Test()
    {
        var signalsMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        signalsMock.Setup(x => x.IncrementQueuedAsyncRequestCount());

        var scope = new AsyncConnectionRequestScope(signalsMock.Object);

        signalsMock.Verify(x => x.IncrementQueuedAsyncRequestCount(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Dispose_Test()
    {
        var signalsMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        signalsMock.Setup(x => x.DecrementQueuedAsyncRequestCount());
        signalsMock.Setup(x => x.IncrementQueuedAsyncRequestCount());

        var scope = new AsyncConnectionRequestScope(signalsMock.Object);

        scope.Dispose();

        signalsMock.Verify(x => x.IncrementQueuedAsyncRequestCount(), Times.Once);
        signalsMock.Verify(x => x.DecrementQueuedAsyncRequestCount(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Dispose_MultipleDisposalsInvokeCallbackOnlyOnceTest()
    {
        var signalsMock = new Mock<IPendingRequestCounter>(MockBehavior.Strict);
        signalsMock.Setup(x => x.DecrementQueuedAsyncRequestCount());
        signalsMock.Setup(x => x.IncrementQueuedAsyncRequestCount());

        var scope = new AsyncConnectionRequestScope(signalsMock.Object);

        scope.Dispose();
        scope.Dispose();
        scope.Dispose();

        signalsMock.Verify(x => x.IncrementQueuedAsyncRequestCount(), Times.Once);
        signalsMock.Verify(x => x.DecrementQueuedAsyncRequestCount(), Times.Once);
        signalsMock.VerifyNoOtherCalls();
    }
}
