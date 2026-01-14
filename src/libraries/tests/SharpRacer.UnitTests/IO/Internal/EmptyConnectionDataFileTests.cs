namespace SharpRacer.IO.Internal;
public class EmptyConnectionDataFileTests
{
    [Fact]
    public void Ctor_Test()
    {
        var dataFile = new EmptyConnectionDataFile();

        Assert.False(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);
        Assert.Equal(ReadOnlyMemory<byte>.Empty, dataFile.Memory);
        Assert.Empty(dataFile.Handles);
    }

    [Fact]
    public void Close_Test()
    {
        var dataFile = new EmptyConnectionDataFile();
        dataFile.Close();

        Assert.False(dataFile.IsOpen);
        Assert.False(dataFile.IsDisposed);
    }

    [Fact]
    public void Dispose_Test()
    {
        var dataFile = new EmptyConnectionDataFile();
        dataFile.Dispose();

        Assert.False(dataFile.IsOpen);
        Assert.True(dataFile.IsDisposed);
    }

    [Fact]
    public void RentMemory_Test()
    {
        var dataFile = new EmptyConnectionDataFile();

        Assert.Throws<InvalidOperationException>(dataFile.GetMemoryHandle);
    }

    [Fact]
    public void RentSpan_Test()
    {
        var dataFile = new EmptyConnectionDataFile();

        Assert.Throws<InvalidOperationException>(() => dataFile.GetSpanHandle());
    }
}
