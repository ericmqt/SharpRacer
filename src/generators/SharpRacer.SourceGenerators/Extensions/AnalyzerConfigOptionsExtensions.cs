using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpRacer.SourceGenerators;
internal static class AnalyzerConfigOptionsExtensions
{
    public static bool TryGetMSBuildProperty(this AnalyzerConfigOptions source, string propertyName, out string value)
    {
        return source.TryGetValue(FormatBuildProperty(propertyName), out value!);
    }

    public static bool TryGetBool(this AnalyzerConfigOptions analyzerConfigOptions, string key, out bool value)
    {
        value = false;

        if (!analyzerConfigOptions.TryGetValue(key, out var keyValue))
        {
            return false;
        }

        return bool.TryParse(keyValue, out value);
    }

    private static string FormatBuildProperty(string propertyName)
    {
        return $"build_property.{propertyName}";
    }
}
