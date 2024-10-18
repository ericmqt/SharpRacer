using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides extension methods for <see cref="DataVariableInfo"/> collections.
/// </summary>
public static class DataVariableInfoCollectionExtensions
{
    /// <summary>
    /// Returns the first <see cref="DataVariableInfo"/> of the sequence with the variable name specified, or <see langword="null"/> if
    /// no such element is found.
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The case-sensitive name of the variable to find.</param>
    /// <returns>
    /// The first <see cref="DataVariableInfo"/> in the sequence with a value for property <see cref="DataVariableInfo.Name"/> matching
    /// the specified variable name, otherwise <see langword="null"/>.
    /// </returns>
    public static DataVariableInfo? FindByName(this IEnumerable<DataVariableInfo> variables, string variableName)
    {
        return variables.FindByName(variableName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns the first <see cref="DataVariableInfo"/> of the sequence with the variable name specified, or <see langword="null"/> if
    /// no such element is found.
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The name of the variable to find.</param>
    /// <param name="variableNameComparison">The <see cref="StringComparison"/> mode to use for comparing variable names.</param>
    /// <returns>
    /// The first <see cref="DataVariableInfo"/> in the sequence with a value for property <see cref="DataVariableInfo.Name"/> matching
    /// the specified variable name according to <paramref name="variableNameComparison"/>, otherwise <see langword="null"/>.
    /// </returns>
    public static DataVariableInfo? FindByName(
        this IEnumerable<DataVariableInfo> variables,
        string variableName,
        StringComparison variableNameComparison)
    {
        return variables.FirstOrDefault(x => x.Name.Equals(variableName, variableNameComparison));
    }

    /// <summary>
    /// Attempts to return the first <see cref="DataVariableInfo"/> of the sequence with the variable name specified, or
    /// <see langword="null"/> if no such element is found. 
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The case-sensitive name of the variable to find.</param>
    /// <param name="variableInfo">
    /// Contains the <see cref="DataVariableInfo"/> matching the specified variable name, otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a <see cref="DataVariableInfo"/> was found matching the specified variable name, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryFindByName(
        this IEnumerable<DataVariableInfo> variables,
        string variableName,
        [NotNullWhen(true)] out DataVariableInfo? variableInfo)
    {
        return variables.TryFindByName(variableName, StringComparison.Ordinal, out variableInfo);
    }

    /// <summary>
    /// Attempts to return the first <see cref="DataVariableInfo"/> of the sequence with the variable name specified, or
    /// <see langword="null"/> if no such element is found. 
    /// </summary>
    /// <param name="variables">The sequence to search.</param>
    /// <param name="variableName">The name of the variable to find.</param>
    /// <param name="variableNameComparison">The <see cref="StringComparison"/> mode to use for comparing variable names.</param>
    /// <param name="variableInfo">
    /// Contains the <see cref="DataVariableInfo"/> matching the specified variable name, otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a <see cref="DataVariableInfo"/> was found matching the specified variable name, otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryFindByName(
        this IEnumerable<DataVariableInfo> variables,
        string variableName,
        StringComparison variableNameComparison,
        [NotNullWhen(true)] out DataVariableInfo? variableInfo)
    {
        variableInfo = variables.FirstOrDefault(x => x.Name.Equals(variableName, variableNameComparison));

        return variableInfo != null;
    }
}
