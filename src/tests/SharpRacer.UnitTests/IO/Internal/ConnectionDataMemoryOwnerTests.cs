using DotNext.IO.MemoryMappedFiles;
using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataMemoryOwnerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();

        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        lifetimeMock.Verify(x => x.AcquireLifetimeHandle(), Times.Once());
        mappedMemoryMock.VerifyGet(x => x.Memory, Times.Once());
    }

    [Fact]
    public void Ctor_ThrowsOnNullArgumentsTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();

        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        Assert.Throws<ArgumentNullException>(() => new ConnectionDataMemoryOwner(null!, lifetimeMock.Object));
        Assert.Throws<ArgumentNullException>(() => new ConnectionDataMemoryOwner(mappedMemoryMock.Object, null!));
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        Assert.False(memoryPool.IsClosed);

        // Acquire a memory owner but don't return so we don't trigger auto-disposal
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Close();

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(1, memoryPool.HandleCount);

        // Verify dispose was not called
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Never);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Never);
    }

    [Fact]
    public void Close_DisposeIfNoOwnersTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        Assert.False(memoryPool.IsClosed);

        // Close the pool
        memoryPool.Close();

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(0, memoryPool.HandleCount);

        // Verify dispose calls
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();

        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);

        lifetimeHandleMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Dispose
        memoryPool.Dispose();

        // Verify dispose calls
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void Rent_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        Assert.NotNull(owner1);
        Assert.Equal(1, memoryPool.HandleCount);
    }

    [Fact]
    public void Rent_ThrowsIfClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Acquire a memory owner before closing so the pool isn't auto-disposed
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Close();
        Assert.Throws<InvalidOperationException>(memoryPool.Rent);
    }

    [Fact]
    public void Rent_ThrowsIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Acquire a memory owner before closing so the pool isn't auto-disposed
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Dispose();
        Assert.Throws<ObjectDisposedException>(memoryPool.Rent);
    }

    [Fact]
    public void Return_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        Assert.NotNull(owner1);
        Assert.Equal(1, memoryPool.HandleCount);

        // Acquire a second memory owner so we don't trigger auto-disposal
        var owner2 = memoryPool.Rent();

        Assert.NotNull(owner2);
        Assert.Equal(2, memoryPool.HandleCount);

        // Return our first owner
        memoryPool.Return(owner1);
        Assert.Equal(1, memoryPool.HandleCount);
        Assert.False(memoryPool.IsClosed);

        // Verify Dispose() not called
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Never);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Never);
    }

    [Fact]
    public void Return_DisposeOnClosedAndLastOwnerReturnedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();

        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);
        mappedMemoryMock.Setup(x => x.Dispose());

        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);
        lifetimeHandleMock.Setup(x => x.Dispose());

        var memoryPool = new ConnectionDataMemoryOwner(mappedMemoryMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Close();

        Assert.Equal(1, memoryPool.HandleCount);

        // Verify Dispose() not called just yet
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Never);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Never);

        // Return our rented memory
        memoryPool.Return(owner1);

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(0, memoryPool.HandleCount);

        // Verify dispose calls
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
    }
}
