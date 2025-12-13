using DotNext.IO.MemoryMappedFiles;
using Moq;

namespace SharpRacer.IO.Internal;
public class DataFileMemoryPoolTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var lifetimeMock = mocks.Create<IConnectionDataFileLifetime>();
        var lifetimeHandleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();
        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();

        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        lifetimeMock.Verify(x => x.AcquireLifetimeHandle(), Times.Once());
        mmfMock.Verify(x => x.CreateMemoryAccessor(), Times.Once());
        mappedMemoryMock.VerifyGet(x => x.Memory, Times.Once());
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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        Assert.False(memoryPool.IsClosed);

        // Acquire a memory owner but don't return so we don't trigger auto-disposal
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Close();

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(1, memoryPool.OwnerCount);

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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        Assert.False(memoryPool.IsClosed);

        // Close the pool
        memoryPool.Close();

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(0, memoryPool.OwnerCount);

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
        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();

        lifetimeMock.Setup(x => x.AcquireLifetimeHandle()).Returns(lifetimeHandleMock.Object);

        lifetimeHandleMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);
        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        Assert.NotNull(owner1);
        Assert.Equal(1, memoryPool.OwnerCount);
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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        Assert.NotNull(owner1);
        Assert.Equal(1, memoryPool.OwnerCount);

        // Acquire a second memory owner so we don't trigger auto-disposal
        var owner2 = memoryPool.Rent();

        Assert.NotNull(owner2);
        Assert.Equal(2, memoryPool.OwnerCount);

        // Return our first owner
        memoryPool.Return(owner1);
        Assert.Equal(1, memoryPool.OwnerCount);
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

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);

        var memoryPool = new DataFileMemoryPool(mmfMock.Object, lifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = memoryPool.Rent();

        // Close the pool
        memoryPool.Close();

        Assert.Equal(1, memoryPool.OwnerCount);

        // Verify Dispose() not called just yet
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Never);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Never);

        // Return our rented memory
        memoryPool.Return(owner1);

        Assert.True(memoryPool.IsClosed);
        Assert.Equal(0, memoryPool.OwnerCount);

        // Verify dispose calls
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        lifetimeHandleMock.Verify(x => x.Dispose(), Times.Once);
    }
}
