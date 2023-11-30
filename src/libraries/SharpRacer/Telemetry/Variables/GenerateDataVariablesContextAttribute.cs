namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Indicates that the decorated class is a source generator target for generating a data variables context.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateDataVariablesContextAttribute : Attribute
{
    /// <summary>
    /// Initializes an instance of <see cref="GenerateDataVariablesContextAttribute"/>.
    /// </summary>
    public GenerateDataVariablesContextAttribute()
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="GenerateDataVariablesContextAttribute"/> with the specified variable names file.
    /// </summary>
    /// <param name="includedVariableNamesFile">
    /// The file name of a JSON file containing an array of strings representing the name of each variable to include in the generated context.
    /// </param>
    public GenerateDataVariablesContextAttribute(string includedVariableNamesFile)
    {
        ArgumentException.ThrowIfNullOrEmpty(includedVariableNamesFile);

        IncludedVariableNamesFile = includedVariableNamesFile;
    }

    /// <summary>
    /// Gets the file name of the telemetry variables JSON file to use as input to the source generator.
    /// </summary>
    public string? IncludedVariableNamesFile { get; }
}