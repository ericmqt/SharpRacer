namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class BuildPropertyKeys
{
    public static string GenerateVariableClassesProperty { get; } = FormatBuildProperty("GenerateTelemetryVariableClasses");
    public static string TelemetryVariableClassesNamespaceProperty { get; } = FormatBuildProperty("TelemetryVariableClassesNamespace");
    public static string TelemetryVariablesFileNameProperty { get; } = FormatBuildProperty("TelemetryVariablesFileName");
    public static string VariableOptionsFileNameProperty { get; } = FormatBuildProperty("TelemetryVariableOptionsFileName");

    private static string FormatBuildProperty(string propertyName)
    {
        return $"build_property.{propertyName}";
    }
}
