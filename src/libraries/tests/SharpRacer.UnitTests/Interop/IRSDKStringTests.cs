using System.Runtime.InteropServices;
using System.Text;

namespace SharpRacer.Interop;
public class IRSDKStringTests
{
    [Fact]
    public void EmptyTest()
    {
        var charBuf = new byte[IRSDKString.Size];
        Array.Fill(charBuf, byte.MinValue);

        var sdkString = MemoryMarshal.Read<IRSDKString>(charBuf);

        Assert.Equal(0, sdkString.GetLength());
        Assert.Equal(string.Empty, sdkString.ToString());
    }

    [Fact]
    public void MaxLengthTest()
    {
        var string32 = "abcdefghijklmnopqrstuvwxyzABCDEF";
        var stringBytes = Encoding.ASCII.GetBytes(string32);

        Assert.Equal(32, stringBytes.Length);

        var sdkString = MemoryMarshal.Read<IRSDKString>(stringBytes);

        Assert.Equal(32, sdkString.GetLength());
        Assert.Equal(string32, sdkString.ToString());
    }

    [Fact]
    public void NullTerminatedTest()
    {
        var testString = "foo";

        var charBuf = new Span<byte>(new byte[IRSDKString.Size]);

        Encoding.ASCII.GetBytes(testString).CopyTo(charBuf);

        var sdkString = MemoryMarshal.Read<IRSDKString>(charBuf);

        Assert.Equal(testString.Length, sdkString.GetLength());
        Assert.Equal(testString, sdkString.ToString());
    }
}
