using Moq;

namespace SharpRacer.Internal.Connections;
public class OuterConnectionTrackerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.CanAttach);
    }

    [Fact]
    public void Attach_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();
        Assert.True(connectionTracker.Attach(outerConnectionMock.Object));
    }

    [Fact]
    public void Attach_ReturnsTrueIfAlreadyTrackedTest()
    {
        var outerConnectionMock = new Mock<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.Attach(outerConnectionMock.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock.Object));

        Assert.Single(connectionTracker.DetachAll());
    }

    [Fact]
    public void Detach_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detachResult = connectionTracker.Detach(outerConnectionMock1.Object, out var isInnerConnectionOrphaned);

        Assert.True(detachResult);
        Assert.False(isInnerConnectionOrphaned);
        Assert.True(connectionTracker.CanAttach);
    }

    [Fact]
    public void Detach_DoesNotOrphanIfConnectionNotTrackedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));

        var detachResult = connectionTracker.Detach(outerConnectionMock2.Object, out var isInnerConnectionOrphaned);

        Assert.False(detachResult);
        Assert.False(isInnerConnectionOrphaned);
        Assert.True(connectionTracker.CanAttach);
    }

    [Fact]
    public void Detach_OrphanTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detachResult = connectionTracker.Detach(outerConnectionMock1.Object, out var isInnerConnectionOrphaned);

        Assert.True(detachResult);
        Assert.False(isInnerConnectionOrphaned);
        Assert.True(connectionTracker.CanAttach);

        detachResult = connectionTracker.Detach(outerConnectionMock2.Object, out isInnerConnectionOrphaned);

        Assert.True(detachResult);
        Assert.True(isInnerConnectionOrphaned);
        Assert.False(connectionTracker.CanAttach);

        Assert.False(connectionTracker.Attach(outerConnectionMock1.Object));
    }

    [Fact]
    public void DetachAll_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var outerConnectionMock1 = mocks.Create<IOuterConnection>();
        var outerConnectionMock2 = mocks.Create<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        Assert.True(connectionTracker.Attach(outerConnectionMock1.Object));
        Assert.True(connectionTracker.Attach(outerConnectionMock2.Object));

        var detachedObjects = connectionTracker.DetachAll().ToArray();

        Assert.Equal(2, detachedObjects.Length);
        Assert.Contains(outerConnectionMock1.Object, detachedObjects);
        Assert.Contains(outerConnectionMock2.Object, detachedObjects);

        Assert.False(connectionTracker.CanAttach);
        Assert.False(connectionTracker.Attach(outerConnectionMock1.Object));
    }

    [Fact]
    public void DisableAttach_Test()
    {
        var outerConnectionMock = new Mock<IOuterConnection>();

        var connectionTracker = new OuterConnectionTracker();

        connectionTracker.DisableAttach();

        Assert.False(connectionTracker.CanAttach);
        Assert.False(connectionTracker.Attach(outerConnectionMock.Object));
        Assert.Empty(connectionTracker.DetachAll());
    }
}
