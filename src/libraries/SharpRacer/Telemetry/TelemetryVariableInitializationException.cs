using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// The exception that is thrown when a data variable object cannot be initialized due to the violation of an invariant of the object
/// being initialized.
/// </summary>
public class TelemetryVariableInitializationException : Exception
{
    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableInitializationException"/> with the specfied message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    public TelemetryVariableInitializationException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableInitializationException"/> with the specfied message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TelemetryVariableInitializationException(string message, Exception? innerException)
        : base(message, innerException)
    {

    }

    /// <summary>
    /// Throws a <see cref="TelemetryVariableInitializationException"/> if <paramref name="dataVariableInfo"/> is an array variable.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">The <see cref="TelemetryVariableInfo"/> object expected to be a scalar variable.</param>
    /// <exception cref="TelemetryVariableInitializationException"><paramref name="dataVariableInfo"/> is an array variable.</exception>
    public static void ThrowIfDataVariableInfoIsNotScalar(Type dataVariableType, TelemetryVariableInfo dataVariableInfo)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (dataVariableInfo.ValueCount != 1)
        {
            throw new TelemetryVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"{dataVariableType} expects a scalar variable, but an array variable was provided instead."));
        }
    }

    /// <summary>
    /// Throws a <see cref="TelemetryVariableInitializationException"/> if <paramref name="dataVariableInfo"/> property value
    /// <see cref="TelemetryVariableInfo.Name"/> is not equal to <paramref name="variableName"/>.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">
    /// The <see cref="TelemetryVariableInfo"/> object expected to have a value for property <see cref="TelemetryVariableInfo.Name"/> equal to
    /// <paramref name="variableName"/>.
    /// </param>
    /// <param name="variableName">The expected value of <paramref name="dataVariableInfo"/> property <see cref="TelemetryVariableInfo.Name"/>.</param>
    /// <exception cref="TelemetryVariableInitializationException">
    /// <paramref name="dataVariableInfo"/> property value <see cref="TelemetryVariableInfo.Name"/> is not equal to <paramref name="variableName"/>.
    /// </exception>
    public static void ThrowIfDataVariableInfoNameIsNotEqual(Type dataVariableType, TelemetryVariableInfo dataVariableInfo, string variableName)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (!string.Equals(dataVariableInfo.Name, variableName, StringComparison.Ordinal))
        {
            throw new TelemetryVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Value for property {nameof(TelemetryVariableInfo)}.{nameof(TelemetryVariableInfo.Name)} is invalid (Value: \"{dataVariableInfo.Name}\", expected: \"{variableName}\")."));
        }
    }

    /// <summary>
    /// Throws a <see cref="TelemetryVariableInitializationException"/> if <paramref name="dataVariableInfo"/> property value
    /// <see cref="TelemetryVariableInfo.ValueCount"/> is not equal to <paramref name="valueCount"/>.
    /// </summary>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="dataVariableInfo">
    /// The <see cref="TelemetryVariableInfo"/> object expected to have a value for property <see cref="TelemetryVariableInfo.ValueCount"/> equal to
    /// <paramref name="valueCount"/>.
    /// </param>
    /// <param name="valueCount">The expected value of <paramref name="dataVariableInfo"/> property <see cref="TelemetryVariableInfo.ValueCount"/>.</param>
    /// <exception cref="TelemetryVariableInitializationException">
    /// <paramref name="dataVariableInfo"/> property value <see cref="TelemetryVariableInfo.ValueCount"/> is not equal to <paramref name="valueCount"/>.
    /// </exception>
    public static void ThrowIfDataVariableInfoValueCountIsNotEqual(Type dataVariableType, TelemetryVariableInfo dataVariableInfo, int valueCount)
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);
        ArgumentNullException.ThrowIfNull(dataVariableInfo);

        if (dataVariableInfo.ValueCount != valueCount)
        {
            throw new TelemetryVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Value for property {nameof(TelemetryVariableInfo)}.{nameof(TelemetryVariableInfo.ValueCount)} is invalid (Value: {dataVariableInfo.ValueCount}, expected: {valueCount})."));
        }
    }

    /// <summary>
    /// Throws a <see cref="TelemetryVariableInitializationException"/> if type parameter <typeparamref name="TValue"/> is not a valid type
    /// argument for the specified <see cref="TelemetryVariableValueType" />.
    /// </summary>
    /// <typeparam name="TValue">The value type of the <see cref="ITelemetryVariable{T}"/> implementation.</typeparam>
    /// <param name="dataVariableType">The type of the data variable object being initialized.</param>
    /// <param name="variableValueType">The variable value type.</param>
    /// <exception cref="TelemetryVariableInitializationException">
    /// Type parameter <typeparamref name="TValue"/> is not a valid type argument for the specified <see cref="TelemetryVariableValueType" />.
    /// </exception>
    public static void ThrowIfValueTypeArgumentIsInvalid<TValue>(Type dataVariableType, TelemetryVariableValueType variableValueType)
        where TValue : unmanaged
    {
        ArgumentNullException.ThrowIfNull(dataVariableType);

        if (variableValueType == TelemetryVariableValueType.Bitfield && Unsafe.SizeOf<TValue>() != Unsafe.SizeOf<int>())
        {
            throw new TelemetryVariableInitializationException(
                FormatMessage(
                    dataVariableType,
                    $"Type argument {typeof(TValue)} is not a valid bitfield variable value type argument (Bitfield variables must use a 32-bit value type)."));
        }

        if (!variableValueType.IsCompatibleValueTypeArgument<TValue>())
        {
            throw new TelemetryVariableInitializationException(
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
