using System.Runtime.InteropServices;
using System.Text;

namespace SharpRacer.Interop;
public class IRSDKStringTests
{
    [Fact]
    public void FromString_Test()
    {
        var valueStr = "foo";
        var value = IRSDKString.FromString(valueStr);

        Assert.Equal(valueStr.Length, value.GetLength());
        Assert.Equal(valueStr, value.ToString());
    }

    [Fact]
    public void FromString_NullOrEmptyValueTest()
    {
        var nullValue = IRSDKString.FromString(null);
        var emptyValue = IRSDKString.FromString(string.Empty);

        Assert.Equal(0, nullValue.GetLength());
        Assert.Equal(0, emptyValue.GetLength());

        Assert.Equal(string.Empty, nullValue.ToString());
        Assert.Equal(string.Empty, emptyValue.ToString());
    }

    [Fact]
    public void FromString_OversizeValueTest()
    {
        var chars = string.Join(null, Enumerable.Repeat('a', IRSDKString.Size + 1));

        Assert.Throws<ArgumentException>(() => IRSDKString.FromString(chars));
    }

    [Fact]
    public void MemoryMarshalRead_EmptyTest()
    {
        var charBuf = new byte[IRSDKString.Size];
        Array.Fill(charBuf, byte.MinValue);

        var sdkString = MemoryMarshal.Read<IRSDKString>(charBuf);

        Assert.Equal(0, sdkString.GetLength());
        Assert.Equal(string.Empty, sdkString.ToString());
    }

    [Fact]
    public void MemoryMarshalRead_MaxLengthTest()
    {
        var string32 = "abcdefghijklmnopqrstuvwxyzABCDEF";
        var stringBytes = Encoding.ASCII.GetBytes(string32);

        Assert.Equal(32, stringBytes.Length);

        var sdkString = MemoryMarshal.Read<IRSDKString>(stringBytes);

        Assert.Equal(32, sdkString.GetLength());
        Assert.Equal(string32, sdkString.ToString());
    }

    [Fact]
    public void MemoryMarshalRead_NullTerminatedTest()
    {
        var testString = "foo";

        var charBuf = new Span<byte>(new byte[IRSDKString.Size]);

        Encoding.ASCII.GetBytes(testString).CopyTo(charBuf);

        var sdkString = MemoryMarshal.Read<IRSDKString>(charBuf);

        Assert.Equal(testString.Length, sdkString.GetLength());
        Assert.Equal(testString, sdkString.ToString());
    }
}
