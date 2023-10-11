using System.Runtime.InteropServices;
using System.Text;

namespace SharpRacer.Interop;
public class IRSDKDescStringTests
{
    [Fact]
    public void EmptyTest()
    {
        var charBuf = new byte[IRSDKDescString.Size];

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(charBuf);

        Assert.Equal(0, sdkString.GetLength());
        Assert.Equal(string.Empty, sdkString.ToString());
    }

    [Fact]
    public void MaxLengthTest()
    {
        var string64 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@";
        var stringBytes = Encoding.ASCII.GetBytes(string64);

        Assert.Equal(64, stringBytes.Length);

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(stringBytes);

        Assert.Equal(64, sdkString.GetLength());
        Assert.Equal(string64, sdkString.ToString());
    }

    [Fact]
    public void NullTerminatedTest()
    {
        var testString = "foo";

        var charBuf = new Span<byte>(new byte[IRSDKDescString.Size]);

        Encoding.ASCII.GetBytes(testString).CopyTo(charBuf);

        var sdkString = MemoryMarshal.Read<IRSDKDescString>(charBuf);

        Assert.Equal(testString.Length, sdkString.GetLength());
        Assert.Equal(testString, sdkString.ToString());
    }
}
