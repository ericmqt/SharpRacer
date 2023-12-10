using SharpRacer.SourceGenerators.Configuration;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class MSBuildProperties
{
    public static MSBuildPropertyKey GenerateVariableClassesKey { get; }
        = MSBuildPropertyKey.FromPropertyName("GenerateTelemetryVariableClasses");

    public static MSBuildPropertyKey RootNamespace { get; } = MSBuildPropertyKey.FromPropertyName("RootNamespace");

    public static MSBuildPropertyKey VariableClassesNamespaceKey { get; }
        = MSBuildPropertyKey.FromPropertyName("TelemetryVariableClassesNamespace");

    public static MSBuildPropertyKey VariableInfoFileNameKey { get; }
        = MSBuildPropertyKey.FromPropertyName("TelemetryVariablesFileName");

    public static MSBuildPropertyKey VariableOptionsFileNameKey { get; }
        = MSBuildPropertyKey.FromPropertyName("TelemetryVariableOptionsFileName");
}
