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

        var variableModels = VariableModelsValueProvider.GetValueProvider(ref context, generatorConfiguration);
        var variableModelArray = variableModels.Collect();

        // Create descriptor generator models
        var descriptorGeneratorModelProvider = DescriptorsGeneratorModelValueProvider.GetValueProvider(ref context, variableModelArray);

        // Create typed variable classes
        var descriptorPropertyReferences = descriptorGeneratorModelProvider.Select(static (x, _) => x.DescriptorPropertyReferences);

        var variableClassGeneratorModels = GetVariableClassGeneratorModels(
            generatorConfiguration,
            descriptorPropertyReferences,
            variableModels);

        var variableClassReferences = variableClassGeneratorModels.Select(
            static (x, _) => new VariableClassReference(x.VariableName, x.ClassName, x.ClassNamespace));

        // Get variable context generator models
        var contextClasses = ContextClassInfoValuesProvider.GetValuesProvider(ref context);

        var contextVariableModels = GetContextVariableModelsProvider(
            ref context,
            variableModels,
            descriptorPropertyReferences,
            variableClassReferences);

        var contextClassModels = contextClasses.Combine(contextVariableModels)
            .Select(static (item, ct) =>
            {
                var variables = GetContextIncludedVariables(item.Left.IncludedVariables, item.Right, out var includedVariablesDiagnostics);
                var model = new ContextClassModel(item.Left, variables);

                return new PipelineValueResult<ContextClassModel>(model, includedVariablesDiagnostics);
            });

        // Generate
        context.RegisterSourceOutput(descriptorGeneratorModelProvider, GenerateDescriptorClass);
        context.RegisterSourceOutput(variableClassGeneratorModels, GenerateVariableClass);
        context.RegisterSourceOutput(contextClassModels, GenerateContextClass);
    }

    private static ImmutableArray<ContextVariableModel> GetContextIncludedVariables(
        IncludedVariables includedVariables,
        ImmutableArray<ContextVariableModel> models,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        if (includedVariables.IncludeAll())
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return models;
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
        var builder = ImmutableArray.CreateBuilder<ContextVariableModel>();

        foreach (var variableName in includedVariables.VariableNames)
        {
            var model = models.FirstOrDefault(x => x.VariableModel.VariableInfo.Name.Equals(variableName, StringComparison.Ordinal));

            if (model != default)
            {
                builder.Add(model);
            }
            else
            {
                // TODO: Diagnostic for missing variable
            }
        }

        diagnostics = diagnosticsBuilder.ToImmutable();
        return builder.ToImmutable();
    }

    private static IncrementalValueProvider<ImmutableArray<ContextVariableModel>> GetContextVariableModelsProvider(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<VariableModel> variableModels,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorPropertyReferences,
        IncrementalValuesProvider<VariableClassReference> variableClassReferences)
    {
        var descriptorAndVariableClassRefs = descriptorPropertyReferences.Combine(variableClassReferences.Collect());

        var contextVariableModelsResult = variableModels.Collect()
            .Combine(descriptorAndVariableClassRefs)
            .Select(static (item, ct) =>
            {
                var factory = new ContextVariableModelFactory(item.Right.Right, item.Right.Left);
                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

                foreach (var variableModel in item.Left)
                {
                    factory.TryAdd(variableModel, out var modelDiagnostics);

                    diagnosticsBuilder.AddRange(modelDiagnostics);
                }

                return new PipelineValuesResult<ContextVariableModel>(factory.Build(), diagnosticsBuilder.ToImmutable());
            });

        context.ReportDiagnostics(contextVariableModelsResult.Select(static (x, _) => x.Diagnostics));

        return contextVariableModelsResult.Select(static (x, _) => x.Values);
    }

    private static IncrementalValuesProvider<VariableClassGeneratorModel> GetVariableClassGeneratorModels(
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorPropertyReferences,
        IncrementalValuesProvider<VariableModel> variableModelsProvider)
    {
        var variableClassGeneratorOptions = generatorConfigurationProvider.Combine(descriptorPropertyReferences)
            .Select(static (x, _) =>
                new VariableClassGeneratorOptions(x.Left.GenerateVariableClasses, x.Left.VariableClassesNamespace, x.Right));

        return variableModelsProvider.Collect()
            .Combine(descriptorPropertyReferences.Combine(variableClassGeneratorOptions))
            .SelectMany(static (x, ct) =>
            {
                var descriptorPropertyRefs = x.Right.Left;

                var factory = new VariableClassGeneratorModelFactory(x.Right.Right, x.Left.Length);

                foreach (var model in x.Left)
                {
                    ct.ThrowIfCancellationRequested();

                    factory.Add(model);
                }

                return factory.Build();
            });
    }

    private static void GenerateContextClass(SourceProductionContext context, PipelineValueResult<ContextClassModel> pipelineResult)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (pipelineResult.Diagnostics.HasErrors())
        {
            foreach (var diagnostic in pipelineResult.Diagnostics)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                context.ReportDiagnostic(diagnostic);
            }
        }

        if (!pipelineResult.HasValue)
        {
            return;
        }

        var model = pipelineResult.Value;

        var compilationUnit = ContextClassGenerator.Create(in model, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();
    }

    private static void GenerateDescriptorClass(SourceProductionContext context, DescriptorsGeneratorModel modelProvider)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (modelProvider.Diagnostics.Any())
        {
            foreach (var diagnostic in modelProvider.Diagnostics)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                context.ReportDiagnostic(diagnostic);
            }
        }

        if (modelProvider.GeneratorModel is null)
        {
            return;
        }

        context.CancellationToken.ThrowIfCancellationRequested();

        var generatorModel = modelProvider.GeneratorModel;
        var generator = new DescriptorClassGenerator(generatorModel);

        var descriptorClassCompilationUnit = generator.CreateCompilationUnit(context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = descriptorClassCompilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        context.AddSource($"{generatorModel.TypeName}.g.cs", generatedSourceText);
    }

    private static void GenerateVariableClass(SourceProductionContext context, VariableClassGeneratorModel model)
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

        // TODO: Pull the variable name from the model and append to filename like "VariableKey.g.cs" to avoid potential collisions
        context.AddSource($"Variables/{model.ClassName}.g.cs", generatedSourceText);
    }
}
