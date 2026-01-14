using System.Runtime.CompilerServices;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.Testing.Telemetry;

public sealed class TelemetryVariableHeaderCollectionBuilder
{
    private int _nextOffset = 0;
    private readonly List<TelemetryVariableHeader> _variableHeaders;

    public TelemetryVariableHeaderCollectionBuilder()
        : this(new List<TelemetryVariableHeader>())
    {

    }

    internal TelemetryVariableHeaderCollectionBuilder(List<TelemetryVariableHeader> variableHeaders)
    {
        _variableHeaders = variableHeaders ?? throw new ArgumentNullException(nameof(variableHeaders));
    }

    public int BufferSize => _nextOffset;

    public TelemetryVariableHeader[] Build()
    {
        return _variableHeaders.ToArray();
    }

    public TelemetryVariableHeader Add<T>(
        string name,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "A variable",
        string? unit = null)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(valueCount, 0);

        var valueType = GetValueType<T>();

        return Add(name, valueType, valueCount, isTimeSliceArray, description, unit);
    }

    public TelemetryVariableHeader Add(
        string name,
        TelemetryVariableValueType valueType,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "A variable",
        string? unit = null)
    {

        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(valueCount, 0);

        if (isTimeSliceArray && valueCount == 1)
        {
            throw new ArgumentException(
                $"Value '{nameof(isTimeSliceArray)}' cannot be set to true when {nameof(valueCount)} equals 1.", nameof(isTimeSliceArray));
        }

        var variableHeader = new TelemetryVariableHeader(
            IRSDKString.FromString(name),
            (int)valueType,
            valueCount,
            isTimeSliceArray,
            _nextOffset,
            IRSDKDescString.FromString(description),
            IRSDKString.FromString(unit));

        var variableValueSize = valueType.GetSize() * valueCount;

        _nextOffset += variableValueSize;

        if (_variableHeaders.Any(x => x.Name.Equals(variableHeader.Name.ToString())))
        {
            throw new ArgumentException($"Variable with name '{name}' already exists.");
        }

        _variableHeaders.Add(variableHeader);

        return variableHeader;
    }

    public TelemetryVariableHeader AddArray<T>(
        string name,
        int valueCount,
        bool isTimeSliceArray = false,
        string? description = "An array variable",
        string? unit = null)
        where T : unmanaged
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(valueCount, 1);

        return Add<T>(name, valueCount, isTimeSliceArray, description, unit);
    }

    public TelemetryVariableHeader AddScalar<T>(string name, string? description = null, string? unit = null)
        where T : unmanaged
    {
        return Add<T>(name, 1, false, description, unit);
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
