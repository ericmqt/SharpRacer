using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class TelemetryVariablesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TODO: Document the generator pipeline
        var descriptorClassTargets = DescriptorClassGeneratorInfo.GetValuesProvider(context.SyntaxProvider)
            //.Select(static (x, _) => new DescriptorClassGeneratorTarget(x))
            // TODO: WithComparer()
            .Collect();

        var variableClassTargets = VariableContextClassGeneratorInfo.GetValuesProvider(context.SyntaxProvider)
            .Combine(context.AdditionalTextsProvider.Collect())
            .Select(static (x, ct) => VariableContextClassGeneratorTarget.Create(x.Left, x.Right, ct))
            // TODO: WithComparer()
            .Collect();

        var generatorSettings = context.AnalyzerConfigOptionsProvider
            .Select(static (x, _) => new GeneratorSettings(x.GlobalOptions))
            .WithComparer(GeneratorSettings.EqualityComparer.Default);

        var generatorSettingsAndTexts = context.AdditionalTextsProvider.Combine(generatorSettings);

        var generatorOptionsFiles = generatorSettingsAndTexts
            .Where(static x => x.Right.IsConfigurationFile(x.Left))
            .Select(static (x, ct) => GeneratorOptionsFile.Create(x.Left, ct))
            .WithComparer(GeneratorOptionsFile.EqualityComparer.Default)
            .Collect();

        var variableFiles = generatorSettingsAndTexts
            .Where(static x => x.Right.IsTelemetryVariablesFile(x.Left))
            .Select(static (x, ct) => VariableInfoFile.Create(x.Left, ct))
            .WithComparer(VariableInfoFile.EqualityComparer.Default)
            .Collect();

        var pipelineResult = GeneratorPipelineResult.Get(
            generatorOptionsFiles.Combine(variableFiles).Combine(generatorSettings),
            descriptorClassTargets.Combine(variableClassTargets));

        context.RegisterSourceOutput(pipelineResult, static (ctx, model) => GenerateSource(ctx, model, ctx.CancellationToken));
    }

    private static void GenerateSource(
        SourceProductionContext context,
        GeneratorPipelineResult pipelineResult,
        CancellationToken cancellationToken = default)
    {
        if (pipelineResult is null)
        {
            throw new ArgumentNullException(nameof(pipelineResult));
        }

        var diagnosticReporter = new DiagnosticReporter(context.ReportDiagnostic);

        // Check if we have any classes to generate
        if (!pipelineResult.DescriptorClassGeneratorTargets.Any() && !pipelineResult.VariableContextGeneratorTargets.Any())
        {
            // TODO: return message diagnostic maybe?
            return;
        }

        if (!TryCreateGeneratorModel(pipelineResult, diagnosticReporter, out var generatorModel))
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Generate descriptor class, if needed
        if (generatorModel.DescriptorClassGeneratorModel != null)
        {
            var generator = new DescriptorClassGenerator(generatorModel.DescriptorClassGeneratorModel);

            var descriptorClassCompilationUnit = generator.CreateCompilationUnit(context.CancellationToken)
                .NormalizeWhitespace(eol: "\n");

            var generatedSourceText = descriptorClassCompilationUnit.GetText(Encoding.UTF8);

            var generatedSourceTextStr = generatedSourceText.ToString();

            context.AddSource($"{generatorModel.DescriptorClassGeneratorModel.TypeName}.g.cs", generatedSourceText);
        }
    }

    private static bool TryCreateGeneratorModel(GeneratorPipelineResult pipelineResult, DiagnosticReporter diagnosticReporter, out GeneratorModel generatorModel)
    {
        generatorModel = null!;

        if (!pipelineResult.TryGetDescriptorClassGeneratorTarget(diagnosticReporter, out var descriptorGeneratorTarget))
        {
            return false;
        }

        // Read generator options or use default value
        if (!pipelineResult.TryGetGeneratorOptionsOrDefault(diagnosticReporter, out var generatorOptions))
        {
            return false;
        }

        if (!TryGetVariableInfoCollection(pipelineResult, diagnosticReporter, out var variableInfoFile, out var variableInfoCollection))
        {
            return false;
        }

        if (!ValidateInputVariables(variableInfoFile, variableInfoCollection, diagnosticReporter))
        {
            return false;
        }

        var variableModels = GetVariableModels(variableInfoCollection, generatorOptions);

        if (!ValidateVariableModels(variableModels, diagnosticReporter))
        {
            return false;
        }

        generatorModel = GeneratorModel.Create(descriptorGeneratorTarget, variableModels);
        return true;
    }

    private static ImmutableArray<VariableModel> GetVariableModels(ImmutableArray<VariableInfo> variableInfos, GeneratorOptions generatorOptions)
    {
        var resultsBuilder = ImmutableArray.CreateBuilder<VariableModel>(variableInfos.Length);

        foreach (var variableInfo in variableInfos)
        {
            if (generatorOptions.VariableOptions.TryGetValue(variableInfo.Name, out var variableOptions))
            {
                resultsBuilder.Add(new VariableModel(variableInfo, variableOptions));
            }
            else
            {
                resultsBuilder.Add(new VariableModel(variableInfo));
            }
        }

        return resultsBuilder.ToImmutableArray();
    }

    private static bool TryGetVariableInfoCollection(
        GeneratorPipelineResult pipelineResult,
        DiagnosticReporter diagnosticReporter,
        out VariableInfoFile variableInfoFile,
        out ImmutableArray<VariableInfo> collection)
    {
        collection = ImmutableArray<VariableInfo>.Empty;

        if (!pipelineResult.TryGetVariableInfoFile(diagnosticReporter, out variableInfoFile))
        {
            return false;
        }

        collection = variableInfoFile.GetVariablesOrDefault(out var readVariablesDiagnostic);

        if (readVariablesDiagnostic != null)
        {
            diagnosticReporter.Report(readVariablesDiagnostic);

            return false;
        }

        // Warn if no variables
        if (collection.Length == 0)
        {
            diagnosticReporter.Report(
                VariablesFileDiagnostics.WarnZeroVariablesInFile(variableInfoFile.Path));
        }

        return true;
    }

    private static bool ValidateInputVariables(VariableInfoFile variablesFile, ImmutableArray<VariableInfo> source, DiagnosticReporter diagnosticReporter)
    {
        if (variablesFile is null)
        {
            throw new ArgumentNullException(nameof(variablesFile));
        }

        var duplicatedNames = source.Select(x => x.Name)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicatedNames.Any())
        {
            foreach (var name in duplicatedNames)
            {
                diagnosticReporter.Report(VariablesFileDiagnostics.DuplicateVariable(name, variablesFile.Path));
            }

            return false;
        }

        return true;
    }

    private static bool ValidateVariableModels(IEnumerable<VariableModel> source, DiagnosticReporter diagnosticReporter)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var duplicatedDescriptorNames = source.Select(x => x.DescriptorName)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicatedDescriptorNames.Any())
        {
            foreach (var descriptorName in duplicatedDescriptorNames)
            {
                // TODO: Report duplicate
            }

            return false;
        }

        var duplicatedContextPropertyNames = source.Select(x => x.ContextPropertyName)
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicatedContextPropertyNames.Any())
        {
            foreach (var propertyName in duplicatedContextPropertyNames)
            {
                // TODO: Report
            }

            return false;
        }

        return true;
    }
}
