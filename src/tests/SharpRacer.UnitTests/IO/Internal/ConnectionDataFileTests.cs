using DotNext.IO.MemoryMappedFiles;
using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataFileTests
{
    [Fact]
    public void Ctor_Test()
    {
        Memory<byte> mappedMemoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(() => mappedMemoryObj);

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        Assert.False(dataFile.IsDisposed);
        Assert.True(dataFile.IsOpen);
        Assert.Equal(mappedMemoryObj, dataFile.Memory);
        Assert.Empty(dataFile.Handles);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullExceptionTest()
    {
        Memory<byte> mappedMemoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(() => mappedMemoryObj);

        Assert.Throws<ArgumentNullException>(() => new ConnectionDataFile(null!, mappedMemoryMock.Object));

        Assert.Throws<ArgumentNullException>(() => new ConnectionDataFile(mmfMock.Object, null!));

        Assert.Throws<ArgumentNullException>(
            () => new ConnectionDataFile(null!, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object));

        Assert.Throws<ArgumentNullException>(
            () => new ConnectionDataFile(mmfMock.Object, null!, memoryPoolMock.Object, spanPoolMock.Object));

        Assert.Throws<ArgumentNullException>(
            () => new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, null!, spanPoolMock.Object));

        Assert.Throws<ArgumentNullException>(
            () => new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, null!));
    }

    [Fact]
    public void AcquireLifetimeHandle_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        var lifetimeHandle = dataFile.AcquireLifetimeHandle();

        Assert.NotNull(lifetimeHandle);
        Assert.False(lifetimeHandle.IsDisposed);

        Assert.Single(dataFile.Handles);
        Assert.Contains(lifetimeHandle, dataFile.Handles);
    }

    [Fact]
    public void AcquireLifetimeHandle_ThrowsIfClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        memoryPoolMock.Setup(x => x.Close());
        spanPoolMock.Setup(x => x.Close());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        dataFile.Close();

        Assert.False(dataFile.IsOpen);
        Assert.Throws<InvalidOperationException>(dataFile.AcquireLifetimeHandle);
    }

    [Fact]
    public void AcquireLifetimeHandle_ThrowsIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mmfMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());
        memoryPoolMock.Setup(x => x.Dispose());
        spanPoolMock.Setup(x => x.Dispose());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        Assert.False(dataFile.IsDisposed);
        Assert.True(dataFile.IsOpen);

        dataFile.Dispose();

        Assert.True(dataFile.IsDisposed);
        Assert.False(dataFile.IsOpen);

        Assert.Throws<ObjectDisposedException>(dataFile.AcquireLifetimeHandle);
    }

    [Fact]
    public void Close_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        memoryPoolMock.Setup(x => x.Close());
        spanPoolMock.Setup(x => x.Close());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        Assert.True(dataFile.IsOpen);

        dataFile.Close();

        Assert.False(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);

        memoryPoolMock.Verify(x => x.Close(), Times.Once);
        spanPoolMock.Verify(x => x.Close(), Times.Once);
    }

    [Fact]
    public void Close_DoesNotThrowIfClosedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        memoryPoolMock.Setup(x => x.Close());
        spanPoolMock.Setup(x => x.Close());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        Assert.True(dataFile.IsOpen);

        dataFile.Close();

        Assert.False(dataFile.IsOpen);

        // Close again to ensure no exception is thrown
        dataFile.Close();

        // Verify pool close methods were only called once
        memoryPoolMock.Verify(x => x.Close(), Times.Once);
        spanPoolMock.Verify(x => x.Close(), Times.Once);
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mmfMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());
        memoryPoolMock.Setup(x => x.Dispose());
        spanPoolMock.Setup(x => x.Dispose());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        Assert.False(dataFile.IsDisposed);
        Assert.True(dataFile.IsOpen);

        dataFile.Dispose();

        Assert.True(dataFile.IsDisposed);
        Assert.False(dataFile.IsOpen);

        mmfMock.Verify(x => x.Dispose(), Times.Once);
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        memoryPoolMock.Verify(x => x.Dispose(), Times.Once);
        spanPoolMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void ReleaseLifetimeHandle_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        // Acquire two handles so the data file doesn't auto-close
        var handle1 = dataFile.AcquireLifetimeHandle();
        var handle2 = dataFile.AcquireLifetimeHandle();

        Assert.NotNull(handle1);
        Assert.False(handle1.IsDisposed);

        Assert.NotNull(handle2);
        Assert.False(handle2.IsDisposed);

        Assert.Equal(2, dataFile.Handles.Count());
        Assert.Contains(handle1, dataFile.Handles);
        Assert.Contains(handle2, dataFile.Handles);

        // Return one handle
        handle1.Dispose();

        Assert.True(handle1.IsDisposed);
        Assert.False(handle2.IsDisposed);

        Assert.Single(dataFile.Handles);
        Assert.DoesNotContain(handle1, dataFile.Handles);
        Assert.Contains(handle2, dataFile.Handles);

        Assert.True(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);
    }

    [Fact]
    public void ReleaseLifetimeHandle_HandleNotOwnedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        var handleMock = mocks.Create<IConnectionDataFileLifetimeHandle>();

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        handleMock.Setup(x => x.Dispose());

        // Acquire one handle so the data file will call Dispose on itself if that specific handle is returned
        var handle1 = dataFile.AcquireLifetimeHandle();

        // Create a second, unaffiliated handle to return
        var handle2 = handleMock.Object;

        Assert.Single(dataFile.Handles);
        Assert.Contains(handle1, dataFile.Handles);
        Assert.DoesNotContain(handle2, dataFile.Handles);

        // Return unaffiliated handle
        dataFile.ReleaseLifetimeHandle(handle2);

        Assert.Single(dataFile.Handles);
        Assert.Contains(handle1, dataFile.Handles);
        Assert.DoesNotContain(handle2, dataFile.Handles);

        Assert.True(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);

        handleMock.Verify(x => x.Dispose(), Times.Never);
    }

    [Fact]
    public void ReleaseLifetimeHandle_DisposeIfClosedAndEmptyTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        memoryPoolMock.Setup(x => x.Close());
        spanPoolMock.Setup(x => x.Close());
        mmfMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());
        memoryPoolMock.Setup(x => x.Dispose());
        spanPoolMock.Setup(x => x.Dispose());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        var handle1 = dataFile.AcquireLifetimeHandle();

        Assert.NotNull(handle1);
        Assert.False(handle1.IsDisposed);

        // Close data file so it automatically calls Dispose on itself when the handle is returned
        dataFile.Close();

        // Return one handle
        handle1.Dispose();

        Assert.True(handle1.IsDisposed);
        Assert.Empty(dataFile.Handles);

        Assert.False(dataFile.IsOpen);
        Assert.True(dataFile.IsDisposed);

        memoryPoolMock.Verify(x => x.Close(), Times.Once);
        spanPoolMock.Verify(x => x.Close(), Times.Once);
        mmfMock.Verify(x => x.Dispose(), Times.Once);
        mappedMemoryMock.Verify(x => x.Dispose(), Times.Once);
        memoryPoolMock.Verify(x => x.Dispose(), Times.Once);
        spanPoolMock.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void ReleaseLifetimeHandle_DoesNotDisposeIfClosedAndNotEmptyTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        memoryPoolMock.Setup(x => x.Close());
        spanPoolMock.Setup(x => x.Close());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        // Acquire two handles so the data file doesn't auto-close
        var handle1 = dataFile.AcquireLifetimeHandle();
        var handle2 = dataFile.AcquireLifetimeHandle();

        // Close data file
        dataFile.Close();

        // Return one handle, leaving the other alive
        handle1.Dispose();

        Assert.True(handle1.IsDisposed);
        Assert.False(handle2.IsDisposed);

        Assert.Single(dataFile.Handles);
        Assert.DoesNotContain(handle1, dataFile.Handles);
        Assert.Contains(handle2, dataFile.Handles);

        Assert.False(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);
    }

    [Fact]
    public void RentMemory_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        var memoryOwnerMock = mocks.Create<IConnectionDataHandle>();

        memoryPoolMock.Setup(x => x.Rent()).Returns(memoryOwnerMock.Object);

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);

        var memoryOwner = dataFile.RentMemory();

        Assert.Equal(memoryOwnerMock.Object, memoryOwner);

        memoryPoolMock.Verify(x => x.Rent(), Times.Once);
    }

    [Fact]
    public void RentMemory_ThrowsIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mmfMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());
        memoryPoolMock.Setup(x => x.Dispose());
        spanPoolMock.Setup(x => x.Dispose());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);
        dataFile.Dispose();

        Assert.Throws<ObjectDisposedException>(() => dataFile.RentMemory());
    }

    [Fact]
    public void RentSpan_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();

        byte[] spanBytes = [0xDE, 0xAD, 0xBE, 0xEF];
        var spanOwnerToken = new DataFileSpanOwnerToken(123);

        var spanPool = new FakeSpanPool(() => spanOwnerToken, spanBytes);

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPool);

        var rentedSpan = dataFile.RentSpan();

        Assert.True(rentedSpan.IsOwned);
        Assert.Equal(rentedSpan.Pool, spanPool);
        Assert.Equal(spanOwnerToken, rentedSpan.Token);
        Assert.Equal(rentedSpan.Span, spanBytes);
    }

    [Fact]
    public void RentSpan_ThrowsIfDisposedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var memoryPoolMock = mocks.Create<IConnectionDataMemoryOwner>();
        var spanPoolMock = mocks.Create<IDataFileSpanPool>();

        mmfMock.Setup(x => x.Dispose());
        mappedMemoryMock.Setup(x => x.Dispose());
        memoryPoolMock.Setup(x => x.Dispose());
        spanPoolMock.Setup(x => x.Dispose());

        var dataFile = new ConnectionDataFile(mmfMock.Object, mappedMemoryMock.Object, memoryPoolMock.Object, spanPoolMock.Object);
        dataFile.Dispose();

        Assert.Throws<ObjectDisposedException>(() => dataFile.RentSpan());
    }

    private class FakeSpanPool : IDataFileSpanPool
    {
        private readonly Func<DataFileSpanOwnerToken> _ownerTokenFactory;
        private readonly byte[] _spanBytes;

        public FakeSpanPool(Func<DataFileSpanOwnerToken> ownerTokenFactory, byte[] spanBytes)
        {
            _ownerTokenFactory = ownerTokenFactory ?? throw new ArgumentNullException(nameof(ownerTokenFactory));
            _spanBytes = spanBytes ?? throw new ArgumentNullException(nameof(spanBytes));
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public DataFileSpanOwner Rent()
        {
            return new DataFileSpanOwner(this, _ownerTokenFactory(), _spanBytes);
        }

        public void Return(ref readonly DataFileSpanOwner owner)
        {
            throw new NotImplementedException();
        }
    }
}
