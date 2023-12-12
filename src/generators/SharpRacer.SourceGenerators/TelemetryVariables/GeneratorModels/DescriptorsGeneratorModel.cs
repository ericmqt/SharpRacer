using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DescriptorsGeneratorModel
{
    public DescriptorsGeneratorModel()
    {
        DescriptorPropertyReferences = ImmutableArray<DescriptorPropertyReference>.Empty;
        Diagnostics = ImmutableArray<Diagnostic>.Empty;
    }

    public DescriptorsGeneratorModel(
        DescriptorClassModel? generatorModel,
        ImmutableArray<DescriptorPropertyReference> descriptorPropertyReferences,
        ImmutableArray<Diagnostic> diagnostics)
    {
        GeneratorModel = generatorModel;

        DescriptorPropertyReferences = !descriptorPropertyReferences.IsDefault
            ? descriptorPropertyReferences
            : ImmutableArray<DescriptorPropertyReference>.Empty;

        Diagnostics = !diagnostics.IsDefault ? diagnostics : ImmutableArray<Diagnostic>.Empty;
    }

    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public ImmutableArray<DescriptorPropertyReference> DescriptorPropertyReferences { get; }
    public DescriptorClassModel? GeneratorModel { get; }

    internal class EqualityComparer : IEqualityComparer<DescriptorsGeneratorModel?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<DescriptorsGeneratorModel?> Default { get; } = new EqualityComparer();

        public bool Equals(DescriptorsGeneratorModel? x, DescriptorsGeneratorModel? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return DescriptorClassModel.EqualityComparer.Default.Equals(x.GeneratorModel, y.GeneratorModel) &&
                x.DescriptorPropertyReferences.SequenceEqual(y.DescriptorPropertyReferences) &&
                x.Diagnostics.SequenceEqual(y.Diagnostics);
        }

        public int GetHashCode(DescriptorsGeneratorModel? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.GeneratorModel, DescriptorClassModel.EqualityComparer.Default);

            for (int i = 0; i < obj.DescriptorPropertyReferences.Length; i++)
            {
                hc.Add(obj.DescriptorPropertyReferences[i]);
            }

            hc.AddDiagnosticArray(obj.Diagnostics);

            return hc.ToHashCode();
        }
    }
}
