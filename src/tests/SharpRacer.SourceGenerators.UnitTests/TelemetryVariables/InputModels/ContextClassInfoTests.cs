using Microsoft.CodeAnalysis;
using Moq;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
public class ContextClassInfoTests
{
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
    public void Equals_WrongTypeObjectTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);

        EquatableStructAssert.ObjectEqualsMethod(false, classInfo1, DateTime.MinValue);
    }

    [Fact]
    public void Equals_IdenticalValueTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);
        var typeSymbol2 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);
        var classInfo2 = new ContextClassInfo(typeSymbol2, Location.None);

        EquatableStructAssert.Equal(classInfo1, classInfo2);
        EquatableStructAssert.Equal(classInfo2, classInfo1);
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        string className = "MyTestClass";
        string classNamespace = "TestAssembly";

        var typeSymbol1 = CreateContextClassNamedTypeSymbol(className, classNamespace);

        var classInfo1 = new ContextClassInfo(typeSymbol1, Location.None);

        EquatableStructAssert.NotEqual(classInfo1, default);
        EquatableStructAssert.NotEqual(default, classInfo1);
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
}
