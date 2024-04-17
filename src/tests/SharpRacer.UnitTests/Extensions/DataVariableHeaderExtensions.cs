using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer;
internal static class DataVariableHeaderExtensions
{
    public static int GetDataFrameLength(this DataVariableHeader[]? variableHeaders)
    {
        if (variableHeaders is null || variableHeaders.Length == 0)
        {
            return 0;
        }

        return variableHeaders.Sum(x => x.GetDataLength());
    }

    public static int GetDataLength(this in DataVariableHeader variableHeader)
    {
        return ((DataVariableValueType)variableHeader.Type).GetSize() * variableHeader.Count;
    }
}
