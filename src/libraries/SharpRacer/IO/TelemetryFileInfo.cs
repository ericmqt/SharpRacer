using System.Collections.Immutable;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.IO;

/// <summary>
/// Provides information about a telemetry file (*.IBT).
/// </summary>
public class TelemetryFileInfo
{
    private readonly DataFileHeader _fileHeader;

    /// <summary>
    /// 
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
        using (var fileStream = FileInfo.OpenRead())
        {
            _fileHeader = TelemetryFileStreamHelpers.ReadHeader(fileStream);

            SessionInfo = TelemetryFileStreamHelpers.ReadSessionInfo(fileStream, _fileHeader);

            var variableHeaders = TelemetryFileStreamHelpers.ReadDataVariableHeaders(fileStream, _fileHeader);
            var variableInfoArray = variableHeaders.Select(x => new DataVariableInfo(x)).ToArray();

            DataVariables = ImmutableArray.Create(variableInfoArray);
        }

        SessionStart = _fileHeader.DiskSubHeader.GetSessionStartDateTimeOffset().ToLocalTime();
        SessionEnd = _fileHeader.DiskSubHeader.GetSessionEndDateTimeOffset().ToLocalTime();
    }

    /// <summary>
    /// Gets an immutable array of <see cref="DataVariableInfo"/> objects that describe the data variables in the telemetry file.
    /// </summary>
    public ImmutableArray<DataVariableInfo> DataVariables { get; }

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
}
