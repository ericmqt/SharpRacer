using SharpRacer.Extensions.Xunit;

namespace SharpRacer.IO.Internal;
public class ConnectionDataSpanHandleTokenTests
{
    [Fact]
    public void Ctor_Test()
    {
        ulong tokenId = ulong.MaxValue - 123;

        var token = new ConnectionDataSpanHandleToken(tokenId);
        Assert.Equal(tokenId, token.Id);
        Assert.NotEqual(ConnectionDataSpanHandleToken.Zero, token);
    }

    [Fact]
    public void Zero_Test()
    {
        var zeroToken = ConnectionDataSpanHandleToken.Zero;
        var token = new ConnectionDataSpanHandleToken(id: 0);
        var ownedToken = new ConnectionDataSpanHandleToken(123);

        Assert.Equal(ConnectionDataSpanHandleToken.Zero, zeroToken);
        Assert.Equal(ConnectionDataSpanHandleToken.Zero, token);
        Assert.Equal(default, zeroToken);

        Assert.NotEqual(zeroToken, ownedToken);
        Assert.NotEqual(ConnectionDataSpanHandleToken.Zero, ownedToken);
        Assert.NotEqual(default, ownedToken);
    }

    [Fact]
    public void Equals_Test()
    {
        var token1 = new ConnectionDataSpanHandleToken(123);
        var token2 = new ConnectionDataSpanHandleToken(123);

        EquatableStructAssert.Equal(token1, token2);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        ConnectionDataSpanHandleToken token1 = default;
        ConnectionDataSpanHandleToken token2 = default;

        EquatableStructAssert.Equal(token1, token2);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var token1 = new ConnectionDataSpanHandleToken(123);
        ConnectionDataSpanHandleToken token2 = default;

        EquatableStructAssert.NotEqual(token1, token2);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var token1 = new ConnectionDataSpanHandleToken(123);
        var token2 = new ConnectionDataSpanHandleToken(456);

        EquatableStructAssert.NotEqual(token1, token2);
    }

    [Fact]
    public void Equals_NullObjTest()
    {
        var token1 = new ConnectionDataSpanHandleToken(123);
        object? token2 = null;

        EquatableStructAssert.ObjectEqualsMethod(false, token1, token2);
    }
}
