namespace SharpRacer.SourceGenerators.Configuration;
public class MSBuildPropertyValueTests
{
    [Fact]
    public void Ctor_ExistsTest()
    {
        var key = MSBuildPropertyKey.FromPropertyName("MyProperty");
        var propertyValue = new MSBuildPropertyValue(key, "Test");

        Assert.Equal("Test", propertyValue.Value);
        Assert.Equal(key, propertyValue.PropertyKey);
        Assert.True(propertyValue.Exists);
        Assert.False(propertyValue.GetBooleanOrDefault(false));
    }

    [Fact]
    public void Ctor_NotExistsTest()
    {
        var key = MSBuildPropertyKey.FromPropertyName("MyProperty");
        var propertyValue = new MSBuildPropertyValue(key, null);

        Assert.Null(propertyValue.Value);
        Assert.False(propertyValue.Exists);
    }

    [Fact]
    public void Equals_Test()
    {
        var propertyValue1 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");
        var propertyValue2 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");

        Assert.True(propertyValue1.Equals(propertyValue2));
        Assert.True(propertyValue1.Equals((object)propertyValue2));
        Assert.False(propertyValue1.Equals(DateTime.Now));

        Assert.True(propertyValue1 == propertyValue2);
        Assert.False(propertyValue1 != propertyValue2);

        Assert.Equal(propertyValue1.GetHashCode(), propertyValue2.GetHashCode());
    }

    [Fact]
    public void Equals_DefaultTest()
    {
        var propertyValue1 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");
        var propertyValue2 = default(MSBuildPropertyValue);

        Assert.False(propertyValue1.Equals(propertyValue2));
        Assert.False(propertyValue2.Equals(propertyValue1));

        Assert.False(propertyValue1 == propertyValue2);
        Assert.True(propertyValue1 != propertyValue2);

        Assert.NotEqual(propertyValue1.GetHashCode(), propertyValue2.GetHashCode());
    }

    [Fact]
    public void GetBooleanOrDefault_Test()
    {
        var stringPropertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");
        Assert.False(stringPropertyValue.GetBooleanOrDefault(false));

        var notExistsPropertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);
        Assert.False(notExistsPropertyValue.GetBooleanOrDefault(false));
        Assert.True(notExistsPropertyValue.GetBooleanOrDefault(true));

        var truePropertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "true");
        Assert.True(truePropertyValue.GetBooleanOrDefault(false));

        var falsePropertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "false");
        Assert.False(falsePropertyValue.GetBooleanOrDefault(true));
    }
}
