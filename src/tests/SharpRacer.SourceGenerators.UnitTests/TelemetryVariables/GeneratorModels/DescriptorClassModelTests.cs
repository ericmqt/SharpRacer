using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
        EquatableStructAssert.NotEqual(model1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, model1, typeName);
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
}
