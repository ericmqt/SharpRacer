namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a collection of <see cref="DataVariableInfo"/> objects from a telemetry data source.
/// </summary>
public interface IDataVariableInfoProvider
{
    /// <summary>
    /// Gets a collection of <see cref="DataVariableInfo"/> objects representing the telemetry variables exposed by the data source.
    /// </summary>
    IEnumerable<DataVariableInfo> DataVariables { get; }

    /// <summary>
    /// Registers a callback that is executed if and when the specified telemetry variable becomes available.
    /// </summary>
    /// <param name="variableName">The name of the variable.</param>
    /// <param name="callback">The action to execute when the telemetry variable becomes available.</param>
    void NotifyDataVariableActivated(string variableName, Action<DataVariableInfo> callback);
}
