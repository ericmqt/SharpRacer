using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides extension methods for <see cref="TelemetryVariableInfo"/> collections.
/// </summary>
public static class TelemetryVariableInfoCollectionExtensions
{
    /// <summary>
    /// Returns the first <see cref="TelemetryVariableInfo"/> of the sequence with the variable name specified, or <see langword="null"/> if
    /// no such element is found.
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The case-sensitive name of the variable to find.</param>
    /// <returns>
    /// The first <see cref="TelemetryVariableInfo"/> in the sequence with a value for property <see cref="TelemetryVariableInfo.Name"/> matching
    /// the specified variable name, otherwise <see langword="null"/>.
    /// </returns>
    public static TelemetryVariableInfo? FindByName(this IEnumerable<TelemetryVariableInfo> variables, string variableName)
    {
        return variables.FindByName(variableName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the first <see cref="TelemetryVariableInfo"/> of the sequence with the variable name specified, or <see langword="null"/> if
    /// no such element is found.
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The name of the variable to find.</param>
    /// <param name="variableNameComparison">The <see cref="StringComparison"/> mode to use for comparing variable names.</param>
    /// <returns>
    /// The first <see cref="TelemetryVariableInfo"/> in the sequence with a value for property <see cref="TelemetryVariableInfo.Name"/> matching
    /// the specified variable name according to <paramref name="variableNameComparison"/>, otherwise <see langword="null"/>.
    /// </returns>
    public static TelemetryVariableInfo? FindByName(
        this IEnumerable<TelemetryVariableInfo> variables,
        string variableName,
        StringComparison variableNameComparison)
    {
        return variables.FirstOrDefault(x => x.Name.Equals(variableName, variableNameComparison));
    }

    /// <summary>
    /// Attempts to return the first <see cref="TelemetryVariableInfo"/> of the sequence with the variable name specified, or
    /// <see langword="null"/> if no such element is found. 
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The case-sensitive name of the variable to find.</param>
    /// <param name="variableInfo">
    /// Contains the <see cref="TelemetryVariableInfo"/> matching the specified variable name, otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a <see cref="TelemetryVariableInfo"/> was found matching the specified variable name, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryFindByName(
        this IEnumerable<TelemetryVariableInfo> variables,
        string variableName,
        [NotNullWhen(true)] out TelemetryVariableInfo? variableInfo)
    {
        return variables.TryFindByName(variableName, StringComparison.Ordinal, out variableInfo);
    }

    /// <summary>
    /// Attempts to return the first <see cref="TelemetryVariableInfo"/> of the sequence with the variable name specified, or
    /// <see langword="null"/> if no such element is found. 
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The name of the variable to find.</param>
    /// <param name="variableNameComparison">The <see cref="StringComparison"/> mode to use for comparing variable names.</param>
    /// <param name="variableInfo">
    /// Contains the <see cref="TelemetryVariableInfo"/> matching the specified variable name, otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a <see cref="TelemetryVariableInfo"/> was found matching the specified variable name, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryFindByName(
        this IEnumerable<TelemetryVariableInfo> variables,
        string variableName,
        StringComparison variableNameComparison,
        [NotNullWhen(true)] out TelemetryVariableInfo? variableInfo)
    {
        variableInfo = variables.FirstOrDefault(x => x.Name.Equals(variableName, variableNameComparison));

        return variableInfo != null;
    }
}
