using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;

namespace SharpRacer.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class TelemetryVariablesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generatorConfiguration = GeneratorConfigurationValueProvider.GetValueProvider(context.AnalyzerConfigOptionsProvider);

        // Variable models
        var variableModelProviderResult = GetVariableModels(ref context, generatorConfiguration);

        context.ReportDiagnostics(variableModelProviderResult.Select(static (x, _) => x.Diagnostics));

        var variableModelArray = variableModelProviderResult.Select(static (x, _) => x.Models);
        var variableModels = variableModelProviderResult.SelectMany(static (x, _) => x.Models);

        // Create descriptor generator models
        var descriptorGeneratorModelProvider = DescriptorClassModelProvider.GetValueProvider(context.SyntaxProvider, variableModelArray);

        // Create references to variable descriptor properties on the descriptor class, if we're generating one
        var descriptorPropertyReferences = descriptorGeneratorModelProvider.Select(static (item, _) =>
        {
            if (item.Model == default)
            {
                return ImmutableArray<DescriptorPropertyReference>.Empty;
            }

            return item.Model.DescriptorProperties.Select(x => new DescriptorPropertyReference(item.Model, x)).ToImmutableArray();
        });

        // Variable classes
        var variableClassGeneratorOptions = generatorConfiguration.Select(
            static (config, _) => new VariableClassGeneratorOptions(config.GenerateVariableClasses, config.VariableClassesNamespace));

        var variableClassGeneratorModels = VariableClassModelValuesProvider.GetValuesProvider(
            variableModels,
            descriptorPropertyReferences,
            variableClassGeneratorOptions);

        var variableClassReferences = variableClassGeneratorModels.Select(
            static (x, _) => new VariableClassReference(x.VariableName, x.ClassName, x.ClassNamespace));

        // Get variable context class models
        var contextClassModels = ContextClassModelValuesProvider.GetValuesProvider(
            ContextClassInfoValuesProvider.GetValuesProvider(context.SyntaxProvider, context.AdditionalTextsProvider),
            variableModels,
            descriptorPropertyReferences,
            variableClassReferences.Collect());

        // Generate
        context.RegisterSourceOutput(descriptorGeneratorModelProvider, GenerateDescriptorClass);
        context.RegisterSourceOutput(variableClassGeneratorModels, GenerateVariableClass);
        context.RegisterSourceOutput(contextClassModels, GenerateContextClass);
    }

    private static IncrementalValueProvider<(ImmutableArray<VariableModel> Models, ImmutableArray<Diagnostic> Diagnostics)> GetVariableModels(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfiguration)
    {
        // VariableInfo
        var variableInfoProviderResult = VariableInfoProvider.GetValueProvider(context.AdditionalTextsProvider, generatorConfiguration);

        context.ReportDiagnostics(variableInfoProviderResult.Select(static (x, _) => x.Diagnostics));

        // VariableOptions
        var variableOptionsProviderResult = VariableOptionsProvider.GetValueProvider(context.AdditionalTextsProvider, generatorConfiguration);

        context.ReportDiagnostics(variableOptionsProviderResult.Select(static (x, _) => x.Diagnostics));

        // Variable models
        return VariableModelsValueProvider.GetValueProvider(
            variableInfoProviderResult.Select(static (x, _) => x.Variables),
            variableOptionsProviderResult.Select(static (x, _) => x.Options));
    }

    private static void GenerateContextClass(SourceProductionContext context, (ContextClassModel Model, ImmutableArray<Diagnostic> Diagnostics) input)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        // Report diagnostics, if any
        foreach (var diagnostic in input.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        var model = input.Model;

        // Return early if we don't have a model to generate
        if (model == default)
        {
            return;
        }

        var compilationUnit = ContextClassGenerator.Create(ref model, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        context.AddSource($"{model.TypeName}.g.cs", generatedSourceText);
    }

    private static void GenerateDescriptorClass(SourceProductionContext context, (DescriptorClassModel Model, ImmutableArray<Diagnostic> Diagnostics) input)
    {
        var generatorModel = input.Model;

        // Report diagnostics, if any
        foreach (var diagnostic in input.Diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }

        // Return early if we don't have a model to generate
        if (generatorModel == default)
        {
            return;
        }

        var compilationUnit = DescriptorClassGenerator.CreateCompilationUnit(ref generatorModel, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        context.AddSource($"{generatorModel.TypeName}.g.cs", generatedSourceText);
    }

    private static void GenerateVariableClass(SourceProductionContext context, VariableClassModel model)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        // Report diagnostics, if any
        foreach (var diagnostic in model.Diagnostics)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            context.ReportDiagnostic(diagnostic);
        }

        // Don't generate code if there are any error diagnostics
        if (model.Diagnostics.HasErrors())
        {
            return;
        }

        context.CancellationToken.ThrowIfCancellationRequested();

        var compilationUnit = VariableClassGenerator.Create(ref model, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        context.AddSource($"Variables/{model.ClassName}.g.cs", generatedSourceText);
    }
}
