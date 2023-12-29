using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class ContextClassInfoValuesProvider
{
    public static IncrementalValuesProvider<ContextClassInfo> GetValuesProvider(
        ref IncrementalGeneratorInitializationContext context)
    {
        var contextClasses = GetContextClassesFromSyntaxValueProvider(context.SyntaxProvider);

        var withIncludedVariablesFileResult = contextClasses.Combine(context.AdditionalTextsProvider.Collect())
            .Select(static (item, ct) => GetIncludedVariablesFile(item.Left, item.Right, ct))
            .WithTrackingName(TrackingNames.ContextClassInfoValuesProvider_WithIncludedVariablesFile);

        context.ReportDiagnostics(withIncludedVariablesFileResult.SelectMany(static (x, _) => x.Diagnostics));

        // Transform each included variables file into array of included variable names
        var contextClassesWithIncludedVariablesResult = GetContextClassesWithIncludedVariables(
            withIncludedVariablesFileResult.Where(static x => !x.Diagnostics.HasErrors()).Select(static (item, _) => item.Value));

        context.ReportDiagnostics(contextClassesWithIncludedVariablesResult.SelectMany(static (x, _) => x.Diagnostics));

        return contextClassesWithIncludedVariablesResult.Select(static (x, _) => x.Value);
    }

    private static PipelineValueResult<(ContextClassInfo ContextClassInfo, IncludedVariablesFile IncludedVariablesFile)> GetIncludedVariablesFile(
        ContextClassInfo contextClassInfo,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (contextClassInfo.IncludedVariablesFileName == default)
        {
            return (contextClassInfo, default);
        }

        var fileName = contextClassInfo.IncludedVariablesFileName;

        if (!additionalTexts.Any())
        {
            return IncludedVariablesDiagnostics.FileNotFound(fileName);
        }

        var matches = additionalTexts.Where(fileName.IsMatch);

        if (matches.Count() > 1)
        {
            return IncludedVariablesDiagnostics.AmbiguousFileName(fileName);
        }

        var file = matches.Single();
        var sourceText = file.GetText(cancellationToken);

        if (sourceText is null)
        {
            return IncludedVariablesDiagnostics.FileContentReadFailure(fileName);
        }

        return (contextClassInfo, new IncludedVariablesFile(fileName, file, sourceText));
    }

    private static IncrementalValuesProvider<PipelineValueResult<ContextClassInfo>> GetContextClassesWithIncludedVariables(
        IncrementalValuesProvider<(ContextClassInfo ContextClassInfo, IncludedVariablesFile IncludedVariablesFile)> source)
    {
        return source.Select(static (item, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (item.IncludedVariablesFile == default)
            {
                return new PipelineValueResult<ContextClassInfo>(item.ContextClassInfo, ImmutableArray<Diagnostic>.Empty);
            }

            var includedVariableNameValues = item.IncludedVariablesFile.ReadJson(cancellationToken, out var readDiagnostic);

            if (readDiagnostic != null && readDiagnostic.IsError())
            {
                return readDiagnostic;
            }

            var factory = new IncludedVariableNameFactory(item.IncludedVariablesFile);
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var variableNameValue in includedVariableNameValues)
            {
                factory.TryAdd(variableNameValue, out var valueDiagnostics);

                // Values with errors won't be emitted into the collection, but warnings still need to be reported out
                if (valueDiagnostics.Any())
                {
                    diagnosticsBuilder.AddRange(valueDiagnostics);
                }
            }

            var includedVariableNames = factory.Build();

            var updatedModel = item.ContextClassInfo.WithIncludedVariables(new IncludedVariables(includedVariableNames));

            return new PipelineValueResult<ContextClassInfo>(updatedModel, diagnosticsBuilder.ToImmutable());
        });
    }

    private static IncrementalValuesProvider<ContextClassInfo> GetContextClassesFromSyntaxValueProvider(SyntaxValueProvider syntaxValueProvider)
    {
        return syntaxValueProvider.ForAttributeWithMetadataName(
            GenerateDataVariablesContextAttributeInfo.FullTypeName,
            predicate: (node, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return node is ClassDeclarationSyntax classDecl &&
                    classDecl.HasAttributes() &&
                    classDecl.IsPartialClass() &&
                    !classDecl.IsStaticClass();
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
                    var includedVariablesFileName = GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(attributeData!);

                    return new ContextClassInfo(targetClassSymbol!, attributeLocation, includedVariablesFileName);
                }

                return default;
            })
            .Where(static item => item != default);
    }
}
