using System.Runtime.CompilerServices;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides information about a telemetry variable which may be used to locate the variable at run-time or serve as a placeholder for a
/// variable that is not available in the current simulator session or telemetry file.
/// </summary>
public class TelemetryVariableDescriptor
{
    /// <summary>
    /// Initializes an instance of <see cref="TelemetryVariableDescriptor"/>.
    /// </summary>
    /// <param name="name">The telemetry variable name.</param>
    /// <param name="valueType">The type of the telemetry variable value.</param>
    /// <param name="valueCount">The number of values represented by the telemetry variable.</param>
    public TelemetryVariableDescriptor(string name, TelemetryVariableValueType valueType, int valueCount)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valueCount);

        Name = name;
        ValueType = valueType;
        ValueCount = valueCount;
    }

    /// <summary>
    /// Gets the name of the telemetry variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the number of values represented by the telemetry variable.
    /// </summary>
    public int ValueCount { get; }

    /// <summary>
    /// Gets the type of value represented by the telemetry variable.
    /// </summary>
    public TelemetryVariableValueType ValueType { get; }

    /// <summary>
    /// Creates a <see cref="TelemetryVariableDescriptor"/> for a telemetry variable that represents an array of values.
    /// </summary>
    /// <typeparam name="T">The element type of the array represented by the telemetry variable.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The number of elements in the array represented by the telemetry variable.</param>
    /// <returns>A <see cref="TelemetryVariableDescriptor"/> that describes the telemetry variable.</returns>
    /// <remarks>
    /// If type argument <typeparamref name="T"/> is an enumeration, the returned <see cref="TelemetryVariableDescriptor" /> will have value
    /// <see cref="TelemetryVariableValueType.Bitfield"/> for property <see cref="ValueType"/>.
    /// </remarks>
    public static TelemetryVariableDescriptor CreateArray<T>(string name, int arrayLength)
        where T : unmanaged
    {
        return CreateArray(name, arrayLength, GetValueType<T>());
    }

    /// <summary>
    /// Creates a <see cref="TelemetryVariableDescriptor"/> for a telemetry variable that represents an array of values.
    /// </summary>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="arrayLength">The number of elements in the array represented by the telemetry variable.</param>
    /// <param name="valueType">
    /// The <see cref="TelemetryVariableValueType"/> that describes the value represented by the telemetry variable.
    /// </param>
    /// <returns>A <see cref="TelemetryVariableDescriptor"/> that describes the telemetry variable.</returns>
    public static TelemetryVariableDescriptor CreateArray(string name, int arrayLength, TelemetryVariableValueType valueType)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(arrayLength, 1);

        return new TelemetryVariableDescriptor(name, valueType, arrayLength);
    }

    /// <summary>
    /// Creates a <see cref="TelemetryVariableDescriptor"/> for a telemetry variable that represents a single value.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the telemetry variable.</typeparam>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <returns>A <see cref="TelemetryVariableDescriptor"/> that describes the telemetry variable.</returns>
    /// <remarks>
    /// If type argument <typeparamref name="T"/> is an enumeration, the returned <see cref="TelemetryVariableDescriptor" /> will have value
    /// <see cref="TelemetryVariableValueType.Bitfield"/> for property <see cref="ValueType"/>.
    /// </remarks>
    public static TelemetryVariableDescriptor CreateScalar<T>(string name)
        where T : unmanaged
    {
        return CreateScalar(name, GetValueType<T>());
    }

    /// <summary>
    /// Creates a <see cref="TelemetryVariableDescriptor"/> for a telemetry variable that represents a single value.
    /// </summary>
    /// <param name="name">The name of the telemetry variable.</param>
    /// <param name="valueType">
    /// The <see cref="TelemetryVariableValueType"/> that describes the value represented by the telemetry variable.
    /// </param>
    /// <returns>A <see cref="TelemetryVariableDescriptor"/> that describes the telemetry variable.</returns>
    public static TelemetryVariableDescriptor CreateScalar(string name, TelemetryVariableValueType valueType)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return new TelemetryVariableDescriptor(name, valueType, valueCount: 1);
    }

    private static TelemetryVariableValueType GetValueType<T>()
        where T : unmanaged
    {
        var typeParam = typeof(T);

        if (typeParam.IsEnum)
        {
            if (Unsafe.SizeOf<T>() != Unsafe.SizeOf<int>())
            {
                throw new ArgumentException(
                    $"Type argument {typeof(T)} is not a valid bitfield variable value type argument (Bitfield variables must use a 32-bit value type).");
            }

            return TelemetryVariableValueType.Bitfield;
        }

        if (typeParam == typeof(byte))
        {
            return TelemetryVariableValueType.Byte;
        }

        if (typeParam == typeof(bool))
        {
            return TelemetryVariableValueType.Bool;
        }

        if (typeParam == typeof(int))
        {
            return TelemetryVariableValueType.Int;
        }

        if (typeParam == typeof(float))
        {
            return TelemetryVariableValueType.Float;
        }

        if (typeParam == typeof(double))
        {
            return TelemetryVariableValueType.Double;
        }

        throw new ArgumentException($"Type argument {typeof(T)} is not a valid variable value type.");
    }
}
