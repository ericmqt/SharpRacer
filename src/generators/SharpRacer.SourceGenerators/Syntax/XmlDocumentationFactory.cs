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

    public static DocumentationCommentTriviaSyntax CreateDocumentationCommentTrivia(params XmlElementSyntax[] elements)
    {
        var nodes = new List<XmlNodeSyntax>();

        for (int i = 0; i < elements.Length; i++)
        {
            if (i > 0)
            {
                // Need new-line following previous tag
                nodes.Add(XmlText(XmlTextNewLine("\n", continueXmlDocumentationComment: false)));
            }

            // Space before opening tag
            nodes.Add(XmlText(" ").WithLeadingTrivia(DocumentationCommentExterior("///")));

            // Add element
            nodes.Add(elements[i]);
        }

        // Trailing new-line
        nodes.Add(XmlText(XmlTextNewLine("\n", continueXmlDocumentationComment: false)));

        return DocumentationCommentTrivia(
            SyntaxKind.MultiLineDocumentationCommentTrivia,
            List(nodes));
    }

    public static XmlElementSyntax Remarks(params XmlNodeSyntax[] nodes)
    {
        var elementNodes = new List<XmlNodeSyntax>()
        {
            // New-line after opening tag and space before content
            XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" "))
        };

        elementNodes.AddRange(nodes);

        // New-line after content and space before closing tag
        elementNodes.Add(XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" ")));

        return XmlRemarksElement(List(elementNodes));
    }

    public static XmlElementSyntax Summary(params XmlNodeSyntax[] nodes)
    {
        var elementNodes = new List<XmlNodeSyntax>()
        {
            // New-line after opening tag and space before content
            XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" "))
        };

        elementNodes.AddRange(nodes);

        // New-line after content and space before closing tag
        elementNodes.Add(XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" ")));

        return XmlSummaryElement(List(elementNodes));
    }
}
