using SharpRacer.Interop;
using SharpRacer.Telemetry;
using SharpRacer.Testing.IO.Builders;

namespace SharpRacer.Testing.IO;

public class DataFileMemory
{
    private readonly List<TelemetryVariableInfo> _variables;
    private readonly List<TelemetryVariableHeader> _variableHeaders;

    internal DataFileMemory(Memory<byte> memory, List<TelemetryVariableHeader> variableHeaders, int sessionInfoStringMaxLength)
    {
        ArgumentNullException.ThrowIfNull(variableHeaders);
        ArgumentOutOfRangeException.ThrowIfLessThan(sessionInfoStringMaxLength, 1);

        Memory = memory;
        _variableHeaders = variableHeaders;
        SessionInfoMaxLength = sessionInfoStringMaxLength;

        _variables = _variableHeaders.Select(x => new TelemetryVariableInfo(x)).ToList();
    }

    public Memory<byte> Memory { get; }
    public int SessionInfoMaxLength { get; }
    public IEnumerable<TelemetryVariableInfo> TelemetryVariables => _variables;

    public static DataFileMemory Create(Action<DataFileMemoryBuilder> configure)
    {
        var builder = new DataFileMemoryBuilder();

        configure?.Invoke(builder);

        return builder.ToDataFile();
    }

    public void UpdateFileHeader(DataFileHeaderSpanAction writeAction)
    {
        var writer = new DataFileHeaderSpan(Memory.Span);

        writeAction(ref writer);
    }

    public void UpdateSessionInfo(string yamlDocument)
    {
        ArgumentException.ThrowIfNullOrEmpty(yamlDocument);

        ref readonly var header = ref DataFileHeader.AsRef(Memory.Span);

        var sessionInfoBytes = SessionInfoString.Encoding.GetBytes(yamlDocument);
        var sessionInfoVersion = header.SessionInfoVersion + 1;

        UpdateSessionInfo(in header, sessionInfoBytes, sessionInfoVersion);
    }

    public void UpdateSessionInfo(string yamlDocument, int sessionInfoVersion)
    {
        ArgumentException.ThrowIfNullOrEmpty(yamlDocument);
        ArgumentOutOfRangeException.ThrowIfLessThan(sessionInfoVersion, 1);

        ref readonly var header = ref DataFileHeader.AsRef(Memory.Span);

        var sessionInfoBytes = SessionInfoString.Encoding.GetBytes(yamlDocument);

        UpdateSessionInfo(in header, sessionInfoBytes, sessionInfoVersion);
    }

    public void UpdateTelemetryBufferOffset(int index, int offset)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        UpdateFileHeader((ref h) => h.SetTelemetryBufferHeader(index, (ref tbh) => tbh.SetBufferOffset(offset)));
    }

    public void UpdateTelemetryBufferTickCount(int index, int tickCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        UpdateFileHeader((ref h) => h.SetTelemetryBufferHeader(index, (ref tbh) => tbh.SetTickCount(tickCount)));
    }

    public void WriteTelemetryBuffer(int index, TelemetryBufferSpanAction writeAction)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, DataFileConstants.MaxTelemetryBuffers);

        ref readonly var header = ref DataFileHeader.AsRef(Memory.Span);

        var bufferSpan = Memory.Span.Slice(header.TelemetryBufferHeaders[index].BufferOffset, header.TelemetryBufferElementLength);

        var writer = new TelemetryBufferSpan(index, bufferSpan);

        writer.Clear();

        writeAction(ref writer);
    }

    private void UpdateSessionInfo(ref readonly DataFileHeader header, byte[] sessionInfoBytes, int sessionInfoVersion)
    {
        ArgumentNullException.ThrowIfNull(sessionInfoBytes);
        ArgumentOutOfRangeException.ThrowIfLessThan(sessionInfoVersion, 1);

        if (sessionInfoBytes.Length > SessionInfoMaxLength)
        {
            throw new ArgumentException(
                $"The encoded byte array of the session information string has length {sessionInfoBytes.Length}, which exceeds the " +
                $"maximum length {SessionInfoMaxLength}");
        }

        UpdateFileHeader((ref h) =>
        {
            h.SetSessionInfoVersion(sessionInfoVersion);
            h.SetSessionInfoLength(sessionInfoBytes.Length);
        });

        var sessionInfoSpan = Memory.Span.Slice(header.SessionInfoOffset, sessionInfoBytes.Length);

        sessionInfoBytes.CopyTo(sessionInfoSpan);
    }
}
