namespace SharpRacer.SourceGenerators;
public class TypeIdentifierTests
{
    [Fact]
    public void Ctor_Test()
    {
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier("MyType", namespaceIdentifier);

        Assert.Equal(namespaceIdentifier, identifier.Namespace);
        Assert.Equal("MyType", identifier.TypeName);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyTypeNameTest()
    {
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");

        Assert.Throws<ArgumentException>(() => new TypeIdentifier(null!, namespaceIdentifier));
        Assert.Throws<ArgumentException>(() => new TypeIdentifier(string.Empty, namespaceIdentifier));
    }

    [Fact]
    public void Ctor_ThrowOnDefaultValueNamespaceIdentifierTest()
    {
        Assert.Throws<ArgumentException>(() => new TypeIdentifier("MyType", default));
    }

    [Fact]
    public void Equals_IdenticalValueTest()
    {
        var identifier1 = new TypeIdentifier("MyType", new NamespaceIdentifier("Test.App"));
        var identifier2 = new TypeIdentifier("MyType", new NamespaceIdentifier("Test.App"));

        EquatableStructAssert.Equal(identifier1, identifier2);
        EquatableStructAssert.NotEqual(identifier1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, identifier1, int.MaxValue);
    }

    [Fact]
    public void Equals_SameTypeDifferentNamespaceTest()
    {
        var identifier1 = new TypeIdentifier("MyType", new NamespaceIdentifier("Test.App1"));
        var identifier2 = new TypeIdentifier("MyType", new NamespaceIdentifier("Test.App2"));

        EquatableStructAssert.NotEqual(identifier1, identifier2);
    }

    [Fact]
    public void Equals_DifferentValueTest()
    {
        var identifier1 = new TypeIdentifier("MyType1", new NamespaceIdentifier("Test.App1"));
        var identifier2 = new TypeIdentifier("MyType2", new NamespaceIdentifier("Test.App2"));

        EquatableStructAssert.NotEqual(identifier1, identifier2);
    }

    [Fact]
    public void ToGlobalQualifiedNameTest()
    {
        string typeName = "MyType";
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier(typeName, namespaceIdentifier);

        var namespaceGlobalQualified = namespaceIdentifier.ToGlobalQualifiedName();

        Assert.Equal($"{namespaceGlobalQualified}.{typeName}", identifier.ToGlobalQualifiedName());

        // Default value
        Assert.Equal(string.Empty, default(TypeIdentifier).ToGlobalQualifiedName());
    }

    [Fact]
    public void ToQualifiedNameTest()
    {
        string typeName = "MyType";
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier(typeName, namespaceIdentifier);

        Assert.Equal($"{namespaceIdentifier}.{typeName}", identifier.ToQualifiedName());

        // Default value
        Assert.Equal(string.Empty, default(TypeIdentifier).ToQualifiedName());
    }

    [Fact]
    public void ToStringTest()
    {
        string typeName = "MyType";
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier(typeName, namespaceIdentifier);

        Assert.Equal($"{namespaceIdentifier}.{typeName}", identifier.ToString());

        Assert.Equal(string.Empty, default(TypeIdentifier).ToString());
    }

    [Fact]
    public void ToString_TypeNameFormatTest()
    {
        string typeName = "MyType";
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier(typeName, namespaceIdentifier);

        var namespaceGlobalQualified = namespaceIdentifier.ToGlobalQualifiedName();

        Assert.Equal(typeName, identifier.ToString(TypeNameFormat.Default));
        Assert.Equal($"{namespaceGlobalQualified}.{typeName}", identifier.ToString(TypeNameFormat.GlobalQualified));
        Assert.Equal($"{namespaceIdentifier}.{typeName}", identifier.ToString(TypeNameFormat.Qualified));

        Assert.Equal(string.Empty, default(TypeIdentifier).ToString(TypeNameFormat.Default));
        Assert.Equal(string.Empty, default(TypeIdentifier).ToString(TypeNameFormat.GlobalQualified));
        Assert.Equal(string.Empty, default(TypeIdentifier).ToString(TypeNameFormat.Qualified));
    }

    [Fact]
    public void ImplicitOperator_String_Test()
    {
        string typeName = "MyType";
        var namespaceIdentifier = new NamespaceIdentifier("Test.App");
        var identifier = new TypeIdentifier(typeName, namespaceIdentifier);

        Assert.Equal(typeName, identifier);

        identifier = default;

        Assert.Equal(string.Empty, identifier);
    }
}
