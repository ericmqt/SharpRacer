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
    public void BaseClassType_ArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal("ArrayDataVariable<int>", classModel.BaseClassType().ToFullString());
    }

    [Fact]
    public void BaseClassType_ScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal("ScalarDataVariable<int>", classModel.BaseClassType().ToFullString());
    }

    [Fact]
    public void ClassAccessibility_InternalTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: true,
            isClassPartial: true);

        Assert.Equal(Accessibility.Internal, classModel.ClassAccessibility());
    }

    [Fact]
    public void ClassAccessibility_PublicTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal(Accessibility.Public, classModel.ClassAccessibility());
    }

    [Fact]
    public void ClassIdentifier_Test()
    {
        var className = "TestVariable";
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            className,
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal(className, classModel.ClassIdentifier().ValueText);
    }

    [Fact]
    public void ClassIdentifierName_Test()
    {
        var className = "TestVariable";
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            className,
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal(className, classModel.ClassIdentifierName().ToFullString());
    }

    [Fact]
    public void DescriptorFieldDeclaration_FromDescriptorPropertyReferenceTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        var expected = "private static readonly DataVariableDescriptor _Descriptor = global::MyApp.Variables.VariableDescriptors.TestDescriptor;";
        var fieldDecl = classModel.DescriptorFieldDeclaration().NormalizeWhitespace().ToFullString();

        Assert.Equal(expected, fieldDecl);
    }

    [Fact]
    public void DescriptorFieldDeclaration_FromLiteralValuesTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            false,
            true);

        var expected = "private static readonly DataVariableDescriptor _Descriptor = new DataVariableDescriptor(\"Test\", DataVariableValueType.Int, 3);";
        var fieldDecl = classModel.DescriptorFieldDeclaration().NormalizeWhitespace().ToFullString();

        Assert.Equal(expected, fieldDecl);
    }

    [Fact]
    public void DescriptorFieldIdentifier_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            false,
            true);

        Assert.Equal("_Descriptor", classModel.DescriptorFieldIdentifier().ToFullString());
    }

    [Fact]
    public void DescriptorFieldIdentifierName_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            false,
            true);

        Assert.Equal("_Descriptor", classModel.DescriptorFieldIdentifierName().ToFullString());
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

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableClassModel model1, VariableClassModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void ICreateDataVariableBaseType_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            isClassInternal: false,
            isClassPartial: true);

        Assert.Equal("ICreateDataVariable<TestVariable>", classModel.ICreateDataVariableBaseType().ToFullString());
    }

    [Fact]
    public void VariableValueTypeArg_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            false,
            true);

        Assert.Equal("int", classModel.VariableValueTypeArg().ToFullString());
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

    public static TheoryData<VariableClassModel, VariableClassModel> GetInequalityData()
    {
        var data = new TheoryData<VariableClassModel, VariableClassModel>
        {
            // Class name
            { CreateVariableClassModel(className: "Test"), CreateVariableClassModel(className: "Test2") },

            // Namespace
            { CreateVariableClassModel(classNamespace: "Test.App"), CreateVariableClassModel(classNamespace: "Test.App.Variables") },

            // Variable name
            { CreateVariableClassModel(variableName: "Test"), CreateVariableClassModel(variableName: "Test2") },

            // Value type
            { CreateVariableClassModel(valueType: VariableValueType.Float), CreateVariableClassModel(valueType: VariableValueType.Double) },

            // Value count
            { CreateVariableClassModel(valueCount: 1), CreateVariableClassModel(valueCount: 2) },

            // Value unit
            { CreateVariableClassModel(valueUnit: "s"), CreateVariableClassModel(valueUnit: "test/s") },

            // IsClassInternal
            { CreateVariableClassModel(isClassInternal: true), CreateVariableClassModel(isClassInternal: false) },

            // IsClassPartial
            { CreateVariableClassModel(isClassPartial: true), CreateVariableClassModel(isClassPartial: false) },

            // Non-null descriptor property references
            {
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty", "MyDescriptors", "Test.App")),
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty2", "MyDescriptors", "Test.App"))
            },

            // One null descriptor property reference
            {
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty", "MyDescriptors", "Test.App")),
                CreateVariableClassModel(descriptorPropertyReferenceFactory: _ => null)
            }
        };

        return data;
    }

    private static VariableClassModel CreateVariableClassModel(
        string className = "TestClass",
        string classNamespace = "Test.App.Variables",
        string variableName = "Test",
        VariableValueType valueType = VariableValueType.Int,
        int valueCount = 1,
        string? valueUnit = null,
        Func<string, DescriptorPropertyReference?>? descriptorPropertyReferenceFactory = null,
        bool isClassInternal = false,
        bool isClassPartial = true)
    {
        var variableInfo = new VariableInfo(variableName, valueType, valueCount, "test", valueUnit, false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        DescriptorPropertyReference? descriptorPropertyReference = null;

        descriptorPropertyReference = descriptorPropertyReferenceFactory?.Invoke(variableName);

        return new VariableClassModel(className, classNamespace, variableModel, descriptorPropertyReference, isClassInternal, isClassPartial);
    }
}
