using System.Runtime.InteropServices;
using System.Text;

namespace SharpRacer.Interop;
public class IRSDKDescStringTests
{
    [Fact]
    public void FromString_Test()
    {
        var valueStr = "foo";
        var value = IRSDKDescString.FromString(valueStr);

        Assert.Equal(valueStr.Length, value.GetLength());
        Assert.Equal(valueStr, value.ToString());
    }

    [Fact]
    public void FromString_NullOrEmptyValueTest()
    {
        var nullValue = IRSDKDescString.FromString(null);
        var emptyValue = IRSDKDescString.FromString(string.Empty);

        Assert.Equal(0, nullValue.GetLength());
        Assert.Equal(0, emptyValue.GetLength());

        Assert.Equal(string.Empty, nullValue.ToString());
        Assert.Equal(string.Empty, emptyValue.ToString());
    }

    [Fact]
    public void FromString_OversizeValueTest()
    {
        var chars = string.Join(null, Enumerable.Repeat('a', IRSDKDescString.Size + 1));

        Assert.Throws<ArgumentException>(() => IRSDKDescString.FromString(chars));
    }

    [Fact]
    public void MemoryMarshalRead_EmptyTest()
    {
        var charBuf = new byte[IRSDKDescString.Size];

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(charBuf);

        Assert.Equal(0, sdkString.GetLength());
        Assert.Equal(string.Empty, sdkString.ToString());
    }

    [Fact]
    public void MemoryMarshalRead_MaxLengthTest()
    {
        var string64 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@";
        var stringBytes = Encoding.ASCII.GetBytes(string64);

        Assert.Equal(64, stringBytes.Length);

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(stringBytes);

        Assert.Equal(64, sdkString.GetLength());
        Assert.Equal(string64, sdkString.ToString());
    }

    [Fact]
    public void MemoryMarshalRead_NullTerminatedTest()
    {
        var testString = "foo";

        var charBuf = new Span<byte>(new byte[IRSDKDescString.Size]);

        Encoding.ASCII.GetBytes(testString).CopyTo(charBuf);

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(charBuf);

        Assert.Equal(testString.Length, sdkString.GetLength());
        Assert.Equal(testString, sdkString.ToString());
    }
}
