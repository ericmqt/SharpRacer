using System.Runtime.InteropServices;
using SharpRacer.Telemetry;

namespace SharpRacer.Testing.IO;

public readonly ref struct TelemetryBufferSpan
{
    public TelemetryBufferSpan(int index, Span<byte> span)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        // TODO: Check minimum length?

        Index = index;
        Span = span;
    }

    public readonly int Index { get; }
    public readonly Span<byte> Span { get; }

    public readonly void Clear()
    {
        Span.Clear();
    }

    public readonly void SetValue<T>(TelemetryVariableInfo variableInfo, T value)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(variableInfo);

        if (!variableInfo.ValueType.IsCompatibleValueTypeArgument<T>())
        {
            throw new ArgumentException(
                $"Type argument {typeof(T)} is not compatible with '{nameof(variableInfo)}' value type '{variableInfo.ValueType}'.",
                nameof(variableInfo));
        }

        var valueSpan = Span.Slice(variableInfo.Offset, variableInfo.ValueSize * variableInfo.ValueCount);

        MemoryMarshal.Write(valueSpan, in value);
    }
}
