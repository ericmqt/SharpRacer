using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class DescriptorPropertyReferenceTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableName = "Test";
        var propertyName = "TestProperty";
        var descriptorClassName = "MyDescriptors";
        var descriptorClassNamespace = "TestApp.Variables";

        var propertyRef = new DescriptorPropertyReference(variableName, propertyName, descriptorClassName, descriptorClassNamespace);

        Assert.Equal(variableName, propertyRef.VariableName);
        Assert.Equal(propertyName, propertyRef.PropertyName);
        Assert.Equal(descriptorClassName, propertyRef.DescriptorClassName);
        Assert.Equal(descriptorClassNamespace, propertyRef.DescriptorClassNamespace);
    }

    [Fact]
    public void Ctor_ThrowsOnNullOrEmptyStringArgs()
    {
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference(null!, "TestProperty", "MyDescriptors", "TestApp.Variables"));
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference("Test", null!, "MyDescriptors", "TestApp.Variables"));
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference("Test", "TestProperty", null!, "TestApp.Variables"));
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors", null!));
    }

    [Fact]
    public void Ctor_FromModelsTest()
    {
        var variableName = "Test";
        var variableInfo = new VariableInfo(variableName, VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var descriptorPropertyModel = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        var descriptorClassName = "MyDescriptors";
        var descriptorClassNamespace = "TestApp.Variables";

        var descriptorClassModel = new DescriptorClassModel(descriptorClassName, descriptorClassNamespace, Location.None)
            .WithDescriptorProperties([descriptorPropertyModel]);

        var propertyRef = new DescriptorPropertyReference(descriptorClassModel, descriptorPropertyModel);

        Assert.Equal(variableName, propertyRef.VariableName);
        Assert.Equal(propertyName, propertyRef.PropertyName);
        Assert.Equal(descriptorClassName, propertyRef.DescriptorClassName);
        Assert.Equal(descriptorClassNamespace, propertyRef.DescriptorClassNamespace);
    }

    [Fact]
    public void Ctor_FromModels_ThrowsOnDefaultArgValuesTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorPropertyModel = new DescriptorPropertyModel("TestProperty", "Test XML summary", variableModel);

        var descriptorClassModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
            .WithDescriptorProperties([descriptorPropertyModel]);

        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference(default, descriptorPropertyModel));
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyReference(descriptorClassModel, default));
    }

    [Fact]
    public void Equals_Test()
    {
        var variableName = "Test";
        var propertyName = "TestProperty";
        var descriptorClassName = "MyDescriptors";
        var descriptorClassNamespace = "TestApp.Variables";

        var propertyRef1 = new DescriptorPropertyReference(variableName, propertyName, descriptorClassName, descriptorClassNamespace);
        var propertyRef2 = new DescriptorPropertyReference(variableName, propertyName, descriptorClassName, descriptorClassNamespace);

        EquatableStructAssert.Equal(propertyRef1, propertyRef2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var propertyRef1 = new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors", "TestApp.Variables");
        EquatableStructAssert.NotEqual(propertyRef1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(DescriptorPropertyReference propertyRef1, DescriptorPropertyReference propertyRef2)
    {
        EquatableStructAssert.NotEqual(propertyRef1, propertyRef2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var propertyRef1 = new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors", "TestApp.Variables");

        EquatableStructAssert.ObjectEqualsMethod(false, propertyRef1, int.MaxValue);
    }

    [Fact]
    public void StaticPropertyMemberAccess_Test()
    {
        var propertyName = "TestProperty";
        var descriptorClassName = "MyDescriptors";
        var descriptorClassNamespace = "TestApp.Variables";

        var propertyRef = new DescriptorPropertyReference("Test", propertyName, descriptorClassName, descriptorClassNamespace);

        Assert.Equal(
            $"global::{descriptorClassNamespace}.{descriptorClassName}.{propertyName}",
            propertyRef.StaticPropertyMemberAccess().ToFullString());
    }

    public static TheoryData<DescriptorPropertyReference, DescriptorPropertyReference> GetInequalityData()
    {
        return new TheoryData<DescriptorPropertyReference, DescriptorPropertyReference>()
        {
            // VariableName
            {
                new DescriptorPropertyReference("Test1", "TestProperty", "MyDescriptors", "TestApp.Variables"),
                new DescriptorPropertyReference("Test2", "TestProperty", "MyDescriptors", "TestApp.Variables")
            },

            // PropertyName
            {
                new DescriptorPropertyReference("Test", "TestProperty1", "MyDescriptors", "TestApp.Variables"),
                new DescriptorPropertyReference("Test", "TestProperty2", "MyDescriptors", "TestApp.Variables")
            },

            // DescriptorClassName
            {
                new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors1", "TestApp.Variables"),
                new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors2", "TestApp.Variables")
            },

            // DescriptorClassNamespace
            {
                new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors", "TestApp.Variables1"),
                new DescriptorPropertyReference("Test", "TestProperty", "MyDescriptors", "TestApp.Variables2")
            },
        };
    }
}
