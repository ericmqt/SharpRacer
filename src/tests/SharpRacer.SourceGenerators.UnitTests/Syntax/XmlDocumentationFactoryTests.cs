using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.Syntax;
public class XmlDocumentationFactoryTests
{
    [Fact]
    public void InheritDoc_Test()
    {
        var node = XmlDocumentationFactory.InheritDoc();

        var nodeString = node.NormalizeWhitespace(eol: "\n").ToFullString();

        Assert.Equal("/// <inheritdoc/>\n", nodeString);
    }
}
