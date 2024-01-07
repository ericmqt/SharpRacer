using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;

namespace SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
internal class VariablesGeneratorTestModel
{
    public VariablesGeneratorTestModel(
        CSharpGeneratorDriver generatorDriver,
        CSharpCompilation compilation,
        VariablesGeneratorAnalyzerConfigOptionsProvider optionsProvider)
    {
        GeneratorDriver = generatorDriver ?? throw new ArgumentNullException(nameof(generatorDriver));
        Compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
        OptionsProvider = optionsProvider ?? throw new ArgumentNullException(nameof(optionsProvider));
    }

    public CSharpCompilation Compilation { get; }
    public CSharpGeneratorDriver GeneratorDriver { get; }
    public VariablesGeneratorAnalyzerConfigOptionsProvider OptionsProvider { get; }

    public GeneratorRunResult RunGenerator()
    {
        var driver = GeneratorDriver.RunGenerators(Compilation);

        var results = driver.GetRunResult().Results;

        if (results.Length > 1)
        {
            throw new InvalidOperationException("More than one generator executed.");
        }

        if (!results.Any())
        {
            throw new InvalidOperationException("No generators executed.");
        }

        return results.Single();
    }
}
