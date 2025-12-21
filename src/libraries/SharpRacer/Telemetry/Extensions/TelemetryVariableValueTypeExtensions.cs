using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

internal static class TelemetryVariableValueTypeExtensions
{
    public static int GetSize(this TelemetryVariableValueType valueType)
    {
        return valueType switch
        {
            TelemetryVariableValueType.Byte => 1,
            TelemetryVariableValueType.Bool => 1,
            TelemetryVariableValueType.Int => 4,
            TelemetryVariableValueType.Bitfield => 4,
            TelemetryVariableValueType.Float => 4,
            TelemetryVariableValueType.Double => 8,

            _ => throw new InvalidOperationException($"'{nameof(valueType)}' has an invalid value '{valueType}'.")
        };
    }

    internal static bool IsCompatibleValueTypeArgument<T>(this TelemetryVariableValueType valueType)
        where T : unmanaged
    {
        if (valueType == TelemetryVariableValueType.Bitfield)
        {
            return Unsafe.SizeOf<T>() == Unsafe.SizeOf<int>();
        }

        if (valueType == TelemetryVariableValueType.Bool)
        {
            return typeof(T) == typeof(bool);
        }

        if (valueType == TelemetryVariableValueType.Byte)
        {
            return typeof(T) == typeof(byte);
        }

        if (valueType == TelemetryVariableValueType.Int)
        {
            return typeof(T) == typeof(int);
        }

        if (valueType == TelemetryVariableValueType.Float)
        {
            return typeof(T) == typeof(float);
        }

        if (valueType == TelemetryVariableValueType.Double)
        {
            return typeof(T) == typeof(double);
        }

        return false;
    }
}
