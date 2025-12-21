using System.Collections.Immutable;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.IO;

/// <summary>
/// Provides information about a telemetry file (*.IBT).
/// </summary>
public class TelemetryFileInfo : ITelemetryVariableInfoProvider
{
    private readonly ImmutableArray<TelemetryVariableInfo> _dataVariables;
    private readonly DataFileHeader _fileHeader;

    /// <summary>
    /// Initializes a new instance of <see cref="TelemetryFileInfo"/> from the specified file name.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// The file specified by <paramref name="fileName"/> was not found or is inaccessible.
    /// </exception>
    public TelemetryFileInfo(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
        }

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(null, fileName);
        }

        FileInfo = new FileInfo(fileName);

        // Read file information
        using (var fileReader = new TelemetryFileReader(fileName))
        {
            _fileHeader = fileReader.FileHeader;

            SessionInfo = fileReader.ReadSessionInfo();

            var variableHeaders = fileReader.ReadDataVariableHeaders();
            var variableInfoArray = variableHeaders.Select(x => new TelemetryVariableInfo(x)).ToArray();

            _dataVariables = ImmutableArray.Create(variableInfoArray);
        }

        SessionStart = _fileHeader.DiskSubHeader.GetSessionStartDateTimeOffset().ToLocalTime();
        SessionEnd = _fileHeader.DiskSubHeader.GetSessionEndDateTimeOffset().ToLocalTime();
    }

    /// <inheritdoc />
    public IEnumerable<TelemetryVariableInfo> Variables => _dataVariables;

    /// <summary>
    /// Gets a <see cref="FileInfo"/> object representing the telemetry file.
    /// </summary>
    public FileInfo FileInfo { get; }

    /// <summary>
    /// Gets the path to the telemetry file.
    /// </summary>
    public string FileName => FileInfo.FullName;

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value in local time describing the date and time the session ended.
    /// </summary>
    public DateTimeOffset SessionEnd { get; }

    /// <summary>
    /// Gets a YAML-formatted <see cref="string"/> containing information about the session.
    /// </summary>
    public string SessionInfo { get; }

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value in local time describing the date and time the session started.
    /// </summary>
    public DateTimeOffset SessionStart { get; }

    /// <inheritdoc />
    public void NotifyTelemetryVariableActivated(string variableName, Action<TelemetryVariableInfo> callback)
    {
        ArgumentException.ThrowIfNullOrEmpty(variableName);
        ArgumentNullException.ThrowIfNull(callback);

        if (Variables.TryFindByName(variableName, out var variableInfo))
        {
            callback(variableInfo);
        }
    }
}
