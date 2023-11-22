namespace SharpRacer.Telemetry.Variables;

/// <summary>
/// Provides a collection of <see cref="DataVariableInfo"/> objects from a telemetry data source.
/// </summary>
public interface IDataVariableInfoProvider
{
    /// <summary>
    /// Gets a collection of <see cref="DataVariableInfo"/> objects representing the telemetry variables exposed by the data source.
    /// </summary>
    /// <returns>
    /// A collection of <see cref="DataVariableInfo"/> objects from the data source.
    /// </returns>
    IEnumerable<DataVariableInfo> GetDataVariables();
}
