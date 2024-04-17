using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharpRacer.SourceGenerators.Testing.Configuration;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class VariablesGeneratorAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    private static readonly EmptyAnalyzerConfigOptions _emptyOptions = new EmptyAnalyzerConfigOptions();

    public VariablesGeneratorAnalyzerConfigOptionsProvider()
    {
        GlobalOptions = new VariablesGeneratorGlobalOptions();
    }

    private VariablesGeneratorAnalyzerConfigOptionsProvider(VariablesGeneratorGlobalOptions globalOptions)
    {
        GlobalOptions = globalOptions;
    }

    public override VariablesGeneratorGlobalOptions GlobalOptions { get; }

    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return _emptyOptions;
    }

    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        return _emptyOptions;
    }

    public VariablesGeneratorAnalyzerConfigOptionsProvider Mutate(
        Action<VariablesGeneratorGlobalOptionsValues>? configureGlobalOptions = null)
    {
        var globalOptions = GlobalOptions.Mutate(configureGlobalOptions);

        return new VariablesGeneratorAnalyzerConfigOptionsProvider(globalOptions);
    }
}
