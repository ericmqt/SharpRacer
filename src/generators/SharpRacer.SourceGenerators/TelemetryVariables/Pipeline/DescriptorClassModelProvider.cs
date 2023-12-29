using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class DescriptorClassModelProvider
{
    public static IncrementalValueProvider<PipelineValueResult<DescriptorClassModel>> GetValueProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var classTargets = GetClassesFromSyntaxValueProvider(context.SyntaxProvider);

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

        // TODO: Warn for empty descriptor properties?

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
                .Select(x => DescriptorClassDiagnostics.AssemblyAlreadyContainsDescriptorClassTarget(
                    x.TypeName, target.TypeName, x.GeneratorAttributeLocation));

            diagnosticsBuilder.AddRange(diagnostics);
        }

        return new PipelineValueResult<DescriptorClassModel>(target, diagnosticsBuilder.ToImmutable());
    }

    private static IncrementalValuesProvider<DescriptorClassModel> GetClassesFromSyntaxValueProvider(SyntaxValueProvider syntaxValueProvider)
    {
        return syntaxValueProvider.ForAttributeWithMetadataName(
            GenerateDataVariableDescriptorsAttributeInfo.FullTypeName,
            predicate: (node, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return node is ClassDeclarationSyntax classDecl &&
                    classDecl.HasAttributes() && classDecl.IsStaticPartialClass();
            },
            transform: (context, cancellationToken) =>
            {
                if (context.TryGetClassSymbolWithAttribute(
                        GenerateDataVariablesContextAttributeInfo.FullTypeName,
                        cancellationToken,
                        out INamedTypeSymbol? targetClassSymbol,
                        out AttributeData? attributeData,
                        out Location? attributeLocation))
                {

                    return new DescriptorClassModel(targetClassSymbol!, attributeLocation);
                }

                return default;
            })
            .Where(static item => item != default);
    }
}
