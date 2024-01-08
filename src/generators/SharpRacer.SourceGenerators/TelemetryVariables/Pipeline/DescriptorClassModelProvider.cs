using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

using DescriptorClassModelResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

using DescriptorClassTargetResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorClassModel Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

using DescriptorPropertiesResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.DescriptorPropertyModel> Properties,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class DescriptorClassModelProvider
{
    public static IncrementalValueProvider<DescriptorClassModelResult> GetValueProvider(
        SyntaxValueProvider syntaxValueProvider,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var descriptorPropertiesResult = variableModelsProvider.Select(GetDescriptorProperties);

        return syntaxValueProvider.ForAttributeWithMetadataName(
                SharpRacerIdentifiers.GenerateDataVariableDescriptorsAttribute.ToQualifiedName(),
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: GetDescriptorClassTarget)
            .Collect()
            .Select(SelectSingleClassModel)
            .Combine(descriptorPropertiesResult)
            .Select<(DescriptorClassModelResult ClassResult, DescriptorPropertiesResult PropertiesResult), DescriptorClassModelResult>(
                static (item, ct) =>
                {
                    var combinedDiagnostics = ImmutableArray.CreateRange(
                        Enumerable.Concat(item.ClassResult.Diagnostics, item.PropertiesResult.Diagnostics));

                    if (item.ClassResult.Model == default)
                    {
                        return (default, combinedDiagnostics);
                    }

                    var model = item.ClassResult.Model.WithDescriptorProperties(item.PropertiesResult.Properties);

                    return (model, combinedDiagnostics);
                })
            .WithTrackingName(TrackingNames.DescriptorClassModelProvider_GetValueProvider);
    }

    private static DescriptorClassTargetResult GetDescriptorClassTarget(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Determine if the target node is a valid descriptor class
        var classDeclarationNode = context.TargetNode as ClassDeclarationSyntax;
        var classDeclarationSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (context.Attributes.Length > 1 ||
            classDeclarationNode is null ||
            classDeclarationSymbol is null ||
            classDeclarationSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            return (default, ImmutableArray<Diagnostic>.Empty);
        }

        var attributeData = context.Attributes.First();
        var attributeLocation = attributeData.GetLocation();

        // Class is a valid target, so build a result
        bool isValid = true;
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var classIdentifier = classDeclarationSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var classDeclLocation = classDeclarationNode.Identifier.GetLocation();

        if (!classDeclarationNode.IsPartialClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.DescriptorClassMustBeDeclaredPartial(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (!classDeclarationNode.IsStaticClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.DescriptorClassMustBeDeclaredStatic(classIdentifier, classDeclLocation));

            isValid = false;
        }

        var diagnostics = diagnosticsBuilder.ToImmutableArray();

        if (!isValid)
        {
            return (default, diagnostics);
        }

        var model = new DescriptorClassModel(
            classDeclarationSymbol.Name,
            classDeclarationSymbol.ContainingNamespace.ToString(),
            attributeLocation);

        return (model, diagnostics);
    }

    private static DescriptorPropertiesResult GetDescriptorProperties(ImmutableArray<VariableModel> variableModels, CancellationToken cancellationToken)
    {
        var factory = new DescriptorPropertyModelFactory();
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        foreach (var model in variableModels)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!factory.TryAdd(model, out var diagnostic))
            {
                diagnosticsBuilder.Add(diagnostic!);
            }
        }

        return (factory.Build(), diagnosticsBuilder.ToImmutable());
    }

    private static DescriptorClassModelResult SelectSingleClassModel(ImmutableArray<DescriptorClassTargetResult> targetResults, CancellationToken cancellationToken)
    {
        if (!targetResults.Any())
        {
            return (default, ImmutableArray<Diagnostic>.Empty);
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        // Forward existing diagnostics
        diagnosticsBuilder.AddRange(targetResults.SelectMany(static x => x.Diagnostics));

        var models = targetResults.Where(static x => x.Model != default)
            .Select(static x => x.Model)
            .OrderBy(static x => $"{x.TypeNamespace}.{x.TypeName}")
            .ToArray();

        if (!models.Any())
        {
            return (default, diagnosticsBuilder.ToImmutable());
        }

        var target = models.First();

        if (models.Length > 1)
        {
            // Enforce an arbitrary order here since we won't know if targets are discovered in the same order from run to run. Now we can
            // skip the first result and generate diagnostics for the rest, ensuring we generate one descriptor class so that the context
            // classes won't regenerate without descriptor property references if they're being used.

            var diagnostics = models
                .Skip(1)
                .Select(x => GeneratorDiagnostics.DescriptorClassAlreadyExistsInAssembly(
                    x.TypeName, target.TypeName, x.GeneratorAttributeLocation));

            diagnosticsBuilder.AddRange(diagnostics);
        }

        return (target, diagnosticsBuilder.ToImmutable());
    }
}
