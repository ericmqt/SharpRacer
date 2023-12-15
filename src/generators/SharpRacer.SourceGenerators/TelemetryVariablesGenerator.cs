﻿using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.Syntax;
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

        var typedVariableClassModels = GetTypedVariableGeneratorModels(
            ref context,
            generatorConfiguration,
            descriptorPropertyReferences,
            variableModels);

        // TODO: Pull references to the typename of generated variables for use in context classes

        context.RegisterSourceOutput(typedVariableClassModels, GenerateTypedVariableClass);

        // Get variable context generator models
        var variableContextClassInfo = context.SyntaxProvider.ForClassWithAttribute(
            GenerateDataVariablesContextAttributeInfo.FullTypeName,
            static classDecl => classDecl.HasAttributes() && classDecl.IsPartialClass() && !classDecl.IsStaticClass());

        var variableContextClassTargets = GetVariableContextClassValuesProvider(variableContextClassInfo, ref context);

        // TODO: Combine context includes with variable models, and also combine with DescriptorPropertyReferences

        // Generate
        context.RegisterSourceOutput(descriptorGeneratorModelProvider, GenerateDescriptorClass);
    }

    private static IncrementalValuesProvider<VariableContextClassResult> GetVariableContextClassValuesProvider(
        IncrementalValuesProvider<ClassWithGeneratorAttribute> variableContextClassResults,
        ref IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<(ClassWithGeneratorAttribute, IncludedVariablesFileName)> contextClassesWithIncludedVariableFileNames =
            variableContextClassResults.Select(static (item, _) =>
                (ClassTarget: item,
                IncludedVariablesFileName: GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(item.AttributeData)));

        var contextClassesWithIncludedVariableFiles = IncludedVariablesFileValuesProvider.GetValuesProvider(
            contextClassesWithIncludedVariableFileNames,
            context.AdditionalTextsProvider);

        context.ReportDiagnostics(contextClassesWithIncludedVariableFiles.SelectMany(static (x, _) => x.Diagnostics));

        var contextClassAndIncludedNamesProvider = ContextClassIncludedVariableNamesValuesProvider.GetValuesProvider(
            contextClassesWithIncludedVariableFiles
                .Where(static x => !x.HasErrors)
                .Select(static (x, _) => x.Value));

        context.ReportDiagnostics(ContextClassIncludedVariableNamesValuesProvider.GetDiagnostics(contextClassAndIncludedNamesProvider));

        return IncludedVariablesValuesProvider.GetValuesProvider(contextClassAndIncludedNamesProvider)
            .Select(static (item, _) => new VariableContextClassResult(item.Item1, item.Item2));
    }

    private static IncrementalValuesProvider<TypedVariableClassGeneratorModel> GetTypedVariableGeneratorModels(
        ref IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider,
        IncrementalValueProvider<ImmutableArray<DescriptorPropertyReference>> descriptorPropertyReferences,
        IncrementalValuesProvider<VariableModel> variableModelsProvider)
    {
        var typedVariablesOptions = generatorConfigurationProvider.Combine(descriptorPropertyReferences)
            .Select(static (x, _) =>
                new TypedVariableClassesGeneratorOptions(x.Left.GenerateVariableClasses, x.Left.VariableClassesNamespace, x.Right));

        var classGenModels = variableModelsProvider.Collect()
            .Combine(descriptorPropertyReferences.Combine(typedVariablesOptions))
            .Select(static (x, ct) =>
            {
                var descriptorPropertyRefs = x.Right.Left;
                var classGenOptions = x.Right.Right;

                var factory = new TypedVariableClassGeneratorModelFactory();
                var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

                foreach (var model in x.Left)
                {
                    ct.ThrowIfCancellationRequested();

                    factory.TryAdd(model, classGenOptions, out var modelDiagnostics);

                    diagnosticsBuilder.AddRange(modelDiagnostics);
                }

                return new PipelineValuesResult<TypedVariableClassGeneratorModel>(factory.Build(), diagnosticsBuilder.ToImmutable());
            });

        context.ReportDiagnostics(classGenModels.Select(static (x, _) => x.Diagnostics));

        return classGenModels.SelectMany(static (x, _) => x.Values);
    }

    private static void GenerateDescriptorClass(SourceProductionContext context, DescriptorsGeneratorModel modelProvider)
    {
        if (modelProvider.Diagnostics.Any())
        {
            foreach (var diagnostic in modelProvider.Diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        if (modelProvider.GeneratorModel is null)
        {
            return;
        }

        var generatorModel = modelProvider.GeneratorModel;
        var generator = new DescriptorClassGenerator(generatorModel);

        var descriptorClassCompilationUnit = generator.CreateCompilationUnit(context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = descriptorClassCompilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        context.AddSource($"{generatorModel.TypeName}.g.cs", generatedSourceText);
    }

    private static void GenerateTypedVariableClass(SourceProductionContext context, TypedVariableClassGeneratorModel model)
    {
        var compilationUnit = TypedVariableClassGenerator.Create(model, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        // TODO: Pull the variable name from the model and append to filename like "VariableKey.g.cs" to avoid potential collisions
        context.AddSource($"Variables/{model.ClassName}.g.cs", generatedSourceText);
    }
}
