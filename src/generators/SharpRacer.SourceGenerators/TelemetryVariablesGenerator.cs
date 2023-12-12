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

        var variableModels = VariableModelsValueProvider.Get(ref context, generatorConfiguration);
        var variableModelArray = variableModels.Collect();

        // Create descriptor generator models
        var descriptorGeneratorModelProvider = DescriptorsGeneratorModelValueProvider.GetValueProvider(context.SyntaxProvider, variableModelArray);

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
        IncrementalValueProvider<GeneratorConfiguration> generatorConfigurationProvider,
        IncrementalValueProvider<DescriptorsGeneratorModel> descriptorGeneratorProvider,
        IncrementalValuesProvider<VariableModel> variableModelsProvider)
    {
        var typedVariablesOptions = generatorConfigurationProvider.Combine(descriptorGeneratorProvider)
            .Select(static (x, _) => TypedVariableClassesGeneratorOptions.Create(x.Left, x.Right));

        return variableModelsProvider.Combine(typedVariablesOptions)
            .Where(static x => x.Right.IsGeneratorEnabled)
            .Select(static (x, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                if (x.Right.TryGetDescriptorPropertyReference(ref x.Left, out var descriptorPropertyReference))
                {
                    return new TypedVariableClassGeneratorModel(
                        $"{x.Left.Name}Variable",
                        x.Right.TargetNamespace,
                        x.Left,
                        descriptorPropertyReference,
                        isClassInternal: false,
                        isClassPartial: true);
                }

                return new TypedVariableClassGeneratorModel(
                    $"{x.Left.Name}Variable",
                    x.Right.TargetNamespace,
                    x.Left,
                    null,
                    isClassInternal: false,
                    isClassPartial: true);
            })
            .WithComparer(TypedVariableClassGeneratorModel.EqualityComparer.Default);
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
