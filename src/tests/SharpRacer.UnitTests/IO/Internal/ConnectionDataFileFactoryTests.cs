using DotNext.IO.MemoryMappedFiles;
using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataFileFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var mmfFactoryMock = mocks.Create<IMemoryMappedDataFileFactory>();

        var factory = new ConnectionDataFileFactory(mmfFactoryMock.Object);

        Assert.Equal(mmfFactoryMock.Object, factory.MemoryMappedDataFileFactory);
    }

    [Fact]
    public void Create_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var mappedMemoryMock = mocks.Create<IMappedMemory>();
        var spanFactoryMock = mocks.Create<IConnectionDataSpanFactory>();

        var mmfMock = mocks.Create<IMemoryMappedDataFile>();

        mmfMock.Setup(x => x.CreateMemoryAccessor()).Returns(mappedMemoryMock.Object);
        mmfMock.Setup(x => x.CreateSpanFactory()).Returns(spanFactoryMock.Object);

        mappedMemoryMock.SetupGet(x => x.Memory).Returns(memoryObj);

        var mmfFactoryMock = mocks.Create<IMemoryMappedDataFileFactory>();
        mmfFactoryMock.Setup(x => x.OpenNew()).Returns(mmfMock.Object);

        var factory = new ConnectionDataFileFactory(mmfFactoryMock.Object);
        var dataFile = factory.Create();

        Assert.NotNull(dataFile);
        Assert.True(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);
        Assert.True(dataFile.Memory.Span.SequenceEqual(memoryObj.Span));
        Assert.Equal(2, dataFile.Handles.Count());

        mmfFactoryMock.Verify(x => x.OpenNew(), Times.Once());
        mmfMock.Verify(x => x.CreateMemoryAccessor(), Times.Exactly(2)); // one for the memory owner and one for the data file itself
        mmfMock.Verify(x => x.CreateSpanFactory(), Times.Once);
    }

}
