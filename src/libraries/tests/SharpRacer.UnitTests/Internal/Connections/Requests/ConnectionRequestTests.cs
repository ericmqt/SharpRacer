using Microsoft.Extensions.Time.Testing;
using Moq;

namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestTests
{
    [Fact]
    public void Ctor_Test()
    {
        var timeout = TimeSpan.FromSeconds(5);

        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var scopeFactoryMock = new Mock<IConnectionRequestScopeFactory>(MockBehavior.Strict);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactoryMock.Object, TimeProvider.System);

        Assert.Equal(outerConnectionMock.Object, request.OuterConnection);
        Assert.Equal(timeout, request.Timeout);
    }

    [Fact]
    public void CreateExecutionState_Test()
    {
        var timeout = TimeSpan.FromSeconds(30);
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var outerConnectionMock = new Mock<IOuterConnection>(MockBehavior.Strict);
        var scopeFactoryMock = new Mock<IConnectionRequestScopeFactory>(MockBehavior.Strict);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactoryMock.Object, timeProvider);

        var state = request.CreateExecutionState();

        Assert.Equal(timeout, state.Timeout);
        Assert.False(state.IsTimedOut);

        // Advance time timeout+1s to prove ConnectionRequest.State is using the TimeProvider given to ConnectionRequest
        timeProvider.Advance(TimeSpan.FromSeconds(31));

        state.Update();
        Assert.True(state.IsTimedOut);
    }

    [Fact]
    public void EnterExecutionScope_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);

        var timeout = TimeSpan.FromSeconds(5);
        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactory, TimeProvider.System);

        Assert.Equal(0, scopeFactory.OpenScopeCount);
        Assert.Equal(0, scopeFactory.TotalScopesClosed);
        Assert.Equal(0, scopeFactory.TotalScopesOpened);

        using (var scope = request.EnterExecutionScope())
        {
            Assert.Equal(1, scopeFactory.OpenScopeCount);
            Assert.Equal(0, scopeFactory.TotalScopesClosed);
            Assert.Equal(1, scopeFactory.TotalScopesOpened);
        }

        Assert.Equal(0, scopeFactory.OpenScopeCount);
        Assert.Equal(1, scopeFactory.TotalScopesClosed);
        Assert.Equal(1, scopeFactory.TotalScopesOpened);
    }

    [Fact]
    public void Execute_ConnectionExceptionTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);

        IOpenInnerConnection? tryGetConnectionOutParam = null;
        var connectionExceptionMessage = "You can't do that!";
        var connectionException = new InvalidOperationException(connectionExceptionMessage);

        connectionProviderMock.Setup(x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam))
            .Throws(() => connectionException)
            .Verifiable(Times.Once());

        var request = new ConnectionRequest(outerConnectionMock.Object, Timeout.InfiniteTimeSpan, scopeFactory, TimeProvider.System);

        try
        {
            request.Execute(connectionProviderMock.Object);
            Assert.Fail("Execute failed to throw connection exception");
        }
        catch (Exception ex)
        {
            Assert.IsType(connectionException.GetType(), ex);
            Assert.Equal(connectionExceptionMessage, ex.Message);
        }

        Assert.Equal(1, scopeFactory.TotalScopesOpened);
        Assert.Equal(1, scopeFactory.TotalScopesClosed);
        Assert.Equal(0, scopeFactory.OpenScopeCount);

        mocks.Verify();
    }

    [Fact]
    public void Execute_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var timeout = TimeSpan.FromSeconds(5);
        IOpenInnerConnection? tryGetConnectionOutParam = innerConnectionMock.Object;

        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);

        scopeFactory.OnScopeEntering(() =>
        {
            // Ensure no execution scopes have been opened or closed yet 
            Assert.Equal(0, scopeFactory.OpenScopeCount);
            Assert.Equal(0, scopeFactory.TotalScopesClosed);
        });

        scopeFactory.OnScopeEntered(() =>
        {
            // Check TryGetConnection not called prior to entering execution scope
            connectionProviderMock.Verify(
                x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam),
                Times.Never());
        });

        scopeFactory.OnScopeExiting(() =>
        {
            Assert.Equal(1, scopeFactory.TotalScopesOpened);
            Assert.Equal(0, scopeFactory.TotalScopesClosed);

            connectionProviderMock.Verify(x => x.TryGetConnection(timeout, true, out tryGetConnectionOutParam), Times.Once());
        });

        connectionProviderMock.Setup(
            x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam))
            .Returns(true)
            .Verifiable(Times.Once);

        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(true)
            .Verifiable(Times.Once);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactory, TimeProvider.System);

        request.Execute(connectionProviderMock.Object);

        innerConnectionMock.Verify(x => x.Attach(outerConnectionMock.Object), Times.Once());

        Assert.Equal(0, scopeFactory.OpenScopeCount);
        Assert.Equal(1, scopeFactory.TotalScopesOpened);
        Assert.Equal(1, scopeFactory.TotalScopesClosed);

        mocks.Verify();
    }

    [Fact]
    public void Execute_RetryTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var timeout = TimeSpan.FromSeconds(30);
        var timeoutRemaining = timeout;

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var timeProviderAdvanceStep = TimeSpan.FromSeconds(10); // do not time out after advancing

        IOpenInnerConnection? tryGetConnectionOutParam = innerConnectionMock.Object;
        var attachReturnValue = false;

        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);

        scopeFactory.OnScopeExiting(() =>
        {
            if (!attachReturnValue)
            {
                // Exiting after the first attempt

                Assert.Equal(timeout, timeoutRemaining);
                connectionProviderMock.Verify(x => x.TryGetConnection(timeoutRemaining, true, out tryGetConnectionOutParam), Times.Once());

                // Setup the retry attempt
                timeProvider.Advance(timeProviderAdvanceStep);
                timeoutRemaining = timeout - timeProviderAdvanceStep;

                // Set return values for success
                attachReturnValue = true;

                // Ensure we have sufficient time remaining
                Assert.True(timeoutRemaining > TimeSpan.Zero);

                Assert.Equal(1, scopeFactory.TotalScopesOpened);
                Assert.Equal(0, scopeFactory.TotalScopesClosed);

            }
            else
            {
                // Retry attempt

                // Check we have opened an execution scope twice having already closed one, about to close another
                Assert.Equal(2, scopeFactory.TotalScopesOpened);
                Assert.Equal(1, scopeFactory.TotalScopesClosed);

                connectionProviderMock.Verify(
                    x => x.TryGetConnection(timeoutRemaining, true, out tryGetConnectionOutParam), Times.Once());
            }
        });

        connectionProviderMock.Setup(
            x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam))
            .Returns(true);

        // Set Attach to return false so it triggers a retry
        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(() => attachReturnValue);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactory, timeProvider);

        try
        {
            request.Execute(connectionProviderMock.Object);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Execute threw {ex.GetType()}: {ex.Message}");
        }

        innerConnectionMock.Verify(x => x.Attach(outerConnectionMock.Object), Times.Exactly(2));

        mocks.Verify();
    }

    [Fact]
    public void Execute_RetryTimedOutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var innerConnectionMock = mocks.Create<IOpenInnerConnection>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var timeout = TimeSpan.FromSeconds(5);
        var timeoutRemaining = timeout;

        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);
        var timeProviderAdvanceStep = TimeSpan.FromSeconds(10); // ensure we timeout after advancing

        IOpenInnerConnection? tryGetConnectionOutParam = innerConnectionMock.Object;
        var tryGetConnectionReturnValue = true;

        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);

        scopeFactory.OnScopeExiting(() =>
        {
            if (tryGetConnectionReturnValue)
            {
                // Exiting after the first attempt

                Assert.Equal(timeout, timeoutRemaining);
                connectionProviderMock.Verify(x => x.TryGetConnection(timeoutRemaining, true, out tryGetConnectionOutParam), Times.Once());

                // Setup the retry attempt
                timeProvider.Advance(timeProviderAdvanceStep);
                timeoutRemaining = timeout - timeProviderAdvanceStep;

                // TimeoutException is thrown based on TryGetConnection returning false
                tryGetConnectionReturnValue = false;

                // Ensure we have timed out for the retry attempt
                Assert.True(timeoutRemaining < TimeSpan.Zero);

                Assert.Equal(1, scopeFactory.TotalScopesOpened);
                Assert.Equal(0, scopeFactory.TotalScopesClosed);

            }
            else
            {
                // Retry attempt

                // Check we have opened an execution scope twice having already closed one, about to close another
                Assert.Equal(2, scopeFactory.TotalScopesOpened);
                Assert.Equal(1, scopeFactory.TotalScopesClosed);

                connectionProviderMock.Verify(
                    x => x.TryGetConnection(timeoutRemaining, false, out tryGetConnectionOutParam), Times.Once());
            }
        });

        connectionProviderMock.Setup(
            x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam))
            .Returns(() => tryGetConnectionReturnValue);

        // Set Attach to return false so it triggers a retry
        innerConnectionMock.Setup(x => x.Attach(It.IsAny<IOuterConnection>()))
            .Returns(false)
            .Verifiable(Times.Once);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactory, timeProvider);

        try
        {
            request.Execute(connectionProviderMock.Object);
            Assert.Fail("Execute returned without throwing TimeoutException");
        }
        catch (Exception ex)
        {
            Assert.IsType<TimeoutException>(ex);
        }

        innerConnectionMock.Verify(x => x.Attach(outerConnectionMock.Object), Times.Once());

        mocks.Verify();
    }

    [Fact]
    public void Execute_TimedOutTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionProviderMock = mocks.Create<IConnectionProvider>();
        var outerConnectionMock = mocks.Create<IOuterConnection>();
        var scopeFactory = new FakeConnectionRequestScopeFactory(mocks);
        var timeout = TimeSpan.FromSeconds(5);

        IOpenInnerConnection? tryGetConnectionOutParam = null;

        connectionProviderMock.Setup(
            x => x.TryGetConnection(It.IsAny<TimeSpan>(), It.IsAny<bool>(), out tryGetConnectionOutParam))
            .Returns(false)
            .Verifiable(Times.Once);

        var request = new ConnectionRequest(outerConnectionMock.Object, timeout, scopeFactory, TimeProvider.System);

        try
        {
            request.Execute(connectionProviderMock.Object);
            Assert.Fail("Execute returned without throwing TimeoutException");
        }
        catch (Exception ex)
        {
            Assert.IsType<TimeoutException>(ex);
        }

        connectionProviderMock.Verify(x => x.TryGetConnection(timeout, true, out tryGetConnectionOutParam), Times.Once());

        mocks.Verify();
    }
}
