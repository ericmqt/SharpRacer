using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Extensions;

namespace SharpRacer.Internal.Connections.Requests;
public class AsyncConnectionRequestTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        var timeout = TimeSpan.FromSeconds(30);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, default);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
        Assert.True(request.CanBeginConnectionAttempt);
        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        mocks.Verify();
    }

    [Fact]
    public void Ctor_InfiniteTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        var timeout = Timeout.InfiniteTimeSpan;

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, default);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
        Assert.True(request.CanBeginConnectionAttempt);
        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        mocks.Verify();
    }

    [Fact]
    public void Ctor_ZeroTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        var timeout = TimeSpan.Zero;

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, default);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
        Assert.False(request.CanBeginConnectionAttempt);
        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        mocks.Verify();
    }

    [Fact]
    public void Cancellation_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        var timeout = TimeSpan.FromSeconds(30);
        var cancellationSource = new CancellationTokenSource();

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, cancellationSource.Token);

        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        cancellationSource.Cancel();

        Assert.Equal(TaskStatus.Canceled, request.Completion.Task.Status);

        mocks.Verify();
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>().RequireDisposed();

        var timeout = TimeSpan.Zero;

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, default);

        request.Dispose();

        mocks.Verify();
    }

    [Fact]
    public void Dispose_MultipleInvocationsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>().RequireDisposed(Times.Once());

        var timeout = TimeSpan.Zero;

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, TimeProvider.System, default);

        request.Dispose();
        request.Dispose();
        request.Dispose();

        mocks.Verify();
    }

    [Fact]
    public async Task Dispose_SetsObjectDisposedExceptionOnCompletionIfNotCompletedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        request.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => request.Completion.Task);

        mocks.Verify();
    }

    [Fact]
    public void IsTimedOut_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        var timeout = TimeSpan.FromSeconds(30);

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, timeProvider, default);

        Assert.False(request.IsTimedOut());

        // Advance time 15 seconds
        timeProvider.Advance(TimeSpan.FromSeconds(15));

        Assert.False(request.IsTimedOut());

        // Advance time another 15 seconds to put timeout remaining at TimeSpan.Zero
        timeProvider.Advance(TimeSpan.FromSeconds(15));

        Assert.False(request.IsTimedOut());

        // Advance time 1 second to put timeout remaining at negative
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        Assert.True(request.IsTimedOut());
    }

    [Fact]
    public void IsTimedOut_ReturnsFalseWithInfiniteTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, Timeout.InfiniteTimeSpan, scopeMock.Object, TimeProvider.System, default);

        Assert.False(request.IsTimedOut());
    }

    [Fact]
    public void IsTimedOut_ReturnsFalseWithZeroTimeoutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.Zero, scopeMock.Object, TimeProvider.System, default);

        Assert.False(request.IsTimedOut());
    }

    [Fact]
    public void TryComplete_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeMock = mocks.Create<IDisposable>().RequireDisposed();

        // Setup the connection provider
        IOpenInnerConnection? tryGetConnectionOutParam = innerConnectionMock.Object;

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(true)
            .Verifiable(Times.Once);

        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<IAsyncConnectionRequest>(), out tryGetConnectionOutParam))
            .Returns(true)
            .Verifiable(Times.Once);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        // TryComplete
        Assert.True(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.RanToCompletion, request.Completion.Task.Status);

        connectionProviderMock.Verify(x => x.TryGetConnection(request, out tryGetConnectionOutParam), Times.Once);
        innerConnectionMock.Verify(x => x.Attach(request.OuterConnection), Times.Once());

        mocks.VerifyAll();
    }

    [Fact]
    public void TryComplete_AttachFailTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeMock = mocks.Create<IDisposable>().RequireNotDisposed();

        IOpenInnerConnection? tryGetConnectionOutParam = innerConnectionMock.Object;

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>())).Returns(false);

        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<IAsyncConnectionRequest>(), out tryGetConnectionOutParam))
            .Returns(true);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        // TryComplete
        Assert.False(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        connectionProviderMock.Verify(x => x.TryGetConnection(request, out tryGetConnectionOutParam), Times.Once);
        innerConnectionMock.Verify(x => x.Attach(request.OuterConnection), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public async Task TryComplete_ConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeMock = mocks.Create<IDisposable>().RequireDisposed();

        // Setup the connection provider
        IOpenInnerConnection? tryGetConnectionOutParam = null;

        var innerExceptionMessage = "You can't do that!";
        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<IAsyncConnectionRequest>(), out tryGetConnectionOutParam))
            .Throws(new InvalidOperationException(innerExceptionMessage));

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        // TryComplete
        Assert.True(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.Faulted, request.Completion.Task.Status);
        var connectionException = await Assert.ThrowsAsync<SimulatorConnectionException>(() => request.Completion.Task);

        connectionProviderMock.Verify(x => x.TryGetConnection(request, out tryGetConnectionOutParam), Times.Once);

        // Check inner exception is contained in SimulatorConnectionException
        Assert.NotNull(connectionException.InnerException);
        Assert.IsType<InvalidOperationException>(connectionException.InnerException);
        Assert.Equal(innerExceptionMessage, connectionException.InnerException.Message);

        mocks.Verify();
    }

    [Fact]
    public void TryComplete_DisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        request.Dispose();

        Assert.True(request.TryComplete(connectionProviderMock.Object));

        mocks.Verify();
    }

    [Fact]
    public void TryComplete_NoConnectionAvailableTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeMock = mocks.Create<IDisposable>().RequireNotDisposed();

        // Setup the connection provider
        IOpenInnerConnection? tryGetConnectionOutParam = null;

        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<IAsyncConnectionRequest>(), out tryGetConnectionOutParam))
            .Returns(false);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        // TryComplete
        Assert.False(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.WaitingForActivation, request.Completion.Task.Status);

        connectionProviderMock.Verify(x => x.TryGetConnection(request, out tryGetConnectionOutParam), Times.Once);

        mocks.Verify();
    }

    [Fact]
    public void TryComplete_TaskCompletionSourceIsAlreadyCanceledTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>().RequireDisposed();

        var cancellationSource = new CancellationTokenSource();

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, cancellationSource.Token);

        // Cancel the completion
        cancellationSource.Cancel();

        Assert.True(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.Canceled, request.Completion.Task.Status);

        mocks.Verify();
    }

    [Fact]
    public void TryComplete_TaskCompletionSourceIsAlreadyCompletedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>().RequireDisposed();

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.FromSeconds(30), scopeMock.Object, TimeProvider.System, default);

        // Complete the Completion
        request.Completion.TrySetResult();

        Assert.True(request.TryComplete(connectionProviderMock.Object));

        mocks.Verify();
    }

    [Fact]
    public async Task TryComplete_TimedOutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        var timeout = TimeSpan.FromSeconds(30);

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, timeout, scopeMock.Object, timeProvider, default);

        // Advance beyond timeout period
        timeProvider.Advance(timeout + TimeSpan.FromSeconds(1));

        Assert.True(request.TryComplete(connectionProviderMock.Object));

        await Assert.ThrowsAsync<TimeoutException>(() => request.Completion.Task);

        // Check scope was disposed
        mocks.Verify();
    }

    [Fact]
    public async Task TryComplete_ZeroTimeoutNoConnectionThrowsExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeMock = mocks.Create<IDisposable>();

        scopeMock.Setup(x => x.Dispose()).Verifiable(Times.Once);

        // Setup the connection provider
        IOpenInnerConnection? tryGetConnectionOutParam = null;

        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<IAsyncConnectionRequest>(), out tryGetConnectionOutParam))
            .Returns(false)
            .Verifiable(Times.Once); // TODO: Check this allows verify call with params

        var request = new AsyncConnectionRequest(
            outerConnectionMock.Object, TimeSpan.Zero, scopeMock.Object, TimeProvider.System, default);

        // TryComplete
        Assert.True(request.TryComplete(connectionProviderMock.Object));
        Assert.Equal(TaskStatus.Faulted, request.Completion.Task.Status);
        await Assert.ThrowsAsync<SimulatorConnectionException>(() => request.Completion.Task);

        connectionProviderMock.Verify(x => x.TryGetConnection(request, out tryGetConnectionOutParam));

        mocks.Verify();
    }
}
