using SharpRacer.Extensions.Xunit;

namespace SharpRacer.IO.Internal;
public class DataFileSpanOwnerTokenTests
{
    [Fact]
    public void Ctor_Test()
    {
        ulong tokenId = ulong.MaxValue - 123;

        var token = new DataFileSpanOwnerToken(tokenId);
        Assert.Equal(tokenId, token.Id);
        Assert.NotEqual(DataFileSpanOwnerToken.Empty, token);
    }

    [Fact]
    public void Empty_Test()
    {
        var emptyToken = DataFileSpanOwnerToken.Empty;
        var ownedToken = new DataFileSpanOwnerToken(123);

        Assert.Equal(DataFileSpanOwnerToken.Empty, emptyToken);
        Assert.Equal(default, emptyToken);

        Assert.NotEqual(ownedToken, emptyToken);
        Assert.NotEqual(ownedToken, DataFileSpanOwnerToken.Empty);
        Assert.NotEqual(ownedToken, default);
    }

    [Fact]
    public void Equals_Test()
    {
        var token1 = new DataFileSpanOwnerToken(123);
        var token2 = new DataFileSpanOwnerToken(123);

        EquatableStructAssert.Equal(token1, token2);
    }

    [Fact]
    public void Equals_DefaultValueEqualityTest()
    {
        DataFileSpanOwnerToken token1 = default;
        DataFileSpanOwnerToken token2 = default;

        EquatableStructAssert.Equal(token1, token2);
    }

    [Fact]
    public void Equals_DefaultValueInequalityTest()
    {
        var token1 = new DataFileSpanOwnerToken(123);
        DataFileSpanOwnerToken token2 = default;

        EquatableStructAssert.NotEqual(token1, token2);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var token1 = new DataFileSpanOwnerToken(123);
        var token2 = new DataFileSpanOwnerToken(456);

        EquatableStructAssert.NotEqual(token1, token2);
    }

    [Fact]
    public void Equals_NullObjTest()
    {
        var token1 = new DataFileSpanOwnerToken(123);
        object? token2 = null;

        EquatableStructAssert.ObjectEqualsMethod(false, token1, token2);
    }
}
