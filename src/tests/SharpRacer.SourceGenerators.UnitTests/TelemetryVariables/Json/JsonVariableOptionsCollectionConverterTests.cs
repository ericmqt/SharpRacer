using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableOptionsCollectionConverterTests
{
    [Fact]
    public void Read_ThrowJsonExceptionIfFirstTokenIsNotStartObjectTest()
    {
        var converter = new JsonVariableOptionsCollectionConverter();

        var ex = Assert.Throws<JsonException>(() =>
        {
            ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes("[ { \"test\" } ]");
            var reader = new Utf8JsonReader(json);
            reader.Read();

            converter.Read(ref reader, typeof(JsonVariableOptions), new JsonSerializerOptions());
        });

        Assert.Equal($"Expected token type '{JsonTokenType.StartObject}'", ex.Message);
    }

    [Fact]
    public void Read_ThrowJsonExceptionIfKeyIsNullOrEmptyTest()
    {
        var converter = new JsonVariableOptionsCollectionConverter();

        var ex = Assert.Throws<JsonException>(() =>
        {
            ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes("{ \"\": {} }");
            var reader = new Utf8JsonReader(json);
            reader.Read();

            converter.Read(ref reader, typeof(JsonVariableOptions), new JsonSerializerOptions());
        });

        Assert.Equal("Expected key that is not null or empty.", ex.Message);
    }

    [Fact]
    public void Read_ThrowJsonExceptionIfOptionsValueFirstTokenIsNotStartObjectTest()
    {
        var converter = new JsonVariableOptionsCollectionConverter();

        var ex = Assert.Throws<JsonException>(() =>
        {
            ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes("{ \"Test\": [] }");
            var reader = new Utf8JsonReader(json);
            reader.Read();

            converter.Read(ref reader, typeof(JsonVariableOptions), new JsonSerializerOptions());
        });

        Assert.Equal($"Expected token type '{JsonTokenType.StartObject}'", ex.Message);
    }

    [Fact]
    public void Write_Test()
    {
        var options1 = CreateOptions(
            "ChanLatency",
            new JsonVariableOptionsValue("ChannelLatency", "ChannelLatencyVariable"));

        var options2 = CreateOptions(
            "LatAccel",
            new JsonVariableOptionsValue("LateralAcceleration", null));

        var optionsArray = ImmutableArray.Create(options1, options2);

        // Write
        var json = string.Empty;

        using (var memoryStream = new MemoryStream())
        using (var jsonWriter = new Utf8JsonWriter(memoryStream))
        {
            var converter = new JsonVariableOptionsCollectionConverter();
            converter.Write(jsonWriter, optionsArray, TelemetryGeneratorSerializationContext.Default.Options);

            jsonWriter.Flush();

            // Read back
            json = Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        var readOptions = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableOptions);

        Assert.Equal(2, readOptions.Length);
        var readOptions1 = readOptions.Single(x => x.Key == options1.Key);
        var readOptions2 = readOptions.Single(x => x.Key == options2.Key);
        Assert.Equal(options1.Value, readOptions1.Value);
        Assert.Equal(options2.Value, readOptions2.Value);
    }

    private static JsonVariableOptions CreateOptions(string key, JsonVariableOptionsValue value, int spanStart = 0)
    {
        var keySpan = new TextSpan(spanStart, key.Length);
        var valueSpan = new TextSpan(key.Length + 3, 100);

        return new JsonVariableOptions(key, keySpan, value, valueSpan);
    }
}
