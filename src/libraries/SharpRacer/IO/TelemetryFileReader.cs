using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using SharpRacer.Interop;

namespace SharpRacer.IO;

/// <summary>
/// Reads information from a telemetry file (*.IBT).
/// </summary>
public class TelemetryFileReader : IDisposable
{
    private readonly DataBufferHeader _dataBufferHeader;
    private readonly SafeFileHandle _fileHandle;
    private readonly DataFileHeader _fileHeader;
    private bool _isDisposed;
    private readonly bool _isFileHandleOwner;

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryFileReader"/> from the specified file name.
    /// </summary>
    /// <param name="fileName">The path to the telemetry file to read.</param>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is an empty string.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <exception cref="IOException">The file has an invalid size or format.</exception>
    public TelemetryFileReader(string fileName)
        : this(
              File.OpenHandle(fileName, FileMode.Open, FileAccess.Read, FileShare.Read),
              ownsHandle: true)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryFileReader"/> from the specified file handle.
    /// </summary>
    /// <param name="fileHandle">A <see cref="SafeFileHandle"/> with read access to a telemetry file.</param>
    /// <exception cref="ArgumentException"><paramref name="fileHandle"/> is invalid or closed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileHandle"/> is null.</exception>
    /// <exception cref="IOException">The file has an invalid size or format.</exception>
    public TelemetryFileReader(SafeFileHandle fileHandle)
        : this(fileHandle, ownsHandle: true)
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="TelemetryFileReader"/> from the specified file handle and optionally assumes responsibility
    /// for disposing the file handle.
    /// </summary>
    /// <param name="fileHandle">A <see cref="SafeFileHandle"/> with read access to a telemetry file.</param>
    /// <param name="ownsHandle">If <see langword="true" />, this instance is responsible for disposing <paramref name="fileHandle"/>.</param>
    /// <exception cref="ArgumentException"><paramref name="fileHandle"/> is invalid or closed.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileHandle"/> is null.</exception>
    /// <exception cref="IOException">The file has an invalid size or format.</exception>
    public TelemetryFileReader(SafeFileHandle fileHandle, bool ownsHandle)
    {
        // Validate file handle
        ArgumentNullException.ThrowIfNull(fileHandle);
        _isFileHandleOwner = ownsHandle;

        if (fileHandle.IsClosed)
        {
            throw new ArgumentException($"The specified file handle is closed.");
        }

        if (fileHandle.IsInvalid)
        {
            throw new ArgumentException($"The specified file handle is invalid.");
        }

        // Ensure the file is long enough to read the header
        var fileLength = RandomAccess.GetLength(fileHandle);

        if (fileLength < DataFileHeader.Size)
        {
            if (ownsHandle)
            {
                fileHandle.Dispose();
            }

            throw new IOException(
                $"The specified file has a length ({fileLength}) which is smaller than the size of the file header ({DataFileHeader.Size}).");
        }

        // Read and validate header
        _fileHeader = TelemetryFile.ReadHeader(fileHandle);

        if (!TelemetryFile.ValidateHeader(_fileHeader))
        {
            if (ownsHandle)
            {
                fileHandle.Dispose();
            }

            throw new IOException("Invalid file header.");
        }

        DataFrameCount = _fileHeader.DiskSubHeader.SessionRecordCount;

        // An IBT file only contains one data buffer header, the remainder will be empty
        _dataBufferHeader = _fileHeader.DataBufferHeaders[0];

        // Calculate the minimum size of the file and check against the actual length of the file
        if (!CheckFileSize(fileHandle, _fileHeader, _dataBufferHeader, out fileLength, out var expectedLength))
        {
            if (ownsHandle)
            {
                fileHandle.Dispose();
            }

            throw new IOException(
                $"The specified file has a length ({fileLength}) which is smaller than the expected size based on file header information ({expectedLength}).");
        }

        // Assign file handle
        _fileHandle = fileHandle;
    }

    /// <summary>
    /// Gets the number of data frames contained in the telemetry file.
    /// </summary>
    public int DataFrameCount { get; }

    /// <summary>
    /// Gets the file header.
    /// </summary>
    public DataFileHeader FileHeader => _fileHeader;

    /// <summary>
    /// Reads a data frame at the specified index.
    /// </summary>
    /// <param name="frameIndex">The zero-based index of the data frame.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="frameIndex"/> is less than zero or is greater than the number of frames in the file.</exception>
    /// <exception cref="IOException">The number of bytes read from the file was less than required to complete the operation.</exception>
    /// <exception cref="InvalidOperationException">The file handle is invalid or closed.</exception>
    /// <exception cref="ObjectDisposedException">The reader is disposed or the file is closed.</exception>
    public byte[] ReadDataFrame(int frameIndex)
    {
        VerifyCanRead();

        ArgumentOutOfRangeException.ThrowIfLessThan(frameIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(frameIndex, _fileHeader.DiskSubHeader.SessionRecordCount, nameof(frameIndex));

        var frameBlob = new byte[_fileHeader.DataBufferElementLength];

        var offset = _dataBufferHeader.BufferOffset + frameIndex * _fileHeader.DataBufferElementLength;

        var bytesRead = RandomAccess.Read(_fileHandle, frameBlob, offset);

        VerifyBytesRead(bytesRead, frameBlob.Length);

        return frameBlob;
    }

    /// <summary>
    /// Reads the data frame at the specified index into a span of bytes.
    /// </summary>
    /// <param name="frameIndex">The zero-based index of the data frame.</param>
    /// <param name="buffer">A span of bytes to write the data frame into.</param>
    /// <returns>The number of bytes read from the file.</returns>
    /// <exception cref="ArgumentException"><paramref name="buffer"/> has a length smaller than the length of a data frame.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="frameIndex"/> is less than zero or is greater than the number of frames in the file.</exception>
    /// <exception cref="IOException">The number of bytes read from the file was less than required to complete the operation.</exception>
    /// <exception cref="InvalidOperationException">The file handle is invalid or closed.</exception>
    /// <exception cref="ObjectDisposedException">The reader is disposed or the file is closed.</exception>
    public int ReadDataFrame(int frameIndex, Span<byte> buffer)
    {
        VerifyCanRead();

        ArgumentOutOfRangeException.ThrowIfLessThan(frameIndex, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(frameIndex, DataFrameCount, nameof(frameIndex));

        if (buffer.Length < _fileHeader.DataBufferElementLength)
        {
            throw new ArgumentException(
                $"The specified buffer has a length ({buffer.Length}) less than the length of a data frame ({_fileHeader.DataBufferElementLength}).");
        }

        var offset = _dataBufferHeader.BufferOffset + frameIndex * _fileHeader.DataBufferElementLength;
        var bytesRead = RandomAccess.Read(_fileHandle, buffer, offset);

        VerifyBytesRead(bytesRead, _fileHeader.DataBufferElementLength);

        return bytesRead;
    }

    /// <summary>
    /// Reads the <see cref="DataVariableHeader"/> array.
    /// </summary>
    /// <returns>An array of <see cref="DataVariableHeader"/> values describing the telemetry variables in the file.</returns>
    /// <exception cref="IOException">The number of bytes read from the file was less than required to complete the operation.</exception>
    /// <exception cref="InvalidOperationException">The file handle is invalid or closed.</exception>
    /// <exception cref="ObjectDisposedException">The reader is disposed or the file is closed.</exception>
    public DataVariableHeader[] ReadDataVariableHeaders()
    {
        VerifyCanRead();

        var variableHeaders = new DataVariableHeader[_fileHeader.VariableCount];

        var bytesRead = RandomAccess.Read(
            _fileHandle,
            MemoryMarshal.AsBytes<DataVariableHeader>(variableHeaders),
            _fileHeader.VariableHeaderOffset);

        VerifyBytesRead(bytesRead, DataVariableHeader.Size * _fileHeader.VariableCount);

        return variableHeaders;
    }

    /// <summary>
    /// Reads the session information string.
    /// </summary>
    /// <returns>The session information string.</returns>
    /// <exception cref="IOException">The number of bytes read from the file was less than required to complete the operation.</exception>
    /// <exception cref="InvalidOperationException">The file handle is invalid or closed.</exception>
    /// <exception cref="ObjectDisposedException">The reader is disposed or the file is closed.</exception>
    public string ReadSessionInfo()
    {
        VerifyCanRead();

        Span<byte> sessionInfoBlob = new byte[_fileHeader.SessionInfoLength];

        var bytesRead = RandomAccess.Read(_fileHandle, sessionInfoBlob, _fileHeader.SessionInfoOffset);

        VerifyBytesRead(bytesRead, sessionInfoBlob.Length);

        // Encoding is actually ISO-8859-1 not UTF8. UTF8 works fine except for tracks with diacritics in their name e.g. MotorLand Aragon
        return Encoding.Latin1.GetString(sessionInfoBlob);
    }

    private static bool CheckFileSize(
        SafeFileHandle fileHandle,
        in DataFileHeader fileHeader,
        in DataBufferHeader dataBufferHeader,
        out long fileLength,
        out long expectedLength)
    {
        ArgumentNullException.ThrowIfNull(fileHandle);

        fileLength = RandomAccess.GetLength(fileHandle);

        expectedLength = dataBufferHeader.BufferOffset +
            fileHeader.DataBufferElementLength * fileHeader.DiskSubHeader.SessionRecordCount;

        return fileLength >= expectedLength;
    }

    /// <summary>
    /// Verifies that the number of bytes read is not less than the number of bytes expected to have been read, otherwise throws <see cref="IOException"/>.
    /// </summary>
    /// <param name="bytesRead"></param>
    /// <param name="bytesExpected"></param>
    /// <exception cref="IOException"></exception>
    private static void VerifyBytesRead(int bytesRead, int bytesExpected)
    {
        if (bytesRead < bytesExpected)
        {
            throw new IOException(
                $"The number of bytes read from the file ({bytesRead}) was less than required to complete the operation ({bytesExpected}).");
        }
    }

    private void VerifyCanRead()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_fileHandle.IsClosed)
        {
            throw new InvalidOperationException("The file handle is closed.");
        }

        if (_fileHandle.IsInvalid)
        {
            throw new InvalidOperationException("The file handle is invalid.");
        }
    }

    #region IDisposable Implementation

    /// <summary>
    /// Releases all resources used by the <see cref="TelemetryFileReader"/> class.
    /// </summary>
    /// <param name="disposing">If <see langword="true" />, indicates that managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                if (_isFileHandleOwner)
                {
                    _fileHandle.Dispose();
                }
            }

            _isDisposed = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="TelemetryFileReader"/> class.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Implementation
}
