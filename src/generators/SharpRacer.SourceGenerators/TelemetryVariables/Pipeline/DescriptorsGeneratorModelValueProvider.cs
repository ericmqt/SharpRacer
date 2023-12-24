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
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var classTargets = context.SyntaxProvider.ForClassWithAttribute(
            GenerateDataVariableDescriptorsAttributeInfo.FullTypeName,
            static classDecl => classDecl.HasAttributes() && classDecl.IsStaticPartialClass())
            .WithTrackingName(TrackingNames.DescriptorsGeneratorModelValueProvider_GetTargetClasses);

        var descriptorPropertiesResult = variableModelsProvider.Select(static (x, ct) =>
        {
            var factory = new DescriptorPropertyModelFactory();
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var model in x)
            {
                ct.ThrowIfCancellationRequested();

                if (!factory.TryAdd(model, out var diagnostic))
                {
                    diagnosticsBuilder.Add(diagnostic!);
                }
            }

            return new PipelineValuesResult<DescriptorPropertyModel>(factory.Build(), diagnosticsBuilder.ToImmutable());
        });

        context.ReportDiagnostics(descriptorPropertiesResult.Select(static (x, _) => x.Diagnostics));

        return classTargets.Collect()
            .Combine(descriptorPropertiesResult.Select(static (x, _) => x.Values))
            .Select(static (x, ct) => CreateModel(x.Left, x.Right, ct))
            .WithComparer(DescriptorsGeneratorModel.EqualityComparer.Default)
            .WithTrackingName(TrackingNames.DescriptorsGeneratorModelValueProvider_GetValueProvider);
    }

    public static DescriptorsGeneratorModel CreateModel(
        ImmutableArray<ClassWithGeneratorAttribute> targetClasses,
        ImmutableArray<DescriptorPropertyModel> descriptorProperties,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!targetClasses.Any())
        {
            // Nothing to generate, not using descriptors
            return new DescriptorsGeneratorModel();
        }

        var target = targetClasses.First();

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (targetClasses.Length > 1)
        {
            // Enforce an arbitrary order here since we won't know if targets are discovered in the same order from run to run. Now we can
            // skip the first result and generate diagnostics for the rest, ensuring we generate one descriptor class so that the context
            // classes won't regenerate without descriptor property references if they're being used.

            var diagnostics = targetClasses
                .OrderBy(x => x.ClassSymbol.ToFullTypeName())
                .Skip(1)
                .Select(x => DescriptorClassDiagnostics.AssemblyAlreadyContainsDescriptorClassTarget(
                    x.ClassSymbol.ToFullTypeName(), target.ClassSymbol.ToFullTypeName(), x.AttributeLocation));

            diagnosticsBuilder.AddRange(diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var generatorModel = new DescriptorClassModel(target, descriptorProperties);

        var descriptorReferences = descriptorProperties.Select(x => new DescriptorPropertyReference(generatorModel, x)).ToImmutableArray();

        return new DescriptorsGeneratorModel(generatorModel, descriptorReferences, diagnosticsBuilder.ToImmutable());
    }
}
