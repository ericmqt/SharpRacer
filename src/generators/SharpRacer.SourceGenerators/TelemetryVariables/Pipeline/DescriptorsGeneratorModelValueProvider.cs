using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class DescriptorsGeneratorModelValueProvider
{
    public static IncrementalValueProvider<DescriptorsGeneratorModel> GetValueProvider(
        SyntaxValueProvider syntaxValueProvider,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var classTargets = syntaxValueProvider.ForClassWithAttribute(
            GenerateDataVariableDescriptorsAttributeInfo.FullTypeName,
            static classDecl => classDecl.HasAttributes() && classDecl.IsStaticPartialClass())
            .WithTrackingName(TrackingNames.DescriptorsGeneratorModelValueProvider_GetTargetClasses);

        return classTargets.Collect()
            .Combine(variableModelsProvider)
            .Select(static (x, ct) => CreateModel(x.Left, x.Right, ct))
            .WithComparer(DescriptorsGeneratorModel.EqualityComparer.Default);
    }

    public static DescriptorsGeneratorModel CreateModel(
        ImmutableArray<ClassWithGeneratorAttribute> targetClasses,
        ImmutableArray<VariableModel> variableModels,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!targetClasses.Any())
        {
            // Nothing to generate, not using descriptors
            return new DescriptorsGeneratorModel();
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

        return new DescriptorsGeneratorModel(generatorModel, descriptorReferences, diagnosticsBuilder.ToImmutable());
    }
}
