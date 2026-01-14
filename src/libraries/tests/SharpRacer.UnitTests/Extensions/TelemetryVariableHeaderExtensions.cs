using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer;

internal static class TelemetryVariableHeaderExtensions
{
    public static int GetDataFrameLength(this TelemetryVariableHeader[]? variableHeaders)
    {
        if (variableHeaders is null || variableHeaders.Length == 0)
        {
            return 0;
        }

        return variableHeaders.Sum(x => x.GetDataLength());
    }

    public static int GetDataLength(this in TelemetryVariableHeader variableHeader)
    {
        return ((TelemetryVariableValueType)variableHeader.Type).GetSize() * variableHeader.Count;
    }
}
