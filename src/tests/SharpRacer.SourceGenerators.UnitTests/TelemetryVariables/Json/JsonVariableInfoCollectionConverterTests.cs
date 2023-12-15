using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class JsonVariableInfoCollectionConverterTests
{
    [Fact]
    public void Write_Test()
    {
        var variable1 = new JsonVariableInfo(
            "SessionTime",
            VariableValueType.Double,
            1,
            "Seconds since session start",
            "s",
            false,
            false,
            null);

        var variable2 = new JsonVariableInfo(
            "SteeringWheelTorque_ST",
            VariableValueType.Float,
            6,
            "Output torque on steering shaft at 360 Hz",
            "N*m",
            true,
            false,
            null);

        var outputVariableArray = ImmutableArray.Create(variable1, variable2);
        string json;

        using (var memoryStream = new MemoryStream())
        using (var jsonWriter = new Utf8JsonWriter(memoryStream))
        {

            var converter = new JsonVariableInfoCollectionConverter();
            converter.Write(jsonWriter, outputVariableArray, TelemetryGeneratorSerializationContext.Default.Options);

            jsonWriter.Flush();

            // Read back
            json = Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        var readVariables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayJsonVariableInfo);

        Assert.Equal(2, readVariables.Length);

        // Provide default spans for read variables so equality won't fail
        var readVariable1 = new JsonVariableInfo(readVariables.Single(x => x.Name == variable1.Name), default);
        var readVariable2 = new JsonVariableInfo(readVariables.Single(x => x.Name == variable2.Name), default);

        Assert.Equal(variable1, readVariable1);
        Assert.Equal(variable2, readVariable2);
    }
}