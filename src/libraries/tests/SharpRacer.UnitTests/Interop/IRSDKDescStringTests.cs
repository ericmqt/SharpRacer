using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Extensions.Xunit;

namespace SharpRacer.Interop;
public class IRSDKDescStringTests
{
    public static TheoryData<IRSDKDescString, IRSDKDescString> InequalityData => GetInequalityData();

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

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        var constructedValue = new IRSDKDescString();

        EquatableStructAssert.Equal(constructedValue, default);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var constructedValue = IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyz");

        EquatableStructAssert.NotEqual(constructedValue, default);
    }

    [Fact]
    public void Equals_EqualityTest()
    {
        const string fullLengthStr = "abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789";
        const string partialStr = "abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJK";

        var str1 = IRSDKDescString.FromString(fullLengthStr);
        var str2 = IRSDKDescString.FromString(fullLengthStr);

        EquatableStructAssert.Equal(str1, str2);

        str1 = IRSDKDescString.FromString(partialStr);
        str2 = IRSDKDescString.FromString(partialStr);

        EquatableStructAssert.Equal(str1, str2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(IRSDKDescString value1, IRSDKDescString value2)
    {
        EquatableStructAssert.NotEqual(value1, value2);
    }

    [Fact]
    public void Equals_NullObjectTest()
    {
        var constructedValue = IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyz");

        Assert.False(constructedValue.Equals(obj: null));
    }

    private static TheoryData<IRSDKDescString, IRSDKDescString> GetInequalityData()
    {
        var emptyStrCharBuf = new byte[IRSDKDescString.Size];
        Array.Fill(emptyStrCharBuf, byte.MinValue);

        var emptyStr = MemoryMarshal.Read<IRSDKDescString>(emptyStrCharBuf);

        return new TheoryData<IRSDKDescString, IRSDKDescString>
        {
            {  IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyzABCDEF"), emptyStr },

            // Last char difference
            {
                IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"),
                IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyz-ABCDEFGHIJKLMNOPQRSTUVWXYZ_012345678X") },

            // Length difference
            {
                IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyz"),
                IRSDKDescString.FromString("abcdefghijklmnopqrstuvwxyzABCDEF")
            },
        };
    }
}
