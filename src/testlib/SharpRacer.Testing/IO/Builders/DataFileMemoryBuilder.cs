using System.Runtime.InteropServices;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.Testing.IO.Builders;

public sealed class DataFileMemoryBuilder
{
    private const int _DefaultSessionInfoStringMaxLength = 512 * 1024; // 512kb
    private int _sessionInfoStringMaxLength;
    private readonly DataFileVariablesBuilder _variablesBuilder;
    private readonly List<TelemetryVariableHeader> _variableHeaders;

    public DataFileMemoryBuilder()
    {
        _variableHeaders = [];

        _variablesBuilder = new DataFileVariablesBuilder(_variableHeaders);

        _sessionInfoStringMaxLength = _DefaultSessionInfoStringMaxLength;
    }

    public int Size => GetDataFileSize();

    public DataFileMemoryBuilder ConfigureSessionInfoStringMaxLength(int maxLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, 1);

        _sessionInfoStringMaxLength = maxLength;

        return this;
    }

    public DataFileMemoryBuilder ConfigureVariables(IEnumerable<TelemetryVariableHeader> telemetryVariableHeaders)
    {
        _variableHeaders.Clear();

        foreach (var header in telemetryVariableHeaders.OrderBy(x => x.Offset))
        {
            _variableHeaders.Add(header);
        }

        return this;
    }

    public DataFileMemoryBuilder ConfigureVariables(Action<IDataFileVariablesBuilder> configure)
    {
        configure?.Invoke(_variablesBuilder);

        return this;
    }

    public DataFileMemory ToDataFile()
    {
        var fileSize = GetDataFileSize();
        Memory<byte> memory = new byte[fileSize];

        var sessionInfoOffset = DataFileHeader.Size;
        var variableHeaderArrayOffset = sessionInfoOffset + _sessionInfoStringMaxLength;
        var variableHeaderArrayLength = TelemetryVariableHeader.Size * _variableHeaders.Count;

        var telemetryBufferLength = _variableHeaders.Sum(x => ((TelemetryVariableValueType)x.Type).GetSize() * x.Count);
        var telemetryBufferArrayOffset = variableHeaderArrayOffset + variableHeaderArrayLength;

        var telemetryBufferHeaders = new TelemetryBufferHeader[]
        {
            new(tickCount: 0, bufferOffset: telemetryBufferArrayOffset + telemetryBufferLength * 0),
            new(tickCount: 0, bufferOffset: telemetryBufferArrayOffset + telemetryBufferLength * 1),
            new(tickCount: 0, bufferOffset: telemetryBufferArrayOffset + telemetryBufferLength * 2),
            new(tickCount: 0, bufferOffset: telemetryBufferArrayOffset + telemetryBufferLength * 3),
        };

        var fileHeader = new DataFileHeader(
            headerVersion: DataFileConstants.HeaderVersion,
            status: 0,
            tickRate: 60,
            sessionInfoVersion: 0,
            sessionInfoLength: 0,
            sessionInfoOffset: DataFileHeader.Size,
            telemetryVariableCount: _variableHeaders.Count,
            telemetryVariableHeaderOffset: variableHeaderArrayOffset,
            telemetryBufferCount: 3,
            telemetryBufferElementLength: telemetryBufferLength,
            telemetryBufferHeaders: TelemetryBufferHeaderArray.FromArray(telemetryBufferHeaders));

        MemoryMarshal.Write(memory.Span, fileHeader);

        // Write the variable headers
        var variableHeaderArray = _variableHeaders.ToArray();
        var variableHeaderSpan = MemoryMarshal.AsBytes(variableHeaderArray);
        MemoryMarshal.AsBytes(variableHeaderArray).CopyTo(memory.Span.Slice(variableHeaderArrayOffset, variableHeaderArrayLength));

        return new DataFileMemory(memory, _variableHeaders, _sessionInfoStringMaxLength);
    }

    private int GetDataFileSize()
    {
        var telemetryBufferLength = _variableHeaders.Sum(x => ((TelemetryVariableValueType)x.Type).GetSize() * x.Count);
        var telemetryBufferArrayLength = telemetryBufferLength * DataFileConstants.MaxTelemetryBuffers;

        var telemetryVariableHeaderArrayLength = _variableHeaders.Count * TelemetryVariableHeader.Size;

        var sessionInfoOffset = DataFileHeader.Size;
        var telemetryVariableHeaderArrayOffset = sessionInfoOffset + _sessionInfoStringMaxLength;

        return DataFileHeader.Size +
            _sessionInfoStringMaxLength +
            telemetryVariableHeaderArrayLength +
            telemetryBufferArrayLength;
    }
}
