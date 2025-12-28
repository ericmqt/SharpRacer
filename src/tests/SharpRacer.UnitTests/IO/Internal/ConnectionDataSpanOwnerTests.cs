using Moq;

namespace SharpRacer.IO.Internal;

public class ConnectionDataSpanOwnerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        var spanOwner = new ConnectionDataSpanOwner(spanFactoryMock.Object, dataFileLifetimeMock.Object);

        dataFileLifetimeMock.Verify(x => x.AcquireLifetimeHandle(), Times.Once);
    }

    [Fact]
    public void Ctor_ThrowsOnNullArgumentsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        Assert.Throws<ArgumentNullException>(() => new ConnectionDataSpanOwner(null!, dataFileLifetimeMock.Object));
        Assert.Throws<ArgumentNullException>(() => new ConnectionDataSpanOwner(spanFactoryMock.Object, null!));
    }

    [Fact]
    public void AcquireSpanHandle_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        // Acquire a handle
        var spanHandle = spanOwner.AcquireSpanHandle();

        Assert.NotEqual(ConnectionDataSpanHandleToken.Zero, spanHandle.Token);
        Assert.True(spanHandle.Span.SequenceEqual(fakeSpanFactoryMemory.Span));
        Assert.True(spanHandle.IsOwned);
        Assert.Equal(spanOwner, spanHandle.Owner);
    }

    [Fact]
    public void AcquireSpanHandle_ThrowsIfClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);
        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        // Acquire an owner first to prevent auto-disposal, ensuring we're only testing the closed condition
        var handle = spanOwner.AcquireSpanHandle();

        spanOwner.Close();

        Assert.True(spanOwner.IsClosed);
        Assert.False(spanOwner.IsDisposed);

        Assert.Throws<InvalidOperationException>(() => spanOwner.AcquireSpanHandle());
    }

    [Fact]
    public void AcquireSpanHandle_ThrowsIfDisposedTest()
    {
        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        dataFileLifetimeHandleMock.Setup(x => x.Dispose());

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        spanOwner.Dispose();

        Assert.Throws<ObjectDisposedException>(() => spanOwner.AcquireSpanHandle());
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        // Acquire a handle first to prevent auto-disposal
        var spanHandle = spanOwner.AcquireSpanHandle();

        Assert.False(spanOwner.IsClosed);

        // Close the owner
        spanOwner.Close();

        Assert.True(spanOwner.IsClosed);
        Assert.Equal(1, spanOwner.HandleCount);
    }

    [Fact]
    public void Close_DisposeIfNoOwnersTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        spanFactoryMock.Setup(x => x.Dispose());

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        dataFileLifetimeHandleMock.Setup(x => x.Dispose());

        var spanOwner = new ConnectionDataSpanOwner(spanFactoryMock.Object, dataFileLifetimeMock.Object);

        Assert.False(spanOwner.IsClosed);
        Assert.False(spanOwner.IsDisposed);
        Assert.Equal(0, spanOwner.HandleCount);

        spanOwner.Close();

        Assert.True(spanOwner.IsClosed);
        Assert.True(spanOwner.IsDisposed);
        Assert.Equal(0, spanOwner.HandleCount);

        dataFileLifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
        spanFactoryMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void Close_ThrowIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        spanFactoryMock.Setup(x => x.Dispose());

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        dataFileLifetimeHandleMock.Setup(x => x.Dispose());

        var spanOwner = new ConnectionDataSpanOwner(spanFactoryMock.Object, dataFileLifetimeMock.Object);
        spanOwner.Dispose();

        Assert.Throws<ObjectDisposedException>(spanOwner.Close);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        spanFactoryMock.Setup(x => x.Dispose());

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        dataFileLifetimeHandleMock.Setup(x => x.Dispose());

        var spanOwner = new ConnectionDataSpanOwner(spanFactoryMock.Object, dataFileLifetimeMock.Object);

        spanOwner.Dispose();

        Assert.True(spanOwner.IsClosed);
        Assert.True(spanOwner.IsDisposed);
        Assert.Equal(0, spanOwner.HandleCount);

        dataFileLifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
        spanFactoryMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void ReleaseSpanHandle_Test()
    {
        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        // Acquire a handle
        var handle1 = spanOwner.AcquireSpanHandle();
        Assert.Equal(1, spanOwner.HandleCount);

        // Acquire a second handle so we don't trigger auto-disposal when returning the first handle
        var handle2 = spanOwner.AcquireSpanHandle();
        Assert.Equal(2, spanOwner.HandleCount);

        // Return the first handle and verify we still have one handle remaining
        spanOwner.ReleaseSpanHandle(in handle1);

        Assert.Equal(1, spanOwner.HandleCount);
        Assert.False(spanOwner.IsClosed);

        // Return the first handle again and ensure it had no effect
        spanOwner.ReleaseSpanHandle(in handle1);

        Assert.Equal(1, spanOwner.HandleCount);
        Assert.False(spanOwner.IsClosed);
    }

    [Fact]
    public void ReleaseSpanHandle_DisposeOnClosedAndLastHandleReturnedTest()
    {
        var fakeSpanFactoryMemory = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var fakeSpanFactory = new FakeConnectionDataSpanFactory(fakeSpanFactoryMemory);

        var mocks = new MockRepository(MockBehavior.Strict);

        var dataFileLifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var dataFileLifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        dataFileLifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(dataFileLifetimeHandleMock.Object);

        dataFileLifetimeHandleMock.Setup(x => x.Dispose());

        var spanOwner = new ConnectionDataSpanOwner(fakeSpanFactory, dataFileLifetimeMock.Object);

        // Acquire a handle to prevent auto-disposal when we call Close()
        var handle1 = spanOwner.AcquireSpanHandle();

        spanOwner.Close();

        Assert.True(spanOwner.IsClosed);
        Assert.False(spanOwner.IsDisposed);
        Assert.Equal(1, spanOwner.HandleCount);
        dataFileLifetimeHandleMock.Verify(x => x.Dispose(), Times.Never);

        // Return our handle and verify the owner is disposed automatically
        spanOwner.ReleaseSpanHandle(in handle1);

        Assert.True(spanOwner.IsClosed);
        Assert.True(spanOwner.IsDisposed);
        Assert.Equal(0, spanOwner.HandleCount);
        dataFileLifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
    }
}
