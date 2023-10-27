namespace SharpRacer.Interop;
public class DataBufferHeaderArrayTests
{
    [Fact]
    public void FromArray_Test()
    {
        var dataBufferHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888) };
        var dataBufferHeadersArray = DataBufferHeaderArray.FromArray(dataBufferHeaders);

        Assert.Equal(dataBufferHeaders[0], dataBufferHeadersArray[0]);
        Assert.Equal(dataBufferHeaders[1], dataBufferHeadersArray[1]);
        Assert.Equal(dataBufferHeaders[2], dataBufferHeadersArray[2]);
        Assert.Equal(dataBufferHeaders[3], dataBufferHeadersArray[3]);
    }

    [Fact]
    public void FromArray_LengthMismatchTest()
    {
        var tooLongHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376), new(1, 5632), new(17, 5888), new(94, 6247), };
        var tooShortHeaders = new DataBufferHeader[] { new(25, 5120), new(52, 5376) };

        Assert.Throws<ArgumentException>(() => DataBufferHeaderArray.FromArray(tooLongHeaders));
        Assert.Throws<ArgumentException>(() => DataBufferHeaderArray.FromArray(tooShortHeaders));
    }
}
