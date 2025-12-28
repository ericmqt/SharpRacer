using SharpRacer.Interop;
using SharpRacer.Telemetry;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.Testing.IO.Builders;

internal sealed class DataFileVariablesBuilder : IDataFileVariablesBuilder
{
    private readonly TelemetryVariableHeaderCollectionBuilder _variableHeaders;

    internal DataFileVariablesBuilder(List<TelemetryVariableHeader> variableHeaders)
    {
        _variableHeaders = new TelemetryVariableHeaderCollectionBuilder(variableHeaders);
    }

    public IDataFileVariablesBuilder Add(TelemetryVariableDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        return Add(descriptor.Name, descriptor.ValueType, descriptor.ValueCount);
    }

    public IDataFileVariablesBuilder AddArray<T>(
        string name,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "An array variable",
        string? unit = null)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(valueCount, 1);

        _variableHeaders.AddArray<T>(name, valueCount, isTimeSliceArray, description, unit);

        return this;
    }

    public IDataFileVariablesBuilder AddScalar<T>(string name, string? description = null, string? unit = null)
        where T : unmanaged
    {
        _variableHeaders.AddScalar<T>(name, description, unit);

        return this;
    }

    private DataFileVariablesBuilder Add(
        string name,
        TelemetryVariableValueType valueType,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "A scalar variable",
        string? unit = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(valueCount, 0);

        _variableHeaders.Add(name, valueType, valueCount, isTimeSliceArray, description, unit);

        return this;
    }
}
