namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Defines a type that provides a set of <see cref="IDataVariable{T}"/> objects initialized from an <see cref="IDataVariableInfoProvider"/>.
/// </summary>
public interface IDataVariablesContext
{
    /// <summary>
    /// Enumerates the set of <see cref="IDataVariable"/> objects represented by this instance.
    /// </summary>
    /// <returns>An enumeration of <see cref="IDataVariable"/> objects.</returns>
    IEnumerable<IDataVariable> EnumerateVariables();
}
