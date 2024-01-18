using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
public class XmlElementContentBuilderTests
{
    [Fact]
    public void Langword_Test()
    {
        var nodes = new XmlElementContentBuilder().Langword("false").Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<see langword=\"false\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void NullKeyword_Test()
    {
        var nodes = new XmlElementContentBuilder().NullKeyword().Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<see langword=\"null\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void OrSeparator_Test()
    {
        var nodes = new XmlElementContentBuilder().OrSeparator().Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        var expected = @"
/// 
///  -OR-
/// 
///  ".ReplaceLineEndings("\n");

        Assert.Equal(expected, nodes.First().ToFullString());
    }

    [Fact]
    public void ParamRef_Test()
    {
        var nodes = new XmlElementContentBuilder().ParamRef("myArg").Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<paramref name=\"myArg\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void See_CrefSyntax_Test()
    {
        var nodes = new XmlElementContentBuilder().See(TypeCref(ParseTypeName("int"))).Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<see cref=\"int\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void See_CrefSyntax_ThrowOnNullArgTest()
    {
        var builder = new XmlElementContentBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.See(crefSyntax: null!));
    }

    [Fact]
    public void See_TypeSyntax_Test()
    {
        var nodes = new XmlElementContentBuilder().See(ParseTypeName("int")).Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<see cref=\"int\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void See_TypeSyntax_ThrowOnNullArgTest()
    {
        var builder = new XmlElementContentBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.See(typeSyntax: null!));
    }

    [Fact]
    public void SeeAlso_TypeSyntax_Test()
    {
        var nodes = new XmlElementContentBuilder().SeeAlso(ParseTypeName("int")).Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal("<seealso cref=\"int\"/>", nodes.First().ToFullString());
    }

    [Fact]
    public void SeeAlso_TypeSyntax_ThrowOnNullArgTest()
    {
        var builder = new XmlElementContentBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.SeeAlso(typeSyntax: null!));
    }

    [Fact]
    public void SeeAlso_UriLink_Test()
    {
        var uri = new Uri("https://example.org/documentation/test.html");
        var linkText = "documentation";

        var nodes = new XmlElementContentBuilder().SeeAlso(uri, linkText).Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        var expected = "<seealso cref=\"https://example.org/documentation/test.html\">documentation</seealso>";
        Assert.Equal(expected, nodes.First().ToFullString());
    }

    [Fact]
    public void SeeAlso_UriLink_ThrowOnNullUriArgTest()
    {
        var builder = new XmlElementContentBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.SeeAlso(link: null!, "link text"));
    }

    [Fact]
    public void SeeAlso_UriLink_ThrowOnNullOrEmptyLinkTextArgTest()
    {
        var uri = new Uri("https://example.org/documentation/test.html");
        var builder = new XmlElementContentBuilder();

        Assert.Throws<ArgumentException>(() => builder.SeeAlso(uri, linkText: null!));
        Assert.Throws<ArgumentException>(() => builder.SeeAlso(uri, linkText: string.Empty));
    }

    [Fact]
    public void Text_Test()
    {
        var textValue = "Hello, world";
        var nodes = new XmlElementContentBuilder().Text(textValue).Build();

        Assert.NotNull(nodes);
        Assert.Single(nodes);

        Assert.Equal(textValue, nodes.First().ToFullString());
    }
}
