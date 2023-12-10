namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal static class GeneratorConfigurationDefaults
{
    public static bool GenerateTypedVariableClasses { get; } = true;
    public static string TelemetryVariableClassesNamespace { get; } = "SharpRacer.Telemetry.Variables.Generated";
    public static VariableInfoFileName VariableInfoFileName { get; } = new VariableInfoFileName("TelemetryVariables.json");
    public static VariableOptionsFileName VariableOptionsFileName { get; } = new VariableOptionsFileName("TelemetryVariables.config.json");
}
