using SharpRacer.Interop;
using SharpRacer.Telemetry.Variables;

namespace SharpRacer.Telemetry.TestUtilities;
internal class TelemetryFileDataVariablesBuilder
{
    private int _nextVariableOffset;
    private readonly List<DataVariableHeader> _variableHeaders;

    public TelemetryFileDataVariablesBuilder()
    {
        _nextVariableOffset = 0;
        _variableHeaders = new List<DataVariableHeader>();
    }

    public TelemetryFileDataVariablesBuilder AddArrayVariable(
        string name,
        DataVariableValueType valueType,
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

    public TelemetryFileDataVariablesBuilder AddScalarVariable(
        string name,
        DataVariableValueType valueType,
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
