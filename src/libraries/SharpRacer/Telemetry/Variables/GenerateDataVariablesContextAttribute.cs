namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Indicates that the decorated class is a source generator target for generating a data variables context.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateDataVariablesContextAttribute : Attribute
{
    /// <summary>
    /// Initializes an instance of <see cref="GenerateDataVariablesContextAttribute"/> with the specified telemetry variables JSON file name.
    /// </summary>
    /// <param name="variablesFileName">The file name of the telemetry variables JSON file to use as input to the source generator.</param>
    public GenerateDataVariablesContextAttribute(string variablesFileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(variablesFileName);

        VariablesFileName = variablesFileName;
    }

    /// <summary>
    /// Gets the file name of a configuration file used to customize source generator output.
    /// </summary>
    public string? ConfigurationFileName { get; set; }

    /// <summary>
    /// Gets the file name of the telemetry variables JSON file to use as input to the source generator.
    /// </summary>
    public string VariablesFileName { get; }
}