namespace SharpRacer.SourceGenerators.Configuration;
public class MSBuildPropertyKeyTests
{
    [Fact]
    public void Equals_Test()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty");
        var key2 = MSBuildPropertyKey.FromPropertyName("MyProperty");

        Assert.True(key1.Equals(key2));
        Assert.True(key2.Equals(key1));

        Assert.True(key1.Equals((object)key2));
        Assert.False(key1.Equals(DateTime.Now));

        Assert.True(key1 == key2);
        Assert.False(key1 != key2);

        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultTest()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty");
        var key2 = default(MSBuildPropertyKey);

        Assert.False(key1.Equals(key2));
        Assert.False(key2.Equals(key1));

        Assert.False(key1 == key2);
        Assert.True(key1 != key2);

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void FromPropertyName_Test()
    {
        var key = MSBuildPropertyKey.FromPropertyName("MyProperty");

        Assert.Equal("MyProperty", key.PropertyName);
        Assert.Equal("build_property.MyProperty", key.Key);
    }

    [Fact]
    public void FromPropertyName_ThrowOnNullOrEmptyPropertyNameTest()
    {
        Assert.Throws<ArgumentException>(() => MSBuildPropertyKey.FromPropertyName(null!));
        Assert.Throws<ArgumentException>(() => MSBuildPropertyKey.FromPropertyName(string.Empty));
    }
}
