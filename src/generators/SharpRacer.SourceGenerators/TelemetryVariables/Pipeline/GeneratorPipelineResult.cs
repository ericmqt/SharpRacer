using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class GeneratorPipelineResult
{
    public GeneratorPipelineResult(
        ImmutableArray<DescriptorClassGeneratorInfo> descriptorClassGeneratorTargets,
        ImmutableArray<VariableContextClassGeneratorTarget> variableContextGeneratorTargets,
        ImmutableArray<GeneratorOptionsFile> generatorOptionsFiles,
        ImmutableArray<VariableInfoFile> variableInfoFiles,
        GeneratorSettings generatorSettings)
    {
        DescriptorClassGeneratorTargets = descriptorClassGeneratorTargets;
        VariableContextGeneratorTargets = variableContextGeneratorTargets;
        GeneratorOptionsFiles = generatorOptionsFiles;
        VariableInfoFiles = variableInfoFiles;
        GeneratorSettings = generatorSettings ?? throw new ArgumentNullException(nameof(generatorSettings));
    }

    public ImmutableArray<DescriptorClassGeneratorInfo> DescriptorClassGeneratorTargets { get; }
    public ImmutableArray<VariableContextClassGeneratorTarget> VariableContextGeneratorTargets { get; }
    public ImmutableArray<GeneratorOptionsFile> GeneratorOptionsFiles { get; }
    public ImmutableArray<VariableInfoFile> VariableInfoFiles { get; }
    public GeneratorSettings GeneratorSettings { get; }

    public static IncrementalValueProvider<GeneratorPipelineResult> Get(
        IncrementalValueProvider<((ImmutableArray<GeneratorOptionsFile> GeneratorOptionsFiles, ImmutableArray<VariableInfoFile> VariableInfoFiles), GeneratorSettings GeneratorSettings)> inputFilesProvider,
        IncrementalValueProvider<(ImmutableArray<DescriptorClassGeneratorInfo> DescriptorTargets, ImmutableArray<VariableContextClassGeneratorTarget> VariableContextTargets)> classGeneratorTargetsProvider)
    {
        var combinedProvider = inputFilesProvider.Combine(classGeneratorTargetsProvider);

        return combinedProvider.Select((x, ct) =>
            new GeneratorPipelineResult(
                x.Right.DescriptorTargets,
                x.Right.VariableContextTargets,
                x.Left.Item1.GeneratorOptionsFiles,
                x.Left.Item1.VariableInfoFiles,
                x.Left.GeneratorSettings));
    }

    public bool TryGetDescriptorClassGeneratorTarget(DiagnosticReporter diagnosticReporter, out DescriptorClassGeneratorInfo? target)
    {
        target = null;

        if (!DescriptorClassGeneratorTargets.Any())
        {
            target = null;
            return true;
        }

        if (DescriptorClassGeneratorTargets.Length > 1)
        {
            diagnosticReporter.Report(GeneratorDiagnostics.MoreThanOneDescriptorGeneratorTarget());

            return false;
        }

        target = DescriptorClassGeneratorTargets.Single();

        return true;
    }

    public bool TryGetGeneratorOptionsOrDefault(DiagnosticReporter diagnosticReporter, out GeneratorOptions generatorOptions)
    {
        if (!GeneratorOptionsFiles.Any())
        {
            generatorOptions = new GeneratorOptions();
            return true;
        }

        if (GeneratorOptionsFiles.Length > 1)
        {
            diagnosticReporter.Report(
                    GeneratorOptionsFileDiagnostics.MultipleFilesFound(GeneratorSettings.ConfigurationFileName));

            generatorOptions = null!;
            return false;
        }

        var generatorOptionsFile = GeneratorOptionsFiles.Single();
        generatorOptions = generatorOptionsFile.GetOptionsOrDefault(out var getOptionsDiagnostic);

        if (getOptionsDiagnostic != null)
        {
            diagnosticReporter.Report(getOptionsDiagnostic);

            return false;
        }

        return true;
    }

    public bool TryGetVariableInfoFile(DiagnosticReporter diagnosticReporter, out VariableInfoFile variableInfoFile)
    {
        variableInfoFile = null!;

        if (!VariableInfoFiles.Any())
        {
            diagnosticReporter.Report(VariablesFileDiagnostics.FileNotFound(GeneratorSettings.TelemetryVariablesFileName));

            return false;
        }

        if (VariableInfoFiles.Length > 1)
        {
            diagnosticReporter.Report(VariablesFileDiagnostics.MultipleMatchingFiles(GeneratorSettings.TelemetryVariablesFileName));

            return false;
        }

        variableInfoFile = VariableInfoFiles.Single();

        return true;
    }
}
