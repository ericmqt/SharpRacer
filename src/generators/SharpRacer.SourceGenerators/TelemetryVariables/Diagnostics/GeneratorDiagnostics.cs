using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
internal static partial class GeneratorDiagnostics
{
    // Variables file: SR1100
    // Configuration file: SR1200

    public static string DefaultCategory = "SharpRacer.SourceGenerators";


    private static readonly DiagnosticDescriptor _MoreThanOneDescriptorGeneratorTarget
        = new DiagnosticDescriptor(
            "SR1010",
            "GenerateDataVariableDescriptorsAttribute can only decorate exactly one class",
            "Only one class in an assembly can be decorated with GenerateDataVariableDescriptorsAttribute",
            DefaultCategory,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);


    public static Diagnostic MoreThanOneDescriptorGeneratorTarget(Location? location = null)
    {
        return Diagnostic.Create(_MoreThanOneDescriptorGeneratorTarget, location);
    }
}
