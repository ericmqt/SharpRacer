using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.Testing;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
internal class VariablesGeneratorBuilder
{
    private readonly List<AdditionalTextFile> _additionalTexts;
    private string _assemblyName = "TestAssembly";
    private CSharpCompilationOptions _compilationOptions;
    private Action<VariablesGeneratorGlobalOptionsValues>? _configureGlobalOptions;
    private IncrementalGeneratorOutputKind _disabledOutputs;
    private readonly List<SyntaxTree> _syntaxTrees;

    public VariablesGeneratorBuilder()
    {
        _syntaxTrees = new List<SyntaxTree>();

        _additionalTexts = new List<AdditionalTextFile>();
        _compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        _disabledOutputs = IncrementalGeneratorOutputKind.None;
    }

    public VariablesGeneratorBuilder ConfigureGlobalOptions(Action<VariablesGeneratorGlobalOptionsValues>? configure)
    {
        _configureGlobalOptions = configure;

        return this;
    }

    public VariablesGeneratorBuilder WithAdditionalText(string path, string contents)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
        }

        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentException($"'{nameof(contents)}' cannot be null or empty.", nameof(contents));
        }

        _additionalTexts.Add(new AdditionalTextFile(path, contents));

        return this;
    }

    public VariablesGeneratorBuilder WithAssemblyName(string assemblyName)
    {
        if (string.IsNullOrEmpty(assemblyName))
        {
            throw new ArgumentException($"'{nameof(assemblyName)}' cannot be null or empty.", nameof(assemblyName));
        }

        _assemblyName = assemblyName;

        return this;
    }

    public VariablesGeneratorBuilder WithCompilationOptions(CSharpCompilationOptions compilationOptions)
    {
        _compilationOptions = compilationOptions ?? throw new ArgumentNullException(nameof(compilationOptions));

        return this;
    }

    public VariablesGeneratorBuilder WithDisabledOutputs(IncrementalGeneratorOutputKind output)
    {
        _disabledOutputs = output;

        return this;
    }

    public VariablesGeneratorBuilder WithSyntaxTree(SyntaxTree? syntaxTree)
    {
        if (syntaxTree != null)
        {
            _syntaxTrees.Add(syntaxTree);
        }

        return this;
    }

    public VariablesGeneratorTestModel Build()
    {
        var telemetryVariablesGenerator = new TelemetryVariablesGenerator();
        var telemetryVariablesSourceGenerator = telemetryVariablesGenerator.AsSourceGenerator();

        var optionsProvider = new VariablesGeneratorAnalyzerConfigOptionsProvider()
            .Mutate(options => _configureGlobalOptions?.Invoke(options));

        var compilation = CSharpCompilation.Create(
            assemblyName: _assemblyName,
            syntaxTrees: _syntaxTrees,
            references: VariablesGeneratorReferenceAssemblies.All,
            options: _compilationOptions);

        var generatorDriverOptions = new GeneratorDriverOptions(_disabledOutputs, trackIncrementalGeneratorSteps: true);

        var generatorDriver = CSharpGeneratorDriver.Create(
            generators: [telemetryVariablesSourceGenerator],
            additionalTexts: _additionalTexts,
            optionsProvider: optionsProvider,
            driverOptions: generatorDriverOptions);

        return new VariablesGeneratorTestModel(generatorDriver, compilation, optionsProvider);
    }
}
