using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
internal class XmlDocumentationTriviaBuilder
{
    private readonly List<XmlElementSyntax> _exceptions;
    private readonly List<XmlElementSyntax> _params;
    private XmlElementSyntax? _remarks;
    private XmlElementSyntax? _summary;

    public XmlDocumentationTriviaBuilder()
    {
        _exceptions = new List<XmlElementSyntax>();
        _params = new List<XmlElementSyntax>();
    }

    public XmlDocumentationTriviaBuilder Exception(CrefSyntax exceptionCref, Action<XmlElementContentBuilder> configure)
    {
        var nodes = new List<XmlNodeSyntax>();

        var builder = new XmlElementContentBuilder(nodes);
        configure(builder);

        _exceptions.Add(XmlExceptionElement(exceptionCref, FormatMultiLineContent(nodes)));

        return this;
    }

    public XmlDocumentationTriviaBuilder Remarks(string text)
    {
        _remarks = XmlRemarksElement(FormatMultiLineContent([XmlText(text)]));

        return this;
    }

    public XmlDocumentationTriviaBuilder Remarks(Action<XmlElementContentBuilder> configure)
    {
        var nodes = new List<XmlNodeSyntax>();

        var builder = new XmlElementContentBuilder(nodes);
        configure(builder);

        _remarks = XmlRemarksElement(FormatMultiLineContent(nodes));

        return this;
    }

    public XmlDocumentationTriviaBuilder Summary(string text)
    {
        _summary = XmlSummaryElement(FormatMultiLineContent([XmlText(text)]));

        return this;
    }

    public XmlDocumentationTriviaBuilder Summary(Action<XmlElementContentBuilder> configure)
    {
        var nodes = new List<XmlNodeSyntax>();

        var builder = new XmlElementContentBuilder(nodes);
        configure(builder);

        _summary = XmlSummaryElement(FormatMultiLineContent(nodes));

        return this;
    }

    public SyntaxList<XmlNodeSyntax> ToSyntaxList()
    {
        var nodes = new List<XmlNodeSyntax>();

        var docElements = EnumerateElements().ToArray();

        for (int i = 0; i < docElements.Length; i++)
        {
            if (i > 0)
            {
                // Need new-line following previous tag
                nodes.Add(XmlText(XmlTextNewLine("\n", continueXmlDocumentationComment: false)));
            }

            // Space before opening tag
            nodes.Add(XmlText(" ").WithLeadingTrivia(DocumentationCommentExterior("///")));

            // Add element
            nodes.Add(docElements[i]);
        }

        // Trailing new-line
        nodes.Add(XmlText(XmlTextNewLine("\n", continueXmlDocumentationComment: false)));

        return List(nodes);
    }

    public DocumentationCommentTriviaSyntax ToTrivia()
    {
        return DocumentationCommentTrivia(
            SyntaxKind.MultiLineDocumentationCommentTrivia,
            ToSyntaxList());
    }

    private IEnumerable<XmlNodeSyntax> EnumerateElements()
    {
        if (_summary != null)
        {
            yield return _summary;
        }

        foreach (var paramElement in _params)
        {
            yield return paramElement;
        }

        foreach (var exceptionElement in _exceptions)
        {
            yield return exceptionElement;
        }

        if (_remarks != null)
        {
            yield return _remarks;
        }
    }

    private static SyntaxList<XmlNodeSyntax> FormatMultiLineContent(IEnumerable<XmlNodeSyntax> nodes)
    {
        var contentNodes = new List<XmlNodeSyntax>
        {
            // New-line after opening tag and space before content
            XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" "))
        };

        contentNodes.AddRange(nodes);

        // New-line after content and space before closing tag
        contentNodes.Add(XmlText(XmlTextNewLine("\n"), XmlTextLiteral(" ")));

        return List(contentNodes);
    }
}
