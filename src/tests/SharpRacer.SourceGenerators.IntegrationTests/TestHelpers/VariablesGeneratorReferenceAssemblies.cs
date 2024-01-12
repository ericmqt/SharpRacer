using Microsoft.CodeAnalysis;
using SharpRacer.Telemetry.Variables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
internal static class VariablesGeneratorReferenceAssemblies
{
    static VariablesGeneratorReferenceAssemblies()
    {
        var sharpRacerAssembly = typeof(GenerateDataVariableDescriptorsAttribute).Assembly;
        SharpRacerLibrary = MetadataReference.CreateFromFile(sharpRacerAssembly.Location);

        All = new List<MetadataReference>(Basic.Reference.Assemblies.Net80.References.All)
        {
            SharpRacerLibrary
        };
    }

    public static IEnumerable<MetadataReference> All { get; }

    public static MetadataReference SharpRacerLibrary { get; }
}