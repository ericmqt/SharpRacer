using SharpRacer.Interop;

namespace SharpRacer.Telemetry.TestUtilities;

internal class TelemetryFileVariablesBuilder
{
    private int _nextVariableOffset;
    private readonly List<TelemetryVariableHeader> _variableHeaders;

    public TelemetryFileVariablesBuilder()
    {
        _nextVariableOffset = 0;
        _variableHeaders = new List<TelemetryVariableHeader>();
    }

    public TelemetryFileVariablesBuilder AddArrayVariable(
        string name,
        TelemetryVariableValueType valueType,
        int valueCount,
        string? valueUnit,
        string description,
        out TelemetryVariableHeader header)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(valueCount, 2);

        header = new TelemetryVariableHeader(
            IRSDKString.FromString(name),
            (int)valueType,
            valueCount,
            false,
            _nextVariableOffset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(valueUnit));

        _nextVariableOffset += header.GetDataLength();

        _variableHeaders.Add(header);

        return this;
    }

    public TelemetryFileVariablesBuilder AddScalarVariable(
        string name,
        TelemetryVariableValueType valueType,
        string? valueUnit,
        string description,
        out TelemetryVariableHeader header)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        header = new TelemetryVariableHeader(
            IRSDKString.FromString(name),
            (int)valueType,
            1,
            false,
            _nextVariableOffset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(valueUnit));

        _nextVariableOffset += header.GetDataLength();

        _variableHeaders.Add(header);

        return this;
    }

    public TelemetryVariableHeader[] Build()
    {
        return _variableHeaders.ToArray();
    }
}
