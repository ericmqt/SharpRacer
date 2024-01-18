using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
internal static class XmlDocumentationFactory
{
    public static DocumentationCommentTriviaSyntax InheritDoc()
    {
        var nodes = new List<XmlNodeSyntax>()
        {
            XmlText(" ").WithLeadingTrivia(DocumentationCommentExterior("///")),
            XmlEmptyElement("inheritdoc"),
            XmlText(XmlTextNewLine("\n", continueXmlDocumentationComment: false))
        };

        return DocumentationCommentTrivia(
            SyntaxKind.SingleLineDocumentationCommentTrivia,
            List(nodes));
    }
}
