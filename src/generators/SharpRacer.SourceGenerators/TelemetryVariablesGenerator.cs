using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using ContextVariableModelsResult = (
    System.Collections.Immutable.ImmutableArray<SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels.ContextVariableModel> ContextVariables,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);

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

        var variableClassGeneratorModels = GetVariableClassGeneratorModels(
            generatorConfiguration,
            descriptorPropertyReferences,
            variableModels);

        var variableClassReferences = variableClassGeneratorModels.Select(
            static (x, _) => new VariableClassReference(x.VariableName, x.ClassName, x.ClassNamespace));

        // Get variable context generator models
        var contextClassResults = ContextClassInfoValuesProvider.GetValuesProvider(context.SyntaxProvider, context.AdditionalTextsProvider);

        var contextVariableModels = GetContextVariableModelsProvider(variableModels, descriptorPropertyReferences, variableClassReferences);

        context.ReportDiagnostics(contextVariableModels.Select(static (x, _) => x.Diagnostics));

        var contextClassModels = contextClassResults.Combine(contextVariableModels.Select(static (x, _) => x.ContextVariables))
            .Select(static (items, ct) =>
            {
                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
                diagnosticsBuilder.AddRange(items.Left.Diagnostics);

                var classInfo = items.Left.Model;

                if (classInfo == default)
                {
                    return (default(ContextClassModel), diagnosticsBuilder.ToImmutable());
                }

                var variables = GetContextIncludedVariables(classInfo, items.Right, diagnosticsBuilder);
                var model = new ContextClassModel(classInfo, variables);

                return (model, diagnosticsBuilder.ToImmutable());
            });

        // Generate
        context.RegisterSourceOutput(descriptorGeneratorModelProvider, GenerateDescriptorClass);
        context.RegisterSourceOutput(variableClassGeneratorModels, GenerateVariableClass);
        context.RegisterSourceOutput(contextClassModels, GenerateContextClass);
    }

    private static ImmutableArray<ContextVariableModel> GetContextIncludedVariables(
        ContextClassInfo contextClassInfo,
        ImmutableArray<ContextVariableModel> models,
        IList<Diagnostic> diagnosticsBuilder)
    {
        if (contextClassInfo.IncludedVariables.IncludeAll())
        {
            return models;
        }

        var builder = ImmutableArray.CreateBuilder<ContextVariableModel>();

        foreach (var variableName in contextClassInfo.IncludedVariables.VariableNames)
        {
            var model = models.FirstOrDefault(x => x.VariableModel.VariableName.Equals(variableName, StringComparison.Ordinal));

            if (model != default)
            {
                builder.Add(model);
            }
            else
            {
                var contextName = $"{contextClassInfo.ClassNamespace}.{contextClassInfo.ClassName}";

                var diagnostic = GeneratorDiagnostics.ContextClassIncludedVariableNotFound(contextClassInfo.ToFullyQualifiedName(), variableName);

                diagnosticsBuilder.Add(diagnostic);
            }
        }

        return builder.ToImmutable();
    }

    private static ImmutableArray<ContextVariableModel> GetContextIncludedVariables(
        ContextClassInfo contextClassInfo,
        ImmutableArray<ContextVariableModel> models,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        if (contextClassInfo.IncludedVariables.IncludeAll())
        {
            diagnostics = ImmutableArray<Diagnostic>.Empty;
            return models;
        }

        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
        var builder = ImmutableArray.CreateBuilder<ContextVariableModel>();

        foreach (var variableName in contextClassInfo.IncludedVariables.VariableNames)
        {
            var model = models.FirstOrDefault(x => x.VariableModel.VariableName.Equals(variableName, StringComparison.Ordinal));

            if (model != default)
            {
                builder.Add(model);
            }
            else
            {
                var contextName = $"{contextClassInfo.ClassNamespace}.{contextClassInfo.ClassName}";

                var diagnostic = GeneratorDiagnostics.ContextClassIncludedVariableNotFound(contextClassInfo.ToFullyQualifiedName(), variableName);

                diagnosticsBuilder.Add(diagnostic);
            }
        }

        diagnostics = diagnosticsBuilder.ToImmutable();
        return builder.ToImmutable();
    }

    private static IncrementalValueProvider<ContextVariableModelsResult> GetContextVariableModelsProvider(
        IncrementalValuesProvider<VariableModel> variableModels,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorPropertyReferences,
        IncrementalValuesProvider<VariableClassReference> variableClassReferences)
    {
        var descriptorAndVariableClassRefs = descriptorPropertyReferences.Combine(variableClassReferences.Collect());

        return variableModels.Collect()
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

                return (factory.Build(), diagnosticsBuilder.ToImmutable());
            });
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

        context.AddSource($"Variables/{model.ClassName}.g.cs", generatedSourceText);
    }
}
