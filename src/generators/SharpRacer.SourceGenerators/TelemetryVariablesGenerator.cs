using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables;
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
        var generatorConfiguration = context.AnalyzerConfigOptionsProvider
            .Select(static (item, ct) => GeneratorConfiguration.FromAnalyzerConfigOptionsProvider(item));

        var variableModels = VariableModelsValueProvider.Get(ref context, generatorConfiguration);
        var variableModelArray = variableModels.Collect();

        // Create descriptor generator models
        var descriptorGeneratorModelProvider = GetDescriptorClassGeneratorModelProvider(context.SyntaxProvider, variableModelArray);

        // Create typed variable classes
        var typedVariableClassModels = GetTypedVariableGeneratorModels(
            generatorConfiguration,
            descriptorGeneratorModelProvider,
            variableModels);

        // TODO: Pull references to the typename of generated variables for use in context classes

        context.RegisterSourceOutput(typedVariableClassModels, GenerateTypedVariableClass);

        // Get variable context generator models
        var variableContextClassInfo = context.SyntaxProvider.ForClassWithAttribute(
            GenerateDataVariablesContextAttributeInfo.FullTypeName,
            static classDecl => classDecl.HasAttributes() && classDecl.IsPartialClass() && !classDecl.IsStaticClass());

        var includedVariables = GetIncludedVariableFilesArray(variableContextClassInfo, ref context);

        var variableContextClassTargets = variableContextClassInfo.Combine(includedVariables.Collect())
            .Select(static (item, ct) =>
            {
                var includedVariablesFileName = GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(item.Left.AttributeData);

                if (includedVariablesFileName == default)
                {
                    return new PipelineValueResult<VariableContextClassResult>(new VariableContextClassResult(item.Left, default));
                }

                var matchingIncludes = item.Right.Where(x => x.FileName == includedVariablesFileName);

                // If unable to locate the specified includes, return a valid value so the class gets generated with all variables
                if (!matchingIncludes.Any())
                {
                    return new PipelineValueResult<VariableContextClassResult>(
                        new VariableContextClassResult(item.Left, default),
                        IncludedVariablesDiagnostics.FileNotFound(includedVariablesFileName, item.Left.AttributeLocation));
                }

                if (matchingIncludes.Count() > 1)
                {
                    return new PipelineValueResult<VariableContextClassResult>(
                        new VariableContextClassResult(item.Left, default),
                        IncludedVariablesDiagnostics.AmbiguousFileName(includedVariablesFileName, item.Left.AttributeLocation));
                }

                return new PipelineValueResult<VariableContextClassResult>(new VariableContextClassResult(item.Left, matchingIncludes.Single()));
            });

        context.ReportDiagnostics(variableContextClassTargets.SelectMany(static (x, _) => x.Diagnostics));

        // TODO: Combine context includes with variable models, and also combine with DescriptorPropertyReferences

        // Generate
        context.RegisterSourceOutput(descriptorGeneratorModelProvider, GenerateDescriptorClass);
    }

    private static IncrementalValuesProvider<IncludedVariables> GetIncludedVariableFilesArray(
        IncrementalValuesProvider<ClassWithGeneratorAttribute> variableContextClassResults,
        ref IncrementalGeneratorInitializationContext context)
    {
        var includedVariableFileNames = variableContextClassResults
            .Select(static (x, _) => GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(x.AttributeData))
            .Where(static x => !x.IsDefault);

        var includedVariableFilesResult = includedVariableFileNames
            .Combine(context.AdditionalTextsProvider.Collect())
            .Select(static (item, ct) =>
            {
                var file = InputFileFactory.IncludedVariablesFile(item.Left, item.Right, ct, out var diagnostic);

                if (diagnostic != null)
                {
                    return new PipelineValueResult<IncludedVariables>(diagnostic);
                }

                var includedNames = IncludedVariableNameFactory.Create(file, ct, out var factoryDiagnostics);

                if (factoryDiagnostics.HasErrors())
                {
                    return new PipelineValueResult<IncludedVariables>(factoryDiagnostics);
                }

                var includedVariables = new IncludedVariables(item.Left, includedNames);

                return new PipelineValueResult<IncludedVariables>(includedVariables);
            });

        context.ReportDiagnostics(includedVariableFilesResult.SelectMany(static (x, _) => x.Diagnostics));

        return includedVariableFilesResult.Select(static (x, _) => x.Value);
    }

    private static IncrementalValueProvider<DescriptorClassGeneratorProvider> GetDescriptorClassGeneratorModelProvider(
        SyntaxValueProvider syntaxValueProvider,
        IncrementalValueProvider<ImmutableArray<VariableModel>> variableModelsProvider)
    {
        var classTargets = syntaxValueProvider.ForClassWithAttribute(
            GenerateDataVariableDescriptorsAttributeInfo.FullTypeName,
            static classDecl => classDecl.HasAttributes() && classDecl.IsStaticPartialClass());

        return classTargets.Collect()
            .Combine(variableModelsProvider)
            .Select(static (x, ct) => DescriptorClassGeneratorProvider.Create(x.Left, x.Right, ct))
            .WithComparer(DescriptorClassGeneratorProvider.EqualityComparer.Default);
    }

    private static IncrementalValuesProvider<VariableClassGeneratorModel> GetTypedVariableGeneratorModels(
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider,
        IncrementalValueProvider<DescriptorClassGeneratorProvider> descriptorGeneratorProvider,
        IncrementalValuesProvider<VariableModel> variableModelsProvider)
    {
        var typedVariablesOptions = generatorConfigurationProvider.Combine(descriptorGeneratorProvider)
            .Select(static (x, _) => TypedVariableClassesDescriptorOptions.Create(x.Left, x.Right));

        return variableModelsProvider.Combine(typedVariablesOptions)
            .Where(static x => x.Right.IsGeneratorEnabled)
            .Select(static (x, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return VariableClassGeneratorModel.Create(x.Left, x.Right);
            });
    }

    private static void GenerateDescriptorClass(SourceProductionContext context, DescriptorClassGeneratorProvider modelProvider)
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

    private static void GenerateTypedVariableClass(SourceProductionContext context, VariableClassGeneratorModel model)
    {
        var compilationUnit = VariableClassGenerator.CreateTypedVariableClassCompilationUnit(model, context.CancellationToken)
            .NormalizeWhitespace(eol: "\n");

        var generatedSourceText = compilationUnit.GetText(Encoding.UTF8);

        var generatedSourceTextStr = generatedSourceText.ToString();

        // TODO: Pull the variable name from the model and append to filename like "VariableKey.g.cs" to avoid potential collisions
        context.AddSource($"Variables/{model.TypeName}.g.cs", generatedSourceText);
    }
}
