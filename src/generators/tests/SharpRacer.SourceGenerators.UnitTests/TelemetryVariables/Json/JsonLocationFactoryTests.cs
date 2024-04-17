using System.Text.Json;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonLocationFactoryTests
{
    [Fact]
    public void Ctor_ThrowOnNullOrEmptyFilePathArgTest()
    {
        Assert.Throws<ArgumentException>(() => new JsonLocationFactory(filePath: null!, new NullSourceText()));
        Assert.Throws<ArgumentException>(() => new JsonLocationFactory(filePath: string.Empty, new NullSourceText()));
    }

    [Fact]
    public void Ctor_ThrowOnNullSourceTextArgTest()
    {
        Assert.Throws<ArgumentNullException>(() => new JsonLocationFactory("test.json", sourceText: null!));
    }

    [Fact]
    public void GetLocation_ReturnsNullOnJsonExceptionWithNoLineNumberTest()
    {
        var jsonEx = new JsonException("Test", "test.json", null, null);

        Assert.Null(new JsonLocationFactory("test.json", new NullSourceText()).GetLocation(jsonEx));
    }

    [Fact]
    public void GetLocation_ReturnsWholeLineSpanOnJsonExceptionWithBytePositionInLineBeyondLineLengthTest()
    {
        var json = @"{
    { ""test"": null
}".ReplaceLineEndings("\n");
        var jsonEx = new JsonException("Test", "test.json", 1, 4000);

        var location = new JsonLocationFactory("test.json", new JsonSourceText(json)).GetLocation(jsonEx);

        Assert.NotNull(location);
        Assert.Equal("    { \"test\": null".Length, location.SourceSpan.Length);
    }

}
