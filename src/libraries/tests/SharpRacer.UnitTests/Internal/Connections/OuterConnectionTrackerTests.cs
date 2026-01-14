using Moq;

namespace SharpRacer.Internal.Connections;
public class OuterConnectionTrackerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var tracker = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.False(tracker.CloseOnEmpty);
        Assert.False(tracker.HasTrackedConnections);
        Assert.False(tracker.IsClosed);
        Assert.True(tracker.IsEmpty);

        tracker = new OuterConnectionTracker(closeOnEmpty: true);

        Assert.True(tracker.CloseOnEmpty);
        Assert.False(tracker.HasTrackedConnections);
        Assert.False(tracker.IsClosed);
        Assert.True(tracker.IsEmpty);
    }

    [Fact]
    public void Attach_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var tracker = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.True(tracker.Attach(outerConnectionMock.Object));
        Assert.False(tracker.IsClosed);
        Assert.False(tracker.IsEmpty);
    }

    [Fact]
    public void Attach_ReturnsTrueIfAlreadyTrackedTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>();

        var tracker = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.True(tracker.Attach(outerConnectionMock.Object));
        Assert.True(tracker.Attach(outerConnectionMock.Object));

        Assert.Single(tracker.DetachAll());
    }

    [Fact]
    public void Close_Test()
    {
        var obj = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.False(obj.IsClosed);

        obj.Close();

        Assert.True(obj.IsClosed);
    }

    [Fact]
    public void Detach_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detach1Result = connectionTracker.Detach(outerConnectionMock1.Object);

        Assert.True(detach1Result);
        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);

        var detach2Result = connectionTracker.Detach(outerConnectionMock2.Object);

        Assert.True(detach2Result);
        Assert.False(connectionTracker.IsClosed);
        Assert.True(connectionTracker.IsEmpty);
    }

    [Fact]
    public void Detach_CloseWhenEmptiedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detach1Result = connectionTracker.Detach(outerConnectionMock1.Object);

        Assert.True(detach1Result);
        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);

        var detach2Result = connectionTracker.Detach(outerConnectionMock2.Object);

        Assert.True(detach2Result);
        Assert.True(connectionTracker.IsClosed);
        Assert.True(connectionTracker.IsEmpty);
    }

    [Fact]
    public void Detach_CloseWhenEmptied_DoesNotOrphanIfNotTrackedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        // Attach only one
        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));

        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);
        Assert.True(connectionTracker.HasTrackedConnections);

        // Detach untracked connection
        Assert.False(connectionTracker.Detach(outerConnectionMock2.Object));

        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);
        Assert.True(connectionTracker.HasTrackedConnections);
    }

    [Fact]
    public void Detach_ReturnFalseIfNotTrackedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();
        var outerConnectionMock3 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: false);

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detachResult1 = connectionTracker.Detach(outerConnectionMock1.Object);

        Assert.True(detachResult1);
        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);

        var detachResult2 = connectionTracker.Detach(outerConnectionMock3.Object);

        Assert.False(detachResult2);
        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);
    }

    [Fact]
    public void DetachAll_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: false);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();
        var outerConnectionMock3 = mocks.Create<IOuterConnection>();
        var outerConnectionMock4 = mocks.Create<IOuterConnection>();

        // Attach 3
        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock3.Object));

        var detachedConnections = connectionTracker.DetachAll().ToList();

        Assert.Equal(3, detachedConnections.Count);

        Assert.Contains(outerConnectionMock1.Object, detachedConnections);
        Assert.Contains(outerConnectionMock2.Object, detachedConnections);
        Assert.Contains(outerConnectionMock3.Object, detachedConnections);

        Assert.False(connectionTracker.HasTrackedConnections);
        Assert.False(connectionTracker.IsClosed);
        Assert.True(connectionTracker.IsEmpty);

        Assert.True(connectionTracker.Attach(outerConnectionMock4.Object));

        Assert.True(connectionTracker.HasTrackedConnections);
        Assert.False(connectionTracker.IsClosed);
        Assert.False(connectionTracker.IsEmpty);
    }

    [Fact]
    public void DetachAll_CloseWhenEmptiedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var connectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();
        var outerConnectionMock3 = mocks.Create<IOuterConnection>();
        var outerConnectionMock4 = mocks.Create<IOuterConnection>();

        // Attach 3
        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock3.Object));

        var detachedConnections = connectionTracker.DetachAll().ToList();

        Assert.Equal(3, detachedConnections.Count);

        Assert.Contains(outerConnectionMock1.Object, detachedConnections);
        Assert.Contains(outerConnectionMock2.Object, detachedConnections);
        Assert.Contains(outerConnectionMock3.Object, detachedConnections);

        Assert.False(connectionTracker.HasTrackedConnections);
        Assert.True(connectionTracker.IsClosed);
        Assert.True(connectionTracker.IsEmpty);

        Assert.False(connectionTracker.Attach(outerConnectionMock4.Object));
        Assert.False(connectionTracker.HasTrackedConnections);
        Assert.True(connectionTracker.IsClosed);
        Assert.True(connectionTracker.IsEmpty);
    }
}
