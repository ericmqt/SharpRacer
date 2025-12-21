namespace SharpRacer.Telemetry;

/// <summary>
/// Defines a type that provides a set of <see cref="ITelemetryVariable{T}"/> objects initialized from an <see cref="ITelemetryVariableInfoProvider"/>.
/// </summary>
public interface ITelemetryVariablesContext
{
    /// <summary>
    /// Enumerates the set of <see cref="ITelemetryVariable"/> objects provided by this context.
    /// </summary>
    /// <returns>An enumeration of <see cref="ITelemetryVariable"/> objects.</returns>
    IEnumerable<ITelemetryVariable> EnumerateVariables();
}
