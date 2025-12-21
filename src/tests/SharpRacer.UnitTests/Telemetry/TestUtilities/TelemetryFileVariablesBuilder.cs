using SharpRacer.Interop;

namespace SharpRacer.Telemetry.TestUtilities;

internal class TelemetryFileVariablesBuilder
{
    private int _nextVariableOffset;
    private readonly List<DataVariableHeader> _variableHeaders;

    public TelemetryFileVariablesBuilder()
    {
        _nextVariableOffset = 0;
        _variableHeaders = new List<DataVariableHeader>();
    }

    public TelemetryFileVariablesBuilder AddArrayVariable(
        string name,
        TelemetryVariableValueType valueType,
        int valueCount,
        string? valueUnit,
        string description,
        out DataVariableHeader header)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(valueCount, 2);

        header = new DataVariableHeader(
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
        out DataVariableHeader header)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        header = new DataVariableHeader(
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

    public DataVariableHeader[] Build()
    {
        return _variableHeaders.ToArray();
    }
}
