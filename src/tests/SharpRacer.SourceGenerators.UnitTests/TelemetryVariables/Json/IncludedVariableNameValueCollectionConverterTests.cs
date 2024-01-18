using System.Text;
using System.Text.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class IncludedVariableNameValueCollectionConverterTests
{
    [Fact]
    public void Read_ThrowJsonExceptionIfFirstTokenIsNotStartArrayTest()
    {
        var converter = new IncludedVariableNameValueCollectionConverter();

        var ex = Assert.Throws<JsonException>(() =>
        {
            ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes("{ \"test\" }");
            var reader = new Utf8JsonReader(json);
            reader.Read();

            converter.Read(ref reader, typeof(IncludedVariableName), new JsonSerializerOptions());
        });

        Assert.Equal($"Expected token type '{JsonTokenType.StartArray}'", ex.Message);
    }

    [Fact]
    public void Read_ThrowJsonExceptionIfValueStartTokenIsNotStringTest()
    {
        var converter = new IncludedVariableNameValueCollectionConverter();

        var ex = Assert.Throws<JsonException>(() =>
        {
            ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes(@"[ { ""Test"": null } ]");
            var reader = new Utf8JsonReader(json);
            reader.Read();

            converter.Read(ref reader, typeof(IncludedVariableName), new JsonSerializerOptions());
        });

        Assert.Equal($"Expected token type '{JsonTokenType.String}'", ex.Message);
    }

    [Fact]
    public void Write_ThrowsNotImplementedExceptionTest()
    {
        var converter = new IncludedVariableNameValueCollectionConverter();

        Assert.Throws<NotImplementedException>(() => converter.Write(null!, [], new JsonSerializerOptions()));
    }
}
