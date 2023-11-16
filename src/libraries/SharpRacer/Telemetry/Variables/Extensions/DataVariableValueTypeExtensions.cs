using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry.Variables;

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

    internal static bool IsCompatibleValueTypeArgument<T>(this DataVariableValueType valueType)
        where T : unmanaged
    {
        if (valueType == DataVariableValueType.Bitfield)
        {
            return Unsafe.SizeOf<T>() == Unsafe.SizeOf<int>();
        }

        if (valueType == DataVariableValueType.Bool)
        {
            return typeof(T) == typeof(bool);
        }

        if (valueType == DataVariableValueType.Byte)
        {
            return typeof(T) == typeof(byte);
        }

        if (valueType == DataVariableValueType.Int)
        {
            return typeof(T) == typeof(int);
        }

        if (valueType == DataVariableValueType.Float)
        {
            return typeof(T) == typeof(float);
        }

        if (valueType == DataVariableValueType.Double)
        {
            return typeof(T) == typeof(double);
        }

        return false;
    }
}
