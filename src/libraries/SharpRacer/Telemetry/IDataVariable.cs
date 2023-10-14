namespace SharpRacer.Telemetry;

/// <summary>
/// Provides a base interface for strongly-typed telemetry variables.
/// </summary>
public interface IDataVariable
{
    /// <summary>
    /// Gets a value indicating if the telemetry variable is exposed by the associated data source.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Gets the variable name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a read-only span of bytes over the telemetry variable data in the specified <see cref="IDataFrame"/>.
    /// </summary>
    /// <param name="dataFrame">A frame of telemetry data.</param>
    /// <returns>A read-only span of bytes over the variable value.</returns>
    ReadOnlySpan<byte> GetDataSpan(IDataFrame dataFrame);

    /// <summary>
    /// Gets a read-only span of bytes over the telemetry variable data in the specified span.
    /// </summary>
    /// <param name="source">A span of bytes representing a frame of telemetry data.</param>
    /// <returns>A read-only span of bytes over the variable value.</returns>
    ReadOnlySpan<byte> GetDataSpan(Span<byte> source);
}
