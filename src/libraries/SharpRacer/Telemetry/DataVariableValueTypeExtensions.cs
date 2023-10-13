namespace SharpRacer.Telemetry;
internal static class DataVariableValueTypeExtensions
{
    public static int GetSize(this DataVariableValueType valueType)
    {
        return valueType switch
        {
            DataVariableValueType.Byte => 1,
            DataVariableValueType.Bool => 1,
            DataVariableValueType.Int => 4,
            DataVariableValueType.Bitfield => 4,
            DataVariableValueType.Float => 4,
            DataVariableValueType.Double => 8,

            _ => throw new InvalidOperationException($"'{nameof(valueType)}' has an invalid value '{valueType}'.")
        };
    }
}
