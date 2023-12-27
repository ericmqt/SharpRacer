using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
public class VariableInfoCollectionConverterTests
{
    [Fact]
    public void Write_Test()
    {
        var variable1 = new VariableInfo(
            "SessionTime",
            VariableValueType.Double,
            1,
            "Seconds since session start",
            "s",
            false,
            false,
            null);

        var variable2 = new VariableInfo(
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

            var converter = new VariableInfoCollectionConverter();
            converter.Write(jsonWriter, outputVariableArray, TelemetryGeneratorSerializationContext.Default.Options);

            jsonWriter.Flush();

            // Read back
            json = Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        var readVariables = JsonSerializer.Deserialize(json, TelemetryGeneratorSerializationContext.Default.ImmutableArrayVariableInfo);

        Assert.Equal(2, readVariables.Length);

        // Provide default spans for read variables so equality won't fail
        var readVariable1 = readVariables.Single(x => x.Name == variable1.Name)
            .WithJsonSpan(new TextSpan(0, 0))
            .WithJsonLocation(Location.None);

        var readVariable2 = readVariables.Single(x => x.Name == variable2.Name)
            .WithJsonSpan(new TextSpan(0, 0))
            .WithJsonLocation(Location.None);

        Assert.Equal(variable1, readVariable1);
        Assert.Equal(variable2, readVariable2);
    }
}
