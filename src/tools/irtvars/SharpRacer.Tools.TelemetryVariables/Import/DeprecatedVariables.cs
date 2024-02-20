using SharpRacer.Tools.TelemetryVariables.Models;

namespace SharpRacer.Tools.TelemetryVariables.Import;
internal static class DeprecatedVariables
{
    private static readonly IReadOnlyDictionary<string, string?> _knownDeprecatedVariables = new Dictionary<string, string?>()
    {
        { "SessionLapsRemain", "SessionLapsRemainEx" },
        { "TrackTemp", "TrackTempCrew" },
        { "ShiftIndicatorPct", null }
    };

    public static bool IsDeprecated(string variableName)
    {
        ArgumentException.ThrowIfNullOrEmpty(variableName);

        return _knownDeprecatedVariables.ContainsKey(variableName);
    }

    public static bool IsDeprecated(DataVariableModel variableModel)
    {
        return variableModel.IsDeprecated || IsDeprecated(variableModel.Name);
    }

    public static string? GetDeprecatingVariableName(DataVariableModel variableModel)
    {
        if (!string.IsNullOrEmpty(variableModel.DeprecatedBy))
        {
            return variableModel.DeprecatedBy;
        }

        _knownDeprecatedVariables.TryGetValue(variableModel.Name, out var deprecatingVariableName);

        return deprecatingVariableName;
    }
}
