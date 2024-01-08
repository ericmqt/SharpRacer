using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using ContextClassInfoResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassTargetResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo Model,
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.IncludedVariablesFileName IncludedVariablesFileName,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassWithIncludedVariablesFileResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo Model,
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.IncludedVariablesFile IncludedVariablesFile,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal static class ContextClassInfoValuesProvider
{
    public static IncrementalValuesProvider<ContextClassInfoResult> GetValuesProvider(
        SyntaxValueProvider syntaxValueProvider,
        IncrementalValuesProvider<AdditionalText> additionalTexts)
    {
        return syntaxValueProvider.ForAttributeWithMetadataName(
                SharpRacerIdentifiers.GenerateDataVariablesContextAttribute.ToQualifiedName(),
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: GetContextClassTarget)
            .WithTrackingName(TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults)
            .Combine(additionalTexts.Collect())
            .Select(static (item, ct) => GetContextClassWithIncludedVariablesFile(item.Left, item.Right, ct))
            .Select(GetContextClassWithIncludedVariables)
            .WithTrackingName(TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider);
    }

    private static ContextClassTargetResult GetContextClassTarget(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDeclarationNode = context.TargetNode as ClassDeclarationSyntax;
        var classDeclarationSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (context.Attributes.Length > 1 ||
            classDeclarationNode is null ||
            classDeclarationSymbol is null ||
            classDeclarationSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            return (default, default, ImmutableArray<Diagnostic>.Empty);
        }

        var attributeData = context.Attributes.First();
        var attributeLocation = attributeData.GetLocation();

        // Build result
        bool isValid = true;
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var classIdentifier = classDeclarationSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var classDeclLocation = classDeclarationNode.Identifier.GetLocation();

        if (!classDeclarationNode.IsPartialClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.ContextClassMustBeDeclaredPartial(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (classDeclarationNode.IsStaticClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.ContextClassMustNotBeDeclaredStatic(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (!classDeclarationSymbol.Interfaces.Any(SharpRacerSymbols.IsIDataVariablesContextInterface))
        {
            diagnosticsBuilder.Add(
                GeneratorDiagnostics.ContextClassMustInheritIDataVariablesContextInterface(classIdentifier, classDeclLocation));

            isValid = false;
        }

        var diagnostics = diagnosticsBuilder.ToImmutableArray();

        if (!isValid)
        {
            return (default, default, diagnostics);
        }

        var includedVariablesFileName = GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(attributeData);
        var classInfo = new ContextClassInfo(classDeclarationSymbol, attributeLocation);

        return (classInfo, includedVariablesFileName, diagnostics);
    }

    private static ContextClassInfoResult GetContextClassWithIncludedVariables(
        ContextClassWithIncludedVariablesFileResult input,
        CancellationToken cancellationToken)
    {
        if (input.IncludedVariablesFile == default)
        {
            return (input.Model, input.Diagnostics);
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        diagnosticsBuilder.AddRange(input.Diagnostics);

        var includedVariableNameValues = input.IncludedVariablesFile.ReadJson(cancellationToken, out var readDiagnostic);

        if (readDiagnostic != null)
        {
            diagnosticsBuilder.Add(readDiagnostic);

            if (readDiagnostic.IsError())
            {
                return (input.Model, diagnosticsBuilder.ToImmutable());
            }
        }

        // Build IncludedVariableName collection
        var factory = new IncludedVariableNameFactory(input.IncludedVariablesFile);

        foreach (var variableNameValue in includedVariableNameValues)
        {
            factory.TryAdd(variableNameValue, out var valueDiagnostics);

            if (valueDiagnostics.Any())
            {
                diagnosticsBuilder.AddRange(valueDiagnostics);
            }
        }

        var includedVariables = new IncludedVariables(factory.Build());

        return (input.Model.WithIncludedVariables(includedVariables), diagnosticsBuilder.ToImmutable());
    }

    private static ContextClassWithIncludedVariablesFileResult GetContextClassWithIncludedVariablesFile(
        ContextClassTargetResult contextTarget,
        ImmutableArray<AdditionalText> additionalTexts,
        CancellationToken cancellationToken)
    {
        if (contextTarget.Model == default)
        {
            return (contextTarget.Model, default, contextTarget.Diagnostics);
        }

        if (contextTarget.IncludedVariablesFileName == default)
        {
            // No include file specified
            return (contextTarget.Model, default, contextTarget.Diagnostics);
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(contextTarget.Diagnostics.Length + 1);

        // Forward previous diagnostics
        diagnosticsBuilder.AddRange(contextTarget.Diagnostics);

        var fileName = contextTarget.IncludedVariablesFileName;
        var matches = additionalTexts.Where(fileName.IsMatch);

        if (!matches.Any())
        {
            diagnosticsBuilder.Add(
                GeneratorDiagnostics.IncludedVariablesFileNotFound(fileName, contextTarget.Model.GeneratorAttributeLocation));

            return (contextTarget.Model, default, diagnosticsBuilder.ToImmutable());
        }

        if (matches.Count() > 1)
        {
            diagnosticsBuilder.Add(
                GeneratorDiagnostics.AmbiguousIncludedVariablesFileName(fileName, contextTarget.Model.GeneratorAttributeLocation));

            return (contextTarget.Model, default, diagnosticsBuilder.ToImmutable());
        }

        var file = matches.Single();
        var sourceText = file.GetText(cancellationToken);

        if (sourceText is null)
        {
            diagnosticsBuilder.Add(
                GeneratorDiagnostics.AdditionalTextContentReadError(file, contextTarget.Model.GeneratorAttributeLocation));

            return (contextTarget.Model, default, diagnosticsBuilder.ToImmutable());
        }

        var includesFile = new IncludedVariablesFile(fileName, file, sourceText);
        return (contextTarget.Model, includesFile, diagnosticsBuilder.ToImmutable());
    }
}
