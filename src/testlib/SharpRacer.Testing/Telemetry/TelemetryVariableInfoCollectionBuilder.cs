using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.Testing.Telemetry;

public class TelemetryVariableInfoCollectionBuilder
{
    private int _bufferSize;
    private readonly List<TelemetryVariableInfo> _variables;

    public TelemetryVariableInfoCollectionBuilder()
    {
        _variables = new List<TelemetryVariableInfo>();
    }

    public int BufferSize => _bufferSize;

    public TelemetryVariableInfo Add(TelemetryVariableHeader header)
    {
        if (_variables.Any(x => x.Name.Equals(header.Name.ToString(), StringComparison.Ordinal)))
        {
            throw new ArgumentException($"Variable with name '{header.Name}' already exists in the collection.", nameof(header));
        }

        if (_variables.Any(x => x.Offset == header.Offset))
        {
            throw new ArgumentException($"Variable with offset {header.Offset} already exists in the collection.", nameof(header));
        }

        var variableInfo = new TelemetryVariableInfo(header);

        _bufferSize += (variableInfo.ValueSize * variableInfo.ValueCount);

        return variableInfo;
    }

    public List<TelemetryVariableInfo> Build()
    {
        return _variables.ToList();
    }
}
