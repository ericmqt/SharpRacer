using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class DescriptorClassGeneratorModel
{
    public DescriptorClassGeneratorModel()
    {
        DescriptorPropertyReferences = ImmutableArray<DescriptorPropertyReference>.Empty;
        Diagnostics = ImmutableArray<Diagnostic>.Empty;
    }

    public DescriptorClassGeneratorModel(
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

    public static DescriptorClassGeneratorModel Create(
        ImmutableArray<ClassWithGeneratorAttribute> targetClasses,
        ImmutableArray<VariableModel> variableModels,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!targetClasses.Any())
        {
            // Nothing to generate, not using descriptors
            return new DescriptorClassGeneratorModel();
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (targetClasses.Length > 1)
        {
            // Enforce an arbitrary order here since we won't know if targets are discovered in the same order from run to run. Now we can
            // skip the first result and generate diagnostics for the rest, ensuring we generate one descriptor class so that the context
            // classes won't regenerate without descriptor property references if they're being used.

            var diagnostics = targetClasses
                .OrderBy(x => x.ClassSymbol.ToFullTypeName())
                .Skip(1)
                .Select(x => GeneratorDiagnostics.MoreThanOneDescriptorGeneratorTarget(x.AttributeLocation));

            diagnosticsBuilder.AddRange(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var target = targetClasses.First();
        var descriptorProperties = variableModels.Select(x => new DescriptorPropertyModel(x)).ToImmutableArray();
        var generatorModel = new DescriptorClassModel(target, descriptorProperties);

        var descriptorReferences = variableModels.Select(x => new DescriptorPropertyReference(generatorModel, x)).ToImmutableArray();

        return new DescriptorClassGeneratorModel(generatorModel, descriptorReferences, diagnosticsBuilder.ToImmutable());
    }

    internal class EqualityComparer : IEqualityComparer<DescriptorClassGeneratorModel?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<DescriptorClassGeneratorModel?> Default { get; } = new EqualityComparer();

        public bool Equals(DescriptorClassGeneratorModel? x, DescriptorClassGeneratorModel? y)
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

        public int GetHashCode(DescriptorClassGeneratorModel? obj)
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
