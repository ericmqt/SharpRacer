using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Interop;

namespace SharpRacer.IO;
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
            // Read the file header
            var headerBlob = new byte[DataFileHeader.Size];

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.ReadExactly(headerBlob);

            _fileHeader = MemoryMarshal.Read<DataFileHeader>(headerBlob);

            // Read the session info string
            var sessionInfoBlob = new byte[_fileHeader.SessionInfoLength];

            fileStream.Seek(_fileHeader.SessionInfoOffset, SeekOrigin.Begin);
            fileStream.ReadExactly(sessionInfoBlob);

            SessionInfo = Encoding.UTF8.GetString(sessionInfoBlob);
        }
    }

    public FileInfo FileInfo { get; }
    public string FileName => FileInfo.FullName;
    public DateTimeOffset SessionEnd { get; }
    public string SessionInfo { get; }
    public DateTimeOffset SessionStart { get; }
}
