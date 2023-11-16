using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// The exception that is thrown when a data variable object cannot be initialized due to the violation of an invariant of the object
/// being initialized.
/// </summary>
public class DataVariableInitializationException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="DataVariableInitializationException"/> with the specfied message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    public DataVariableInitializationException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariableInitializationException"/> with the specfied message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DataVariableInitializationException(string message, Exception? innerException)
        : base(message, innerException)
    {

    }

    /// <summary>
    /// Throws a <see cref="DataVariableInitializationException"/> if <paramref name="dataVariableInfo"/> is an array variable.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">The <see cref="DataVariableInfo"/> object expected to be a scalar variable.</param>
    /// <exception cref="DataVariableInitializationException"><paramref name="dataVariableInfo"/> is an array variable.</exception>
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

    /// <summary>
    /// Throws a <see cref="DataVariableInitializationException"/> if <paramref name="dataVariableInfo"/> property value
    /// <see cref="DataVariableInfo.Name"/> is not equal to <paramref name="variableName"/>.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">
    /// The <see cref="DataVariableInfo"/> object expected to have a value for property <see cref="DataVariableInfo.Name"/> equal to
    /// <paramref name="variableName"/>.
    /// </param>
    /// <param name="variableName">The expected value of <paramref name="dataVariableInfo"/> property <see cref="DataVariableInfo.Name"/>.</param>
    /// <exception cref="DataVariableInitializationException">
    /// <paramref name="dataVariableInfo"/> property value <see cref="DataVariableInfo.Name"/> is not equal to <paramref name="variableName"/>.
    /// </exception>
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

    /// <summary>
    /// Throws a <see cref="DataVariableInitializationException"/> if <paramref name="dataVariableInfo"/> property value
    /// <see cref="DataVariableInfo.ValueCount"/> is not equal to <paramref name="valueCount"/>.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">
    /// The <see cref="DataVariableInfo"/> object expected to have a value for property <see cref="DataVariableInfo.ValueCount"/> equal to
    /// <paramref name="valueCount"/>.
    /// </param>
    /// <param name="valueCount">The expected value of <paramref name="dataVariableInfo"/> property <see cref="DataVariableInfo.ValueCount"/>.</param>
    /// <exception cref="DataVariableInitializationException">
    /// <paramref name="dataVariableInfo"/> property value <see cref="DataVariableInfo.ValueCount"/> is not equal to <paramref name="valueCount"/>.
    /// </exception>
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

    /// <summary>
    /// Throws a <see cref="DataVariableInitializationException"/> if type parameter <typeparamref name="TValue"/> is not a valid type
    /// argument for the specified <see cref="DataVariableValueType" />.
    /// </summary>
    /// <typeparam name="TValue">The value type of the <see cref="IDataVariable{T}"/> implementation.</typeparam>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="variableValueType">The variable value type.</param>
    /// <exception cref="DataVariableInitializationException">
    /// Type parameter <typeparamref name="TValue"/> is not a valid type argument for the specified <see cref="DataVariableValueType" />.
    /// </exception>
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