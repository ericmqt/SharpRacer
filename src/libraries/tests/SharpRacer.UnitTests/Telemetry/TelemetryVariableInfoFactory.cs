using SharpRacer.Interop;

namespace SharpRacer.Telemetry;

internal static class TelemetryVariableInfoFactory
{
    public static TelemetryVariableInfo CreateArray(string variableName, TelemetryVariableValueType valueType, int valueCount)
    {
        return CreateArray(variableName, valueType, valueCount, isTimeSliceArray: false);
    }

    public static TelemetryVariableInfo CreateArray(string variableName, TelemetryVariableValueType valueType, int valueCount, bool isTimeSliceArray)
    {
        return CreateArray(variableName, valueType, valueCount, isTimeSliceArray, 2048);
    }

    public static TelemetryVariableInfo CreateArray(string variableName, TelemetryVariableValueType valueType, int valueCount, bool isTimeSliceArray, int offset)
    {
        var header = CreateArrayHeader(variableName, valueType, valueCount, isTimeSliceArray, offset, "Description", "Unit");

        return new TelemetryVariableInfo(header);
    }

    public static TelemetryVariableInfo CreateScalar(string variableName, TelemetryVariableValueType valueType)
    {
        return CreateScalar(variableName, valueType, 1024);
    }

    public static TelemetryVariableInfo CreateScalar(string variableName, TelemetryVariableValueType valueType, int offset)
    {
        var header = CreateScalarHeader(variableName, valueType, offset, "Description", "Unit");

        return new TelemetryVariableInfo(header);
    }

    private static TelemetryVariableHeader CreateArrayHeader(
        string variableName,
        TelemetryVariableValueType valueType,
        int valueCount,
        bool isTimeSliceArray,
        int offset,
        string description,
        string? unit)
    {
        return new TelemetryVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            valueCount,
            isTimeSliceArray,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }

    private static TelemetryVariableHeader CreateScalarHeader(
        string variableName,
        TelemetryVariableValueType valueType,
        int offset,
        string description,
        string? unit)
    {
        return new TelemetryVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            1,
            false,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }
}
