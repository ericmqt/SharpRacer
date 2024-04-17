using SharpRacer.Interop;

namespace SharpRacer.Telemetry;
internal static class DataVariableInfoFactory
{
    public static DataVariableInfo CreateArray(string variableName, DataVariableValueType valueType, int valueCount)
    {
        return CreateArray(variableName, valueType, valueCount, isTimeSliceArray: false);
    }

    public static DataVariableInfo CreateArray(string variableName, DataVariableValueType valueType, int valueCount, bool isTimeSliceArray)
    {
        return CreateArray(variableName, valueType, valueCount, isTimeSliceArray, 2048);
    }

    public static DataVariableInfo CreateArray(string variableName, DataVariableValueType valueType, int valueCount, bool isTimeSliceArray, int offset)
    {
        var header = CreateArrayHeader(variableName, valueType, valueCount, isTimeSliceArray, offset, "Description", "Unit");

        return new DataVariableInfo(header);
    }

    public static DataVariableInfo CreateScalar(string variableName, DataVariableValueType valueType)
    {
        return CreateScalar(variableName, valueType, 1024);
    }

    public static DataVariableInfo CreateScalar(string variableName, DataVariableValueType valueType, int offset)
    {
        var header = CreateScalarHeader(variableName, valueType, offset, "Description", "Unit");

        return new DataVariableInfo(header);
    }

    private static DataVariableHeader CreateArrayHeader(
        string variableName,
        DataVariableValueType valueType,
        int valueCount,
        bool isTimeSliceArray,
        int offset,
        string description,
        string? unit)
    {
        return new DataVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            valueCount,
            isTimeSliceArray,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }

    private static DataVariableHeader CreateScalarHeader(
        string variableName,
        DataVariableValueType valueType,
        int offset,
        string description,
        string? unit)
    {
        return new DataVariableHeader(
            IRSDKString.FromString(variableName),
            (int)valueType,
            1,
            false,
            offset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));
    }
}
