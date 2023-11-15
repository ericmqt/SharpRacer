using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

public class DataVariableInitializationException : Exception
{
    public DataVariableInitializationException()
    {

    }

    public DataVariableInitializationException(string message)
        : base(message)
    {

    }

    public DataVariableInitializationException(string message, Exception innerException)
        : base(message, innerException)
    {

    }

    public static void ThrowIfDataVariableInfoIsNotScalar(Type dataVariableType, DataVariableInfo dataVariableInfo)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (dataVariableInfo.ValueCount != 1)
        {
            throw new DataVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"{dataVariableType} expects a scalar variable, but an array variable was provided instead."));
        }
    }

    public static void ThrowIfDataVariableInfoNameIsNotEqual(Type dataVariableType, DataVariableInfo dataVariableInfo, string variableName)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (!string.Equals(dataVariableInfo.Name, variableName, StringComparison.Ordinal))
        {
            throw new DataVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Value for property {nameof(DataVariableInfo)}.{nameof(DataVariableInfo.Name)} is invalid (Value: \"{dataVariableInfo.Name}\", expected: \"{variableName}\")."));
        }
    }

    public static void ThrowIfDataVariableInfoValueCountIsNotEqual(Type dataVariableType, DataVariableInfo dataVariableInfo, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (dataVariableInfo.ValueCount != valueCount)
        {
            throw new DataVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Value for property {nameof(DataVariableInfo)}.{nameof(DataVariableInfo.ValueCount)} is invalid (Value: {dataVariableInfo.ValueCount}, expected: {valueCount})."));
        }
    }

    public static void ThrowIfValueTypeArgumentIsInvalid<TValue>(Type dataVariableType, DataVariableValueType variableValueType)
        where TValue : unmanaged
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);

        if (variableValueType == DataVariableValueType.Bitfield && Unsafe.SizeOf<TValue>() != Unsafe.SizeOf<int>())
        {
            throw new DataVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Type argument {typeof(TValue)} is not a valid bitfield variable value type argument (Bitfield variables must use a 32-bit value type)."));
        }

        if (!variableValueType.IsCompatibleValueTypeArgument<TValue>())
        {
            throw new DataVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Type argument {typeof(TValue)} is invalid for variable value type '{variableValueType}'."));
        }
    }

    private static string FormatMessage(Type dataVariableType, string message)
    {
        return $"{dataVariableType} initialization failed: {message}";
    }
}