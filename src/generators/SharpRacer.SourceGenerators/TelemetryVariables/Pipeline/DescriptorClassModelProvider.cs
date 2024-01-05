using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class DescriptorClassModelProvider
{
    public static IncrementalValueProvider<PipelineValueResult<DescriptorClassModel>> GetValueProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var classTargetResults = context.SyntaxProvider.ForAttributeWithMetadataName(
            SharpRacerIdentifiers.GenerateDataVariableDescriptorsAttribute.ToQualifiedName(),
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (context, ct) => DescriptorClassModelResult.Create(context, ct));

        context.ReportDiagnostics(classTargetResults.SelectMany(static (x, _) => x.Diagnostics));

        var classTargets = classTargetResults
            .Where(static x => x.IsValid)
            .Select(static (x, _) => x.Model);

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

        var classesWithDescriptorProperties = classTargets
            .Combine(descriptorPropertiesResult.Select(static (x, _) => x.Values))
            .Select(static (item, _) => item.Left.WithDescriptorProperties(item.Right));

        return classesWithDescriptorProperties.Collect()
            .Select(static (item, _) => GetSingleDescriptorClassModel(item));
    }

    private static PipelineValueResult<DescriptorClassModel> GetSingleDescriptorClassModel(ImmutableArray<DescriptorClassModel> models)
    {
        if (!models.Any())
        {
            return new PipelineValueResult<DescriptorClassModel>();
        }

        var target = models.First();

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        if (models.Length > 1)
        {
            // Enforce an arbitrary order here since we won't know if targets are discovered in the same order from run to run. Now we can
            // skip the first result and generate diagnostics for the rest, ensuring we generate one descriptor class so that the context
            // classes won't regenerate without descriptor property references if they're being used.

            var diagnostics = models
                .OrderBy(x => $"{x.TypeNamespace}.{x.TypeName}")
                .Skip(1)
                .Select(x => GeneratorDiagnostics.DescriptorClassAlreadyExistsInAssembly(
                    x.TypeName, target.TypeName, x.GeneratorAttributeLocation));

            diagnosticsBuilder.AddRange(diagnostics);
        }

        return new PipelineValueResult<DescriptorClassModel>(target, diagnosticsBuilder.ToImmutable());
    }
}
