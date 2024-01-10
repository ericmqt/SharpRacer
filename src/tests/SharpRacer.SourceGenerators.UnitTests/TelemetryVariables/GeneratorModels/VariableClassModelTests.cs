using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableClassModelTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableName = "Test";
        int variableValueCount = 3;
        var variableValueType = VariableValueType.Int;
        string? variableValueUnit = "test/s";

        var variableInfo = new VariableInfo(
            variableName,
            variableValueType,
            variableValueCount,
            "Test variable",
            variableValueUnit,
            false,
            false,
            null);

        var variableModel = new VariableModel(variableInfo, default);

        var className = "TestVariable";
        var classNamespace = "TestApp.Variables";

        var classModel = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            null,
            false,
            true);

        Assert.Equal(className, classModel.ClassName);
        Assert.Equal(classNamespace, classModel.ClassNamespace);
        Assert.Equal(variableName, classModel.VariableName);
        Assert.Equal(variableValueType, classModel.VariableValueType);
        Assert.Equal(variableValueCount, classModel.VariableValueCount);

        Assert.Null(classModel.DescriptorPropertyReference);
        Assert.False(classModel.IsClassInternal);
        Assert.True(classModel.IsClassPartial);
        Assert.False(classModel.Diagnostics.IsDefault);
        Assert.Empty(classModel.Diagnostics);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        classModel = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef,
            false,
            true);

        Assert.NotNull(classModel.DescriptorPropertyReference);
        Assert.Equal(descriptorRef, classModel.DescriptorPropertyReference.Value);
    }

    [Fact]
    public void Equals_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var className = "TestVariable";
        var classNamespace = "TestApp.Variables";

        var classModel1 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef,
            false,
            true);

        var classModel2 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef,
            false,
            true);

        EquatableStructAssert.Equal(classModel1, classModel2);
        EquatableStructAssert.NotEqual(classModel1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, classModel1, classNamespace);
    }

    [Fact]
    public void Equals_IdenticalExceptDescriptorRefTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef1 = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");
        var descriptorRef2 = new DescriptorPropertyReference("Test1", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var className = "TestVariable";
        var classNamespace = "TestApp.Variables";

        var classModel1 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef1,
            false,
            true);

        var classModel2 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef2,
            false,
            true);

        EquatableStructAssert.NotEqual(classModel1, classModel2);
    }

    [Fact]
    public void Equals_IdenticalExceptNullDescriptorRefTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var className = "TestVariable";
        var classNamespace = "TestApp.Variables";

        var classModel1 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            descriptorRef,
            false,
            true);

        var classModel2 = new VariableClassModel(
            className,
            classNamespace,
            variableModel,
            null,
            false,
            true);

        EquatableStructAssert.NotEqual(classModel1, classModel2);
    }

    [Fact]
    public void WithDescriptors_SingleDescriptorTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var classModel1 = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        Assert.False(classModel1.Diagnostics.IsDefault);
        Assert.Empty(classModel1.Diagnostics);

        var diagnostic = GeneratorDiagnostics.VariableClassConfiguredClassNameInUse(
            "TestVariable", "Test", "TestVariable", "TestOriginal", Location.None);

        var classModel2 = classModel1.WithDiagnostics(diagnostic);

        Assert.False(classModel2.Diagnostics.IsDefault);
        Assert.NotEmpty(classModel2.Diagnostics);
        Assert.Single(classModel2.Diagnostics);
        Assert.Single(classModel2.Diagnostics, x => x.Id == DiagnosticIds.VariableClassNameInUse);

        EquatableStructAssert.NotEqual(classModel1, classModel2);
    }

    [Fact]
    public void WithDescriptors_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var classModel1 = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        Assert.False(classModel1.Diagnostics.IsDefault);
        Assert.Empty(classModel1.Diagnostics);

        var diagnostics = ImmutableArray.Create(
            GeneratorDiagnostics.VariableClassConfiguredClassNameInUse(
                "TestVariable", "Test", "TestVariable", "TestOriginal", Location.None));

        var classModel2 = classModel1.WithDiagnostics(diagnostics);

        Assert.False(classModel2.Diagnostics.IsDefault);
        Assert.NotEmpty(classModel2.Diagnostics);
        Assert.Single(classModel2.Diagnostics);
        Assert.Single(classModel2.Diagnostics, x => x.Id == DiagnosticIds.VariableClassNameInUse);

        EquatableStructAssert.NotEqual(classModel1, classModel2);
    }
}
