namespace SharpRacer.SourceGenerators.Syntax;
public class XmlDocumentationTriviaBuilderTests
{
    /*
     * 2024-01-15
     * 
     * NOTE: The extra space character on lines following the opening tag is necessary for now because when NormalizeWhitespace() is called
     * on a syntax node, the extra space gets squashed into one. Otherwise without the extra space, normalized documentation trivia winds
     * up looking like ///<summary>
     * 
     * There is likely a flaw with the way I've constructed the XML documentation nodes and further investigation is needed. For now I'm
     * leaving it because I need to finish this project and the output of NormalizeWhitespace is fine as-is.
     */

    [Fact]
    public void Param_Test()
    {
        var trivia = new XmlDocumentationTriviaBuilder().Param("myArg", "Hello, world!").ToTrivia();

        var expected = @"/// <param name=""myArg"">
///  Hello, world!
///  </param>
".ReplaceLineEndings("\n");

        var triviaStr = trivia.ToFullString();

        Assert.Equal(expected, triviaStr);
    }

    [Fact]
    public void Param_WithBuilderTest()
    {
        var trivia = new XmlDocumentationTriviaBuilder().Param("myArg", b => b.Text("Hello, world!")).ToTrivia();

        var expected = @"/// <param name=""myArg"">
///  Hello, world!
///  </param>
".ReplaceLineEndings("\n");

        var triviaStr = trivia.ToFullString();

        Assert.Equal(expected, triviaStr);
    }

    [Fact]
    public void Remarks_Test()
    {
        var trivia = new XmlDocumentationTriviaBuilder().Remarks("Hello, world!").ToTrivia();

        var expected = @"/// <remarks>
///  Hello, world!
///  </remarks>
".ReplaceLineEndings("\n");

        Assert.Equal(expected, trivia.ToFullString());
    }

    [Fact]
    public void Summary_Test()
    {
        var trivia = new XmlDocumentationTriviaBuilder().Summary("Hello, world!").ToTrivia();

        var expected = @"/// <summary>
///  Hello, world!
///  </summary>
".ReplaceLineEndings("\n");

        var triviaStr = trivia.ToFullString();
        Assert.Equal(expected, triviaStr);
    }
}
