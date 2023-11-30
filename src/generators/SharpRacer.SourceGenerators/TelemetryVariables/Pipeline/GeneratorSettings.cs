using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.Configuration;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class GeneratorSettings
{
    private static readonly string DefaultConfigurationFileName = "TelemetryVariables.config.json";
    private static readonly string DefaultTelemetryVariablesFileName = "TelemetryVariables.json";

    public GeneratorSettings(AnalyzerConfigOptions analyzerConfigOptions)
    {
        if (analyzerConfigOptions is null)
        {
            throw new ArgumentNullException(nameof(analyzerConfigOptions));
        }

        ConfigurationFileName = GetConfigurationFileName(analyzerConfigOptions);
        TelemetryVariablesFileName = GetTelemetryVariablesFileName(analyzerConfigOptions);
    }

    public string ConfigurationFileName { get; }
    public string TelemetryVariablesFileName { get; }

    public bool IsConfigurationFile(AdditionalText text)
    {
        return text.Path.EndsWith(ConfigurationFileName, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsTelemetryVariablesFile(AdditionalText text)
    {
        return text.Path.EndsWith(TelemetryVariablesFileName, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetConfigurationFileName(AnalyzerConfigOptions analyzerConfigOptions)
    {
        if (analyzerConfigOptions.TryGetValue(BuildPropertyKeys.GeneratorConfigurationFileNameProperty, out string? fileName))
        {
            return fileName;
        }

        return DefaultConfigurationFileName;
    }

    public static string GetTelemetryVariablesFileName(AnalyzerConfigOptions analyzerConfigOptions)
    {
        if (analyzerConfigOptions.TryGetValue(BuildPropertyKeys.TelemetryVariablesFileNameProperty, out string? fileName))
        {
            return fileName;
        }

        return DefaultTelemetryVariablesFileName;
    }

    internal class EqualityComparer : IEqualityComparer<GeneratorSettings>
    {
        public static EqualityComparer Default { get; } = new EqualityComparer();

        public bool Equals(GeneratorSettings x, GeneratorSettings y)
        {
            return StringComparer.Ordinal.Equals(x.ConfigurationFileName, y.ConfigurationFileName) &&
                StringComparer.Ordinal.Equals(x.TelemetryVariablesFileName, y.TelemetryVariablesFileName);
        }

        public int GetHashCode(GeneratorSettings obj)
        {
            var hc = new HashCode();

            hc.Add(obj.ConfigurationFileName);
            hc.Add(obj.TelemetryVariablesFileName);

            return hc.ToHashCode();
        }
    }
}
