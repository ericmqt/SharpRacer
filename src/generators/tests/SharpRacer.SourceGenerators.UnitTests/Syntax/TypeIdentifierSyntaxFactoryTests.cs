using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;

public class TypeIdentifierSyntaxFactoryTests
{
    [Fact]
    public void GlobalQualifiedTypeName_Test()
    {
        var result = TypeIdentifierSyntaxFactory.GlobalQualifiedTypeName(
            SharpRacerIdentifiers.DataVariableDescriptor.Namespace,
            IdentifierName(SharpRacerIdentifiers.DataVariableDescriptor.TypeName));

        var expected = ParseTypeName(SharpRacerIdentifiers.DataVariableDescriptor.ToString(TypeNameFormat.GlobalQualified));
        var expectedStr = expected.ToFullString();
        var resultStr = result.ToFullString();

        SyntaxAssert.StructuralEquivalent(expected, result);
    }

    [Fact]
    public void GlobalQualifiedTypeName_SingleNamespaceParentTest()
    {
        var typeId = new TypeIdentifier("Test", new NamespaceIdentifier("System"));

        var expected = ParseTypeName(typeId.ToString(TypeNameFormat.GlobalQualified));
        var result = TypeIdentifierSyntaxFactory.GlobalQualifiedTypeName(typeId);

        SyntaxAssert.StructuralEquivalent(expected, result);
    }

    [Fact]
    public void GlobalQualifiedTypeName_NestedNamespaceParentTest()
    {
        var typeId = new TypeIdentifier("Test", new NamespaceIdentifier("System.Testing"));

        var expected = ParseTypeName(typeId.ToString(TypeNameFormat.GlobalQualified));
        var result = TypeIdentifierSyntaxFactory.GlobalQualifiedTypeName(typeId);

        SyntaxAssert.StructuralEquivalent(expected, result);
    }

    [Fact]
    public void QualifiedTypeName_Test()
    {
        var result = TypeIdentifierSyntaxFactory.QualifiedTypeName(
            SharpRacerIdentifiers.DataVariableDescriptor.Namespace,
            IdentifierName(SharpRacerIdentifiers.DataVariableDescriptor.TypeName));

        var expected = ParseTypeName(SharpRacerIdentifiers.DataVariableDescriptor.ToString(TypeNameFormat.Qualified));
        var expectedStr = expected.ToFullString();
        var resultStr = result.ToFullString();

        SyntaxAssert.StructuralEquivalent(expected, result);
    }

    [Fact]
    public void QualifiedTypeName_SingleNamespaceParentTest()
    {
        var typeId = new TypeIdentifier("Test", new NamespaceIdentifier("System"));

        var expected = ParseTypeName(typeId.ToString(TypeNameFormat.Qualified));
        var result = TypeIdentifierSyntaxFactory.QualifiedTypeName(typeId);

        SyntaxAssert.StructuralEquivalent(expected, result);
    }

    [Fact]
    public void QualifiedTypeName_NestedNamespaceParentTest()
    {
        var typeId = new TypeIdentifier("Test", new NamespaceIdentifier("System.Testing"));

        var expected = ParseTypeName(typeId.ToString(TypeNameFormat.Qualified));
        var result = TypeIdentifierSyntaxFactory.QualifiedTypeName(typeId);

        SyntaxAssert.StructuralEquivalent(expected, result);
    }
}
