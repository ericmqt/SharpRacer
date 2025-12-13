using DotNext.IO.MemoryMappedFiles;
using Moq;

namespace SharpRacer.IO.Internal;
public class DataFileMemorySpanPoolTests
{
    [Fact]
    public void Ctor_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var mocks = new SpanPoolMocks(memoryObj);

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        Assert.False(spanPool.IsClosed);
        Assert.Equal(0, spanPool.OwnerCount);

        mocks.LifetimeMock.Verify(x => x.AcquireLifetimeHandle(), Times.Once());
        mocks.MemoryMappedDataFileMock.Verify(x => x.CreateMemoryAccessor(), Times.Once());
        mocks.MappedMemoryMock.VerifyGet(x => x.Memory, Times.Once());

        mocks.VerifyDispose(Times.Never);
    }

    [Fact]
    public void Close_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var mocks = new SpanPoolMocks(memoryObj);

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        // Acquire an owner first to prevent auto-disposal
        var owner = spanPool.Rent();

        Assert.False(spanPool.IsClosed);

        // Close the pool
        spanPool.Close();

        Assert.True(spanPool.IsClosed);
        Assert.Equal(1, spanPool.OwnerCount);

        mocks.VerifyDispose(Times.Never);
    }

    [Fact]
    public void Close_DisposeIfNoOwnersTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        Assert.Equal(0, spanPool.OwnerCount);
        Assert.False(spanPool.IsClosed);

        // Close the pool
        spanPool.Close();

        Assert.True(spanPool.IsClosed);
        Assert.Equal(0, spanPool.OwnerCount);

        mocks.VerifyDispose(Times.Once);
    }

    [Fact]
    public void Close_ThrowIfDisposedTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        spanPool.Dispose();

        Assert.Throws<ObjectDisposedException>(spanPool.Close);
        mocks.VerifyDispose(Times.Once);
    }

    [Fact]
    public void Dispose_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        // Dispose
        spanPool.Dispose();
        mocks.VerifyDispose(Times.Once);
    }

    [Fact]
    public void Rent_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var mocks = new SpanPoolMocks(memoryObj);

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        Assert.False(spanPool.IsClosed);

        // Acquire an owner first to prevent auto-disposal
        var owner = spanPool.Rent();

        Assert.Equal(1, spanPool.OwnerCount);

        Assert.True(owner.IsOwned);
        Assert.Equal(owner.Pool, spanPool);
        Assert.NotEqual(default, owner.Token);
        Assert.True(owner.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Rent_ThrowsIfClosedTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var mocks = new SpanPoolMocks(memoryObj);

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        Assert.False(spanPool.IsClosed);

        // Acquire an owner first to prevent auto-disposal, ensuring we're only testing the closed condition
        var owner = spanPool.Rent();

        // Close the pool
        spanPool.Close();
        Assert.True(spanPool.IsClosed);

        Assert.Throws<InvalidOperationException>(() => spanPool.Rent());
    }

    [Fact]
    public void Rent_ThrowsIfDisposedTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        Assert.False(spanPool.IsClosed);

        // Acquire an owner first to prevent auto-disposal, ensuring we're only testing the closed condition
        var owner = spanPool.Rent();

        // Dispose the pool
        spanPool.Dispose();

        Assert.Throws<ObjectDisposedException>(() => spanPool.Rent());
    }

    [Fact]
    public void Return_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = spanPool.Rent();

        Assert.Equal(1, spanPool.OwnerCount);

        // Acquire a second memory owner so we don't trigger auto-disposal
        var owner2 = spanPool.Rent();

        Assert.Equal(2, spanPool.OwnerCount);

        // Return the first owner
        spanPool.Return(in owner1);

        Assert.Equal(1, spanPool.OwnerCount);
        Assert.False(spanPool.IsClosed);

        // Return the first owner again and ensure nothing changed
        spanPool.Return(in owner1);

        Assert.Equal(1, spanPool.OwnerCount);
        Assert.False(spanPool.IsClosed);

        // Verify Dispose() not automatically triggered
        mocks.VerifyDispose(Times.Never);
    }

    [Fact]
    public void Return_DisposeOnClosedAndLastOwnerReturnedTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new SpanPoolMocks(memoryObj)
            .ConfigureDispose();

        var spanPool = new DataFileMemorySpanPool(mocks.MemoryMappedDataFileMock.Object, mocks.LifetimeMock.Object);

        // Acquire a memory owner
        var owner1 = spanPool.Rent();

        // Close the pool
        spanPool.Close();

        Assert.Equal(1, spanPool.OwnerCount);

        // Verify Dispose() not called just yet
        mocks.VerifyDispose(Times.Never);

        // Return our owner
        spanPool.Return(in owner1);

        // Verify closed and disposed
        Assert.Equal(0, spanPool.OwnerCount);
        Assert.True(spanPool.IsClosed);
        mocks.VerifyDispose(Times.Once);
    }

    private class SpanPoolMocks
    {
        public SpanPoolMocks(Memory<byte> memoryObj)
        {
            Memory = memoryObj;

            MockRepository = new MockRepository(MockBehavior.Strict);

            LifetimeMock = MockRepository.Create<IConnectionDataFileLifetime>();
            LifetimeHandleMock = MockRepository.Create<IConnectionDataFileLifetimeHandle>();
            MappedMemoryMock = MockRepository.Create<IMappedMemory>();
            MemoryMappedDataFileMock = MockRepository.Create<IMemoryMappedDataFile>();

            MappedMemoryMock.SetupGet(x => x.Memory)
                .Returns(Memory);

            LifetimeMock.Setup(x => x.AcquireLifetimeHandle())
                .Returns(LifetimeHandleMock.Object);

            MemoryMappedDataFileMock.Setup(x => x.CreateMemoryAccessor()).Returns(MappedMemoryMock.Object);
        }

        public Mock<IConnectionDataFileLifetime> LifetimeMock { get; }
        public Mock<IConnectionDataFileLifetimeHandle> LifetimeHandleMock { get; }
        public Mock<IMappedMemory> MappedMemoryMock { get; }
        public Memory<byte> Memory { get; }
        public Mock<IMemoryMappedDataFile> MemoryMappedDataFileMock { get; }
        public MockRepository MockRepository { get; }

        public SpanPoolMocks ConfigureDispose()
        {
            MappedMemoryMock.Setup(x => x.Dispose());
            LifetimeHandleMock.Setup(x => x.Dispose());

            return this;
        }

        public void VerifyDispose(Func<Times> times)
        {
            MappedMemoryMock.Verify(x => x.Dispose(), times);
            LifetimeHandleMock.Verify(x => x.Dispose(), times);
        }
    }
}
