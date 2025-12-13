using Moq;

namespace SharpRacer.IO.Internal;
public class DataFileMemoryOwnerTests
{
    [Fact]
    public void Ctor_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);
        var poolMock = mocks.Create<IDataFileMemoryPool>();

        var owner = new DataFileMemoryOwner(memoryObj, poolMock.Object);

        Assert.True(owner.Memory.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Dispose_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var mocks = new MockRepository(MockBehavior.Strict);
        var poolMock = mocks.Create<IDataFileMemoryPool>();

        var owner = new DataFileMemoryOwner(memoryObj, poolMock.Object);

        poolMock.Setup(x => x.Return(owner));

        // Dispose and return to pool
        owner.Dispose();

        poolMock.Verify(x => x.Return(owner), Times.Once);
        Assert.Throws<ObjectDisposedException>(() => owner.Memory);
    }
}
