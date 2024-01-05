using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
internal class XmlElementContentBuilder
{
    private readonly List<XmlNodeSyntax> _nodes;

    public XmlElementContentBuilder(List<XmlNodeSyntax> nodes)
    {
        _nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
    }

    public XmlElementContentBuilder Langword(string langword)
    {
        var element = XmlEmptyElement("see")
            .AddAttributes(XmlTextAttribute("langword", langword));

        _nodes.Add(element);

        return this;
    }

    public XmlElementContentBuilder NullKeyword()
    {
        _nodes.Add(XmlNullKeywordElement());

        return this;
    }

    public XmlElementContentBuilder OrSeparator()
    {
        var node = XmlText(
            XmlTextNewLine("\n"),
            XmlTextNewLine("\n"),
            XmlTextLiteral(" -OR-"),
            XmlTextNewLine("\n"),
            XmlTextNewLine("\n"),
            XmlTextLiteral(" "));

        /*_nodes.Add(XmlNewLine("\n"));
        _nodes.Add(XmlNewLine("\n"));
        _nodes.Add(XmlText("-OR-"));
        _nodes.Add(XmlNewLine("\n"));
        _nodes.Add(XmlNewLine("\n"));*/

        _nodes.Add(node);

        return this;
    }

    public XmlElementContentBuilder ParamRef(string paramName)
    {
        _nodes.Add(XmlParamRefElement(paramName));

        return this;
    }

    public XmlElementContentBuilder See(TypeSyntax typeSyntax)
    {
        if (typeSyntax is null)
        {
            throw new ArgumentNullException(nameof(typeSyntax));
        }

        _nodes.Add(XmlSeeElement(TypeCref(typeSyntax)));

        return this;
    }

    public XmlElementContentBuilder See(CrefSyntax crefSyntax)
    {
        if (crefSyntax is null)
        {
            throw new ArgumentNullException(nameof(crefSyntax));
        }

        _nodes.Add(XmlSeeElement(crefSyntax));

        return this;
    }

    public XmlElementContentBuilder SeeAlso(TypeSyntax typeSyntax)
    {
        if (typeSyntax is null)
        {
            throw new ArgumentNullException(nameof(typeSyntax));
        }

        _nodes.Add(XmlSeeAlsoElement(TypeCref(typeSyntax)));

        return this;
    }

    public XmlElementContentBuilder SeeAlso(Uri link, string linkText)
    {
        if (link is null)
        {
            throw new ArgumentNullException(nameof(link));
        }

        if (string.IsNullOrEmpty(linkText))
        {
            throw new ArgumentException($"'{nameof(linkText)}' cannot be null or empty.", nameof(linkText));
        }

        _nodes.Add(XmlSeeAlsoElement(link, SingletonList<XmlNodeSyntax>(XmlText(linkText))));

        return this;
    }

    public XmlElementContentBuilder Text(string text)
    {
        _nodes.Add(XmlText(text));

        return this;
    }
}
