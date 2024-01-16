using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Moq;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class ContextClassInfoTests
{
    public static TheoryData<ContextClassInfo, ContextClassInfo> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_Test()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var classTypeSymbol = new Mock<INamedTypeSymbol>(MockBehavior.Strict);
        var classContainingNamespaceSymbol = new Mock<INamespaceSymbol>();

        classContainingNamespaceSymbol.Setup(x => x.ToString())
            .Returns(classNamespace);

        classTypeSymbol.SetupGet(x => x.Name)
            .Returns(className);

        classTypeSymbol.SetupGet(x => x.ContainingNamespace)
            .Returns(classContainingNamespaceSymbol.Object);

        var classInfo = new ContextClassInfo(classTypeSymbol.Object, Location.None);

        Assert.Equal(className, classInfo.ClassName);
        Assert.Equal(classNamespace, classInfo.ClassNamespace);
        Assert.Equal(Location.None, classInfo.GeneratorAttributeLocation);
        Assert.Equal(default, classInfo.IncludedVariables);
    }

    [Fact]
    public void Equals_Test()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);
        var typeSymbol2 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);
        var classInfo2 = new ContextClassInfo(typeSymbol2, Location.None);

        EquatableStructAssert.Equal(classInfo1, classInfo2);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);

        EquatableStructAssert.NotEqual(classInfo1, default);
    }

    [Fact]
    public void Equals_IncludedVariablesTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var includedVariables = new IncludedVariables([new IncludedVariableName("Test", Location.None)]);
        var typeSymbol = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol, Location.None).WithIncludedVariables(includedVariables);
        var classInfo2 = new ContextClassInfo(typeSymbol, Location.None).WithIncludedVariables(includedVariables);

        EquatableStructAssert.Equal(classInfo1, classInfo2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(ContextClassInfo classInfo1, ContextClassInfo classInfo2)
    {
        EquatableStructAssert.NotEqual(classInfo1, classInfo2);
    }

    [Fact]
    public void Equals_WrongTypeObjectTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);

        EquatableStructAssert.ObjectEqualsMethod(false, classInfo1, DateTime.MinValue);
    }

    [Fact]
    public void ToFullyQualifiedName_Test()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol = CreateContextClassNamedTypeSymbol(className, classNamespace);
        var classInfo = new ContextClassInfo(typeSymbol, Location.None);

        Assert.Equal($"{classNamespace}.{className}", classInfo.ToFullyQualifiedName());
    }

    [Fact]
    public void WithIncludedVariables_Test()
    {
        var classTypeSymbol = CreateContextClassNamedTypeSymbol("MyTestClass", "Test.App");

        var originalClassInfo = new ContextClassInfo(classTypeSymbol, Location.None);

        Assert.Equal(default, originalClassInfo.IncludedVariables);

        var includedVariables = new IncludedVariables(
            [
                new IncludedVariableName("SessionTime", Location.None),
                new IncludedVariableName("SessionTick", Location.None)
            ]);

        var mutatedClassInfo = originalClassInfo.WithIncludedVariables(includedVariables);

        EquatableStructAssert.NotEqual(originalClassInfo, mutatedClassInfo);
        Assert.Equal(includedVariables, mutatedClassInfo.IncludedVariables);
        Assert.Equal(2, mutatedClassInfo.IncludedVariables.VariableNames.Length);

        Assert.Single(mutatedClassInfo.IncludedVariables.VariableNames, x => x.Value == "SessionTime");
        Assert.Single(mutatedClassInfo.IncludedVariables.VariableNames, x => x.Value == "SessionTick");
    }

    private static INamedTypeSymbol CreateContextClassNamedTypeSymbol(string className, string classNamespace)
    {
        var classTypeSymbol = new Mock<INamedTypeSymbol>(MockBehavior.Strict);
        var classContainingNamespaceSymbol = new Mock<INamespaceSymbol>();

        classContainingNamespaceSymbol.Setup(x => x.ToString())
            .Returns(classNamespace);

        classTypeSymbol.SetupGet(x => x.Name)
            .Returns(className);

        classTypeSymbol.SetupGet(x => x.ContainingNamespace)
            .Returns(classContainingNamespaceSymbol.Object);

        return classTypeSymbol.Object;
    }

    private static TheoryData<ContextClassInfo, ContextClassInfo> GetInequalityData()
    {
        var fakeLocation = Location.Create(
            "test.json",
            new TextSpan(10, 20),
            new LinePositionSpan(new LinePosition(1, 2), new LinePosition(4, 5)));

        var includedVariables1 = new IncludedVariables([new IncludedVariableName("Test", Location.None)]);
        var includedVariables2 = new IncludedVariables([new IncludedVariableName("TestEx", Location.None)]);

        return new TheoryData<ContextClassInfo, ContextClassInfo>()
        {
            // Class name
            {
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass1", "TestApp.Variables"), Location.None),
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass2", "TestApp.Variables"), Location.None)
            },

            // Class namespace
            {
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None),
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables.Contexts"), Location.None)
            },

            // Location
            {
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None),
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), fakeLocation)
            },

            // IncludedVariables, one is default
            {
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None)
                    .WithIncludedVariables(includedVariables1),
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None)
                    .WithIncludedVariables(default)
            },

            // IncludedVariables, different includes
            {
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None)
                    .WithIncludedVariables(includedVariables1),
                new ContextClassInfo(CreateContextClassNamedTypeSymbol("TestClass", "TestApp.Variables"), Location.None)
                    .WithIncludedVariables(includedVariables2)
            },
        };
    }
}
