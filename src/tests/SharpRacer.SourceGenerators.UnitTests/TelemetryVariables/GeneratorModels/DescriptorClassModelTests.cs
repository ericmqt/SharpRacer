using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class DescriptorClassModelTests
{
    [Fact]
    public void Ctor_Test()
    {
        var typeName = "MyDescriptors";
        var typeNamespace = "TestApp.Variables";

        var model = new DescriptorClassModel(typeName, typeNamespace, Location.None);

        Assert.Equal(typeName, model.TypeName);
        Assert.Equal(typeNamespace, model.TypeNamespace);
        Assert.Equal(Location.None, model.GeneratorAttributeLocation);
        Assert.False(model.DescriptorProperties.IsDefault);
    }

    [Fact]
    public void Ctor_ThrowsOnNullOrEmptyStringArgsTest()
    {
        Assert.Throws<ArgumentException>(() => new DescriptorClassModel(null!, "test", Location.None));
        Assert.Throws<ArgumentException>(() => new DescriptorClassModel("Test", null!, Location.None));
    }

    [Fact]
    public void Equals_Test()
    {
        var typeName = "MyDescriptors";
        var typeNamespace = "TestApp.Variables";

        var model1 = new DescriptorClassModel(typeName, typeNamespace, Location.None);
        var model2 = new DescriptorClassModel(typeName, typeNamespace, Location.None);

        EquatableStructAssert.Equal(model1, model2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var model1 = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None);

        EquatableStructAssert.NotEqual(model1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(DescriptorClassModel model1, DescriptorClassModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var model1 = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None);

        EquatableStructAssert.ObjectEqualsMethod(false, model1, int.MinValue);
    }

    [Fact]
    public void WithDescriptorProperties_Test()
    {
        var originalModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None);

        Assert.Empty(originalModel.DescriptorProperties);

        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var descriptorPropertyModel = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        var descriptorProperties = ImmutableArray.Create(descriptorPropertyModel);

        var mutatedModel = originalModel.WithDescriptorProperties(descriptorProperties);

        Assert.Single(mutatedModel.DescriptorProperties);
        Assert.Single(mutatedModel.DescriptorProperties, x => x.PropertyName == propertyName);

        EquatableStructAssert.NotEqual(originalModel, mutatedModel);
    }

    [Fact]
    public void WithDescriptorProperties_DefaultArrayParameterTest()
    {
        var originalModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None);

        Assert.Empty(originalModel.DescriptorProperties);

        var mutatedModel = originalModel.WithDescriptorProperties(default);

        Assert.False(mutatedModel.DescriptorProperties.IsDefault);
        Assert.Empty(mutatedModel.DescriptorProperties);

        EquatableStructAssert.Equal(originalModel, mutatedModel);
    }

    public static TheoryData<DescriptorClassModel, DescriptorClassModel> GetInequalityData()
    {
        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var variableInfo2 = new VariableInfo("TestEx", VariableValueType.Float, 1, "New and improved test variable", "test/s", false, false, null);
        var variableModel2 = new VariableModel(variableInfo2, default);

        var descriptorProperties1 = ImmutableArray.Create(new DescriptorPropertyModel("TestProperty", "This is the test property", variableModel1));
        var descriptorProperties2 = ImmutableArray.Create(new DescriptorPropertyModel("TestExProperty", "This is the new test property", variableModel2));

        var fakeLocation = Location.Create(
            "test.json",
            new TextSpan(10, 20),
            new LinePositionSpan(new LinePosition(1, 2), new LinePosition(4, 5)));

        return new TheoryData<DescriptorClassModel, DescriptorClassModel>()
        {
            // TypeName
            {
                new DescriptorClassModel("MyDescriptors1", "TestApp.Variables", Location.None),
                new DescriptorClassModel("MyDescriptors2", "TestApp.Variables", Location.None)
            },

            // TypeNamespace
            {
                new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None),
                new DescriptorClassModel("MyDescriptors", "TestApp.Descriptors", Location.None)
            },

            // GeneratorAttributeLocation
            {
                new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None),
                new DescriptorClassModel("MyDescriptors", "TestApp.Variables", fakeLocation)
            },

            // DescriptorProperties
            {
                new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
                    .WithDescriptorProperties(descriptorProperties1),
                new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
                    .WithDescriptorProperties(descriptorProperties2)
            }
        };
    }
}
