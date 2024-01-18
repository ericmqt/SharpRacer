namespace SharpRacer.SourceGenerators.Configuration;
public class MSBuildPropertyKeyTests
{
    [Fact]
    public void Equals_Test()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty");
        var key2 = MSBuildPropertyKey.FromPropertyName("MyProperty");

        EquatableStructAssert.Equal(key1, key2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty");

        EquatableStructAssert.NotEqual(key1, default);
    }

    [Fact]
    public void Equals_InequalityTest()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty1");
        var key2 = MSBuildPropertyKey.FromPropertyName("MyProperty2");

        EquatableStructAssert.NotEqual(key1, key2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var key1 = MSBuildPropertyKey.FromPropertyName("MyProperty");

        EquatableStructAssert.ObjectEqualsMethod(false, key1, int.MaxValue);
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
