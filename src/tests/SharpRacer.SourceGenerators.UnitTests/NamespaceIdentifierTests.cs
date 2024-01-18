using Microsoft.CodeAnalysis;
using Moq;

namespace SharpRacer.SourceGenerators;
public class NamespaceIdentifierTests
{
    [Fact]
    public void Ctor_Test()
    {
        string namespaceStr = "Test.App";

        var identifier = new NamespaceIdentifier(namespaceStr);

        Assert.Equal(namespaceStr, identifier.ToString());
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyStringArgTest()
    {
        Assert.Throws<ArgumentException>(() => new NamespaceIdentifier(null!));
        Assert.Throws<ArgumentException>(() => new NamespaceIdentifier(string.Empty));
    }

    [Fact]
    public void CreateType_Test()
    {
        string namespaceStr = "Test.App";
        string typeName = "MyType";

        var identifier = new NamespaceIdentifier(namespaceStr);
        var typeIdentifier = identifier.CreateType(typeName);

        Assert.NotEqual(default, typeIdentifier);
        Assert.Equal(typeName, typeIdentifier.TypeName);
        Assert.Equal(identifier, typeIdentifier.Namespace);
    }

    [Fact]
    public void CreateType_ThrowOnNullOrEmptyTypeNameTest()
    {
        var identifier = new NamespaceIdentifier("Test.App");

        Assert.Throws<ArgumentException>(() => identifier.CreateType(null!));
        Assert.Throws<ArgumentException>(() => identifier.CreateType(string.Empty));
    }

    [Fact]
    public void EqualsTest()
    {
        var identifier1 = new NamespaceIdentifier("Test.App");
        var identifier2 = new NamespaceIdentifier("Test.App");

        EquatableStructAssert.Equal(identifier1, identifier2);

        identifier2 = new NamespaceIdentifier("TestApp.Tests");

        EquatableStructAssert.NotEqual(identifier1, identifier2);
        EquatableStructAssert.ObjectEqualsMethod(false, identifier1, int.MaxValue);

        EquatableStructAssert.NotEqual(identifier1, default);
    }

    [Fact]
    public void Equals_INamespaceSymbolTest()
    {
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::Test.App");

        var identifier = new NamespaceIdentifier("Test.App");

        Assert.True(identifier.Equals(namespaceSymbolMock.Object));
    }

    [Fact]
    public void Equals_INamespaceSymbol_DefaultValueTest()
    {
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::Test.App");

        NamespaceIdentifier identifier = default;

        Assert.False(identifier.Equals(namespaceSymbolMock.Object));
    }

    [Fact]
    public void ImplicitOperator_String_Test()
    {
        string namespaceStr = "Test.App";

        var identifier = new NamespaceIdentifier(namespaceStr);

        Assert.Equal(namespaceStr, identifier);

        identifier = default;

        Assert.Equal(string.Empty, identifier);
    }

    [Fact]
    public void ToGlobalQualifiedName_Test()
    {
        string namespaceStr = "Test.App";

        var identifier = new NamespaceIdentifier(namespaceStr);

        Assert.Equal(namespaceStr, identifier.ToString(qualifyGlobal: false));
        Assert.Equal($"global::{namespaceStr}", identifier.ToGlobalQualifiedName());
        Assert.Equal(identifier.ToString(qualifyGlobal: true), identifier.ToGlobalQualifiedName());

        identifier = default;
        Assert.Equal(string.Empty, identifier.ToGlobalQualifiedName());
    }

    [Fact]
    public void ToString_Test()
    {
        string namespaceStr = "Test.App";

        var identifier = new NamespaceIdentifier(namespaceStr);

        Assert.Equal(namespaceStr, identifier.ToString(qualifyGlobal: false));
        Assert.Equal($"global::{namespaceStr}", identifier.ToString(qualifyGlobal: true));

        identifier = default;
        Assert.Equal(string.Empty, identifier.ToString());
        Assert.Equal(string.Empty, identifier.ToString(qualifyGlobal: true));
        Assert.Equal(string.Empty, identifier.ToString(qualifyGlobal: false));
    }
}

