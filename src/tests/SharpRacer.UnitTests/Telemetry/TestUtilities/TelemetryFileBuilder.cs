using SharpRacer.Interop;

namespace SharpRacer.Telemetry.TestUtilities;
internal class TelemetryFileBuilder
{
    private List<Memory<byte>> _dataFrames;
    private readonly int _dataFrameLength;
    private DataFileHeader _fileHeader;
    private string? _sessionInfo;
    private readonly TelemetryVariableHeader[] _variableHeaders;

    private TelemetryFileBuilder(TelemetryVariableHeader[]? variableHeaders)
    {
        _variableHeaders = variableHeaders ?? Array.Empty<TelemetryVariableHeader>();

        _dataFrameLength = _variableHeaders.Sum(x => x.GetDataLength());
        _dataFrames = new List<Memory<byte>>();

        _fileHeader = new DataFileHeader()
            .WithHeaderVersion(DataFileConstants.HeaderVersion)
            .WithStatus(1)
            .WithTickRate(60)
            .WithDataBufferCount(1)
            .WithDataBufferElementLength(_dataFrameLength)
            .WithVariableCount(_variableHeaders.Length);
    }

    public static TelemetryFileBuilder Create()
    {
        return new TelemetryFileBuilder(null);
    }

    public static TelemetryFileBuilder Create(Action<TelemetryFileVariablesBuilder>? configureVariables)
    {
        var variablesBuilder = new TelemetryFileVariablesBuilder();

        configureVariables?.Invoke(variablesBuilder);

        var variableHeaders = variablesBuilder.Build();

        return new TelemetryFileBuilder(variableHeaders);
    }

    public TelemetryFileBuilder AddDataFrame(Action<DataFrameWriter> writeFrameAction)
    {
        ArgumentNullException.ThrowIfNull(writeFrameAction);

        var frame = new Memory<byte>(new byte[_dataFrameLength]);

        var writer = new DataFrameWriter(frame);

        writeFrameAction(writer);

        _dataFrames.Add(frame);

        return this;
    }

    public TelemetryFileBuilder SetHeaderVersion(int headerVersion)
    {
        _fileHeader = _fileHeader.WithHeaderVersion(headerVersion);

        return this;
    }

    public TelemetryFileBuilder SetSessionInfo(string? sessionInfo, int version)
    {
        _sessionInfo = sessionInfo;

        _fileHeader = _fileHeader.WithSessionInfoVersion(version);

        return this;
    }

    public TelemetryFileBuilder SetSessionStartAndDuration(DateTimeOffset sessionStart, TimeSpan duration)
    {
        _fileHeader = _fileHeader.WithDiskSubHeader(_fileHeader.DiskSubHeader.WithSessionStartDateAndDuration(sessionStart, duration));

        return this;
    }

    public void Write(string fileName, out DataFileHeader fileHeader)
    {
        TelemetryFileWriter.Write(
            fileName,
            _fileHeader,
            _variableHeaders,
            _sessionInfo,
            _dataFrames,
            out fileHeader);
    }
}
