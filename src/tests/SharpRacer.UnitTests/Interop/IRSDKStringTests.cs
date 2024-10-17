using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;
public class IRSDKStringTests
{
    public static TheoryData<IRSDKString, IRSDKString> InequalityData => GetInequalityData();

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
        var stringBytes = Encoding.UTF8.GetBytes(string32);

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

        Encoding.UTF8.GetBytes(testString).CopyTo(charBuf);

        var sdkString = MemoryMarshal.Read<IRSDKString>(charBuf);

        Assert.Equal(testString.Length, sdkString.GetLength());
        Assert.Equal(testString, sdkString.ToString());
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedValue = new IRSDKString();

        EquatableStructAssert.Equal(constructedValue, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedValue = IRSDKString.FromString("abcdefghijklmnopqrstuvwxyz");

        EquatableStructAssert.NotEqual(constructedValue, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        const string fullLengthStr = "abcdefghijklmnopqrstuvwxyzABCDEF";
        const string partialStr = "abcdefghijklmnopqrstuvwxyz";

        var str1 = IRSDKString.FromString(fullLengthStr);
        var str2 = IRSDKString.FromString(fullLengthStr);

        EquatableStructAssert.Equal(str1, str2);

        str1 = IRSDKString.FromString(partialStr);
        str2 = IRSDKString.FromString(partialStr);

        EquatableStructAssert.Equal(str1, str2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(IRSDKString value1, IRSDKString value2)
    {
        EquatableStructAssert.NotEqual(value1, value2);
    }

    private static TheoryData<IRSDKString, IRSDKString> GetInequalityData()
    {
        var emptyStrCharBuf = new byte[IRSDKString.Size];
        Array.Fill(emptyStrCharBuf, byte.MinValue);

        var emptyStr = MemoryMarshal.Read<IRSDKString>(emptyStrCharBuf);

        return new TheoryData<IRSDKString, IRSDKString>
        {
            {  IRSDKString.FromString("abcdefghijklmnopqrstuvwxyzABCDEF"), emptyStr },

            // Last char difference
            { IRSDKString.FromString("abcdefghijklmnopqrstuvwxyzABCDEF"), IRSDKString.FromString("abcdefghijklmnopqrstuvwxyzABCDEX") },

            // Length difference
            { IRSDKString.FromString("abcdefghijklmnopqrstuvwxyz"), IRSDKString.FromString("abcdefghijklmnopqrstuvwxyzABCDEF") },
        };
    }
}
