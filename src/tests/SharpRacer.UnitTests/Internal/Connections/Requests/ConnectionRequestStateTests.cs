using Microsoft.Extensions.Time.Testing;

namespace SharpRacer.Internal.Connections.Requests;
public class ConnectionRequestStateTests
{
    [Fact]
    public void Ctor_InfiniteTimeoutTest()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = Timeout.InfiniteTimeSpan;

        var state = new ConnectionRequest.State(timeout, timeProvider);

        Assert.True(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);
    }

    [Fact]
    public void Ctor_NonZeroTimeoutTest()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = TimeSpan.FromSeconds(3);

        var state = new ConnectionRequest.State(timeout, timeProvider);

        Assert.Equal(timeout, state.Timeout);
        Assert.Equal(timeout, state.TimeoutRemaining);
        Assert.False(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);
    }

    [Fact]
    public void Ctor_ZeroTimeoutTest()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = TimeSpan.Zero;

        var state = new ConnectionRequest.State(timeout, timeProvider);

        Assert.Equal(timeout, state.Timeout);
        Assert.Equal(timeout, state.TimeoutRemaining);
        Assert.False(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.False(state.CanBeginConnectionAttempt);
    }

    [Fact]
    public void Update_Test()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = TimeSpan.FromSeconds(30);
        var state = new ConnectionRequest.State(timeout, timeProvider);

        // Update without changing time
        state.Update();

        Assert.Equal(timeout, state.Timeout);
        Assert.Equal(timeout, state.TimeoutRemaining);
        Assert.False(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);

        // Advance fifteen seconds
        var timeStep = TimeSpan.FromSeconds(15);
        timeProvider.Advance(timeStep);

        state.Update();

        var expectedTimeoutRemaining = timeout - timeStep;
        Assert.Equal(expectedTimeoutRemaining, state.TimeoutRemaining);
        Assert.Equal(timeout, state.Timeout);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);

        // Advance another fifteen seconds to reach TimeoutRemaining = TimeSpan.Zero
        timeProvider.Advance(timeStep);

        state.Update();

        Assert.Equal(TimeSpan.Zero, state.TimeoutRemaining);
        Assert.False(state.IsTimedOut);

        // CanBeginConnectionAttempt should have transitioned from true -> false
        Assert.False(state.CanBeginConnectionAttempt);

        // Advance another fifteen seconds to reach TimeoutRemaining = -15s
        timeProvider.Advance(timeStep);

        state.Update();

        Assert.Equal(TimeSpan.Zero - timeStep, state.TimeoutRemaining);
        Assert.True(state.IsTimedOut);
        Assert.False(state.CanBeginConnectionAttempt);
    }

    [Fact]
    public void Update_InfiniteTimeoutTest()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = Timeout.InfiniteTimeSpan;
        var state = new ConnectionRequest.State(timeout, timeProvider);

        // Update without changing the time and check that nothing has changed
        Assert.True(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);
        Assert.Equal(Timeout.InfiniteTimeSpan, state.TimeoutRemaining);

        // Advance time and check the state is unchanged
        timeProvider.Advance(TimeSpan.FromDays(999));

        Assert.True(state.IsTimeoutInfinite);
        Assert.False(state.IsTimedOut);
        Assert.True(state.CanBeginConnectionAttempt);
        Assert.Equal(Timeout.InfiniteTimeSpan, state.TimeoutRemaining);
    }

    [Fact]
    public void Update_ZeroTimeoutTest()
    {
        var timeProvider = new FakeTimeProvider(DateTimeOffset.UtcNow);

        var timeout = TimeSpan.Zero;
        var state = new ConnectionRequest.State(timeout, timeProvider);

        // Update without changing the time and check that IsTimedOut is still false (because no time passed)
        state.Update();

        Assert.False(state.IsTimedOut);
        Assert.Equal(timeout, state.TimeoutRemaining);

        // Advance time and check that IsTimedOut is true
        timeProvider.Advance(TimeSpan.FromSeconds(1));

        state.Update();

        Assert.True(state.IsTimedOut);
        Assert.True(state.TimeoutRemaining < TimeSpan.Zero);
    }
}
