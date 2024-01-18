using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SharpRacer.SourceGenerators.Testing.Syntax;
internal class TriviaRemover : CSharpSyntaxRewriter
{
    public static TriviaRemover Default { get; } = new TriviaRemover();

    [return: NotNullIfNotNull(nameof(node))]
    public override SyntaxNode? Visit(SyntaxNode? node)
    {
        var visitedNode = base.Visit(node);

        if (visitedNode is null)
        {
            return null;
        }

        return visitedNode.WithoutTrivia();
    }

    public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
    {
        return default;
    }
}
