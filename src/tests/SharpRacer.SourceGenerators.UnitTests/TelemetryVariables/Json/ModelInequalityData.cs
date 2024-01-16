using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal static class ModelInequalityData
{
    internal static TheoryData<JsonVariableOptions, JsonVariableOptions> JsonVariableOptionsData()
    {
        var optionsValue1 = new JsonVariableOptionsValue("Test1", "TestClass");
        var optionsValue2 = new JsonVariableOptionsValue("Test2", "TestClass");

        var keySpan1 = new TextSpan(200, "Key1".Length);
        var keySpan2 = new TextSpan(400, "Key2".Length);

        var valueSpan1 = new TextSpan(250, 100);
        var valueSpan2 = new TextSpan(450, 100);

        return new TheoryData<JsonVariableOptions, JsonVariableOptions>()
        {
            // Key
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key2", keySpan1, optionsValue1, valueSpan1)
            },

            // Key span
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan2, optionsValue1, valueSpan1)
            },

            // Value
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan1, optionsValue2, valueSpan1)
            },

            // Value span
            {
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan1),
                new JsonVariableOptions("Key1", keySpan1, optionsValue1, valueSpan2)
            },
        };
    }

    internal static TheoryData<JsonVariableOptionsValue, JsonVariableOptionsValue> JsonVariableOptionsValueData()
    {
        return new TheoryData<JsonVariableOptionsValue, JsonVariableOptionsValue>()
        {
            // Name
            {
                new JsonVariableOptionsValue("Test1", "TestClass"),
                new JsonVariableOptionsValue("Test2", "TestClass")
            },

            // Class name
            {
                new JsonVariableOptionsValue("Test", "TestClass1"),
                new JsonVariableOptionsValue("Test", "TestClass2")
            }
        };
    }
}
