using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
public class SyntaxFactoryHelpersTests
{
    [Fact]
    public static void GeneratedCodeAttribute_Test()
    {
        var attr = SyntaxFactoryHelpers.GeneratedCodeAttribute().NormalizeWhitespace().ToFullString();

        var expected = $"System.CodeDom.Compiler.GeneratedCodeAttribute(\"{TelemetryVariablesGenerator.ToolName}\", \"{TelemetryVariablesGenerator.ToolVersion}\")";
        Assert.Equal(expected, attr);
    }

    [Fact]
    public static void GeneratedCodeAttribute_WithToolNameAndVersionArgsTest()
    {
        string toolName = "SharpRacer.SourceGenerators.UnitTests";
        string toolVersion = "1.2.3.4";

        var attr = SyntaxFactoryHelpers.GeneratedCodeAttribute(toolName, toolVersion).NormalizeWhitespace().ToFullString();

        var expected = $"System.CodeDom.Compiler.GeneratedCodeAttribute(\"{toolName}\", \"{toolVersion}\")";
        Assert.Equal(expected, attr);
    }

    [Fact]
    public void ModifiersFromAccessibility_InternalTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Internal).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.InternalKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_PrivateTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Private).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.PrivateKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_ProtectedTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Protected).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.ProtectedKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void ModifiersFromAccessibility_ProtectedInternalTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.ProtectedAndInternal).ToList();

        Assert.Equal(2, tokens.Count);

        Assert.Contains(SyntaxKind.ProtectedKeyword, tokens.Select(x => x.Kind()));
        Assert.Contains(SyntaxKind.InternalKeyword, tokens.Select(x => x.Kind()));
    }

    [Fact]
    public void ModifiersFromAccessibility_PublicTest()
    {
        var tokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(Accessibility.Public).ToList();

        Assert.Single(tokens);
        Assert.Equal(SyntaxKind.PublicKeyword, tokens.Single().Kind());
    }

    [Fact]
    public void NullCheck_IdentifierTest()
    {
        var result = SyntaxFactoryHelpers.NullCheck("myVar").NormalizeWhitespace(eol: "\r\n").ToFullString();

        var expected = "if (myVar is null)\r\n{\r\n    throw new ArgumentNullException(\"myVar\");\r\n}";

        Assert.Equal(expected, result);
    }

    [Fact]
    public void NullCheck_IdentifierNameTest()
    {
        var result = SyntaxFactoryHelpers.NullCheck(IdentifierName("myVar")).NormalizeWhitespace(eol: "\r\n").ToFullString();

        var expected = "if (myVar is null)\r\n{\r\n    throw new ArgumentNullException(\"myVar\");\r\n}";

        Assert.Equal(expected, result);
    }

    [Fact]
    public void NullCheck_ThrowOnNullOrEmptyIdentifierStringTest()
    {
        string identifier = null!;
        Assert.Throws<ArgumentException>(() => SyntaxFactoryHelpers.NullCheck(identifier));
    }
}
