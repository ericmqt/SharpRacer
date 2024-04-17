using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Testing.Syntax;
using Xunit;

namespace SharpRacer.SourceGenerators;
public static class SyntaxAssert
{
    public static void CompilationUnitStringEqual(string expected, CompilationUnitSyntax actual)
    {
        var expectedCompilationUnit = ParseSyntaxTree(expected).GetCompilationUnitRoot();

        CompilationUnitStringEqual(expectedCompilationUnit, actual);
    }

    public static void CompilationUnitStringEqual(CompilationUnitSyntax expected, CompilationUnitSyntax actual)
    {
        NormalizedStringEqual(expected, actual);
    }

    public static void SyntaxTreeStringEqual(
        string expected,
        SyntaxTree actual,
        [CallerArgumentExpression(nameof(expected))] string expectedArg = null!,
        [CallerArgumentExpression(nameof(actual))] string actualArg = null!)
    {
        SyntaxTreeStringEqual(ParseSyntaxTree(expected), actual, expectedArg, actualArg);
    }

    public static void SyntaxTreeStringEqual(
        SyntaxTree expected,
        SyntaxTree actual,
        [CallerArgumentExpression(nameof(expected))] string expectedArg = null!,
        [CallerArgumentExpression(nameof(actual))] string actualArg = null!)
    {
        var expectedCompilationUnit = expected.GetCompilationUnitRoot();
        var actualCompilationUnit = actual.GetCompilationUnitRoot();

        NormalizedStringEqual(expectedCompilationUnit, actualCompilationUnit);
    }

    public static void StructuralEquivalent<TNode>(
        TNode expected,
        TNode actual,
        [CallerArgumentExpression(nameof(expected))] string expectedArg = null!,
        [CallerArgumentExpression(nameof(actual))] string actualArg = null!)
        where TNode : SyntaxNode
    {
        expected = (TNode)TriviaRemover.Default.Visit(expected);
        actual = (TNode)TriviaRemover.Default.Visit(actual);

        if (SyntaxNodeDifference.TryFindFirst(expected, actual, out var difference))
        {
            throw difference.ToXunitException(expectedArg, actualArg);
        }
    }

    public static void NormalizedStringEqual<TNode>(string expected, TNode actual)
        where TNode : SyntaxNode
    {
        var expectedNodeString = expected.ReplaceLineEndings("\n");
        var actualNodeString = GetNormalizedNode(actual).ToFullString();

        Assert.Equal(expectedNodeString, actualNodeString);
    }

    public static void NormalizedStringEqual<TNode>(TNode expected, TNode actual)
        where TNode : SyntaxNode
    {
        var expectedNodeString = GetNormalizedNode(expected).ToFullString();
        var actualNodeString = GetNormalizedNode(actual).ToFullString();

        Assert.Equal(expectedNodeString, actualNodeString);
    }

    private static TNode GetNormalizedNode<TNode>(TNode node)
        where TNode : SyntaxNode
    {
        return node.NormalizeWhitespace(eol: "\n");
    }

    public static SyntaxTree ParseSyntaxTree(string source)
    {
        return CSharpSyntaxTree.ParseText(
            source.ReplaceLineEndings("\n"),
            CSharpParseOptions.Default.WithDocumentationMode(DocumentationMode.Parse));
    }
}
