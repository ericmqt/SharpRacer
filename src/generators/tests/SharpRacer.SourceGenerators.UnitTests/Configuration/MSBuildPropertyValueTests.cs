namespace SharpRacer.SourceGenerators.Configuration;
public class MSBuildPropertyValueTests
{
    public static TheoryData<MSBuildPropertyValue, MSBuildPropertyValue> InequalityData => GetInequalityData();

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

        Assert.Equal(key, propertyValue.PropertyKey);
        Assert.Null(propertyValue.Value);
        Assert.False(propertyValue.Exists);
    }

    [Fact]
    public void Ctor_ThrowOnDefaultPropertyKeyArgTest()
    {
        Assert.Throws<ArgumentException>(() => new MSBuildPropertyValue(default, null));
    }

    [Fact]
    public void Equals_Test()
    {
        var propertyValue1 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");
        var propertyValue2 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");

        EquatableStructAssert.Equal(propertyValue1, propertyValue2);
    }

    [Fact]
    public void Equals_PropertyDoesNotExistTest()
    {
        var propertyValue1 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);
        var propertyValue2 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);

        EquatableStructAssert.Equal(propertyValue1, propertyValue2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(MSBuildPropertyValue propertyValue1, MSBuildPropertyValue propertyValue2)
    {
        EquatableStructAssert.NotEqual(propertyValue1, propertyValue2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var propertyValue1 = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);

        EquatableStructAssert.ObjectEqualsMethod(false, propertyValue1, int.MaxValue);
    }

    [Fact]
    public void GetBooleanOrDefault_NonBooleanValueTest()
    {
        var stringPropertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");
        Assert.False(stringPropertyValue.GetBooleanOrDefault(false));
    }

    [Fact]
    public void GetBooleanOrDefault_PropertyDoesNotExistTest()
    {
        var propertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);

        Assert.Null(propertyValue.Value);
        Assert.False(propertyValue.Exists);

        Assert.False(propertyValue.GetBooleanOrDefault(false));
        Assert.True(propertyValue.GetBooleanOrDefault(true));
    }

    [Fact]
    public void GetBooleanOrDefault_PropertyHasFalseValueTest()
    {
        var propertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "false");

        Assert.NotNull(propertyValue.Value);
        Assert.True(propertyValue.Exists);
        Assert.False(propertyValue.GetBooleanOrDefault(true));
    }

    [Fact]
    public void GetBooleanOrDefault_PropertyHasTrueValueTest()
    {
        var propertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "true");

        Assert.NotNull(propertyValue.Value);
        Assert.True(propertyValue.Exists);
        Assert.True(propertyValue.GetBooleanOrDefault(false));
    }

    [Fact]
    public void GetValueOrDefault_DoesNotExistTest()
    {
        var propertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null);

        Assert.Null(propertyValue.Value);
        Assert.False(propertyValue.Exists);

        Assert.Equal("defaultValue", propertyValue.GetValueOrDefault("defaultValue"));
    }

    [Fact]
    public void GetValueOrDefault_ExistsTest()
    {
        var propertyValue = new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "Test");

        Assert.NotNull(propertyValue.Value);
        Assert.True(propertyValue.Exists);

        Assert.Equal("Test", propertyValue.GetValueOrDefault("defaultValue"));
    }

    private static TheoryData<MSBuildPropertyValue, MSBuildPropertyValue> GetInequalityData()
        => new TheoryData<MSBuildPropertyValue, MSBuildPropertyValue>()
        {
            // Value and default value
            {
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "test"),
                default
            },

            // Different property keys
            {
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty1"), null),
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty2"), null)
            },

            // Same property keys, one value exists, one does not
            {
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "test"),
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), null)
            },

            // Same property keys, values differ
            {
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "test1"),
                new MSBuildPropertyValue(MSBuildPropertyKey.FromPropertyName("MyProperty"), "test2")
            }
        };
}
