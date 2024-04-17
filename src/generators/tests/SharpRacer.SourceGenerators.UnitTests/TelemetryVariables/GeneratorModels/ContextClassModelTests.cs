using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Moq;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class ContextClassModelTests
{
    [Fact]
    public void Ctor_Test()
    {
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo = CreateContextClassInfo(className, classNamespace);
        var contextVariableModels = ImmutableArray<ContextVariableModel>.Empty;

        var model = new ContextClassModel(classInfo, contextVariableModels);

        Assert.Equal(className, model.ClassName);
        Assert.Equal(classNamespace, model.ClassNamespace);
        Assert.False(model.Variables.IsDefault);
        Assert.Empty(model.Variables);
    }

    [Fact]
    public void Ctor_FromStringsTest()
    {
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var model = new ContextClassModel(className, classNamespace, []);

        Assert.Equal(className, model.ClassName);
        Assert.Equal(classNamespace, model.ClassNamespace);
        Assert.False(model.Variables.IsDefault);
        Assert.Empty(model.Variables);
    }

    [Fact]
    public void ClassIdentifier_Test()
    {
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo = CreateContextClassInfo(className, classNamespace);
        var model = new ContextClassModel(classInfo, ImmutableArray<ContextVariableModel>.Empty);

        var classIdentifier = model.ClassIdentifier();
        Assert.Equal(className, classIdentifier.ValueText);
    }

    [Fact]
    public void ClassIdentifierName_Test()
    {
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo = CreateContextClassInfo(className, classNamespace);
        var model = new ContextClassModel(classInfo, ImmutableArray<ContextVariableModel>.Empty);

        var classIdentifier = model.ClassIdentifierName();

        Assert.Equal(className, classIdentifier.ToFullString());
    }

    [Fact]
    public void Equals_Test()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        // Create class models
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo1 = CreateContextClassInfo(className, classNamespace);
        var classInfo2 = CreateContextClassInfo(className, classNamespace);

        var model1 = new ContextClassModel(classInfo1, [contextVariableModel]);
        var model2 = new ContextClassModel(classInfo2, [contextVariableModel]);

        EquatableStructAssert.Equal(model1, model2);
        EquatableStructAssert.NotEqual(model1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, model1, int.MinValue);
        EquatableStructAssert.Equal<ContextClassModel>(default, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(ContextClassModel model1, ContextClassModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    private static ContextClassInfo CreateContextClassInfo(string className, string classNamespace)
    {
        var classTypeSymbol = new Mock<INamedTypeSymbol>(MockBehavior.Strict);
        var classContainingNamespaceSymbol = new Mock<INamespaceSymbol>();

        classContainingNamespaceSymbol.Setup(x => x.ToString())
            .Returns(classNamespace);

        classTypeSymbol.SetupGet(x => x.Name)
            .Returns(className);

        classTypeSymbol.SetupGet(x => x.ContainingNamespace)
            .Returns(classContainingNamespaceSymbol.Object);

        return new ContextClassInfo(classTypeSymbol.Object, Location.None);
    }

    public static TheoryData<ContextClassModel, ContextClassModel> GetInequalityData()
    {
        var data = new TheoryData<ContextClassModel, ContextClassModel>()
        {
            // Type name
            {
                new ContextClassModel("MyContext", "Test.App.Contexts", []),
                new ContextClassModel("MyContext2", "Test.App.Contexts", [])
            },

            // Type namespace
            {
                new ContextClassModel("MyContext", "Test.App.Contexts", []),
                new ContextClassModel("MyContext", "Test.App.Variables.Contexts", [])
            }
        };

        // Different variables
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        data.Add(
            new ContextClassModel("MyContext", "Test.App.Contexts", []),
            new ContextClassModel("MyContext", "Test.App.Contexts", [contextVariableModel]));

        return data;
    }
}
