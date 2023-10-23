using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using SharpRacer.Interop;

namespace SharpRacer.IO;

/// <summary>
/// Provides helper methods for handling telemetry files.
/// </summary>
internal static class TelemetryFile
{
    /// <summary>
    /// Calculates the expected length of a telemetry file from its header.
    /// </summary>
    /// <param name="fileHeader">The <see cref="DataFileHeader"/> from which to calculate the expected file size.</param>
    /// <returns>The expected length, in bytes, of the telemetry file.</returns>
    public static int CalculateFileLengthFromHeader(in DataFileHeader fileHeader)
    {
        var totalDataBufferLength = fileHeader.DataBufferElementLength * fileHeader.DiskSubHeader.SessionRecordCount;

        return fileHeader.DataBufferHeaders[0].BufferOffset + totalDataBufferLength;
    }

    /// <summary>
    /// Opens a <see cref="SafeFileHandle"/> for reading a telemetry file from the specified file name.
    /// </summary>
    /// <param name="fileName">The path to the telemetry file.</param>
    /// <returns>A <see cref="SafeFileHandle"/> to the telemetry file.</returns>
    public static SafeFileHandle OpenHandle(string fileName)
    {
        return File.OpenHandle(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    /// <summary>
    /// Reads a <see cref="DataFileHeader"/> value from the specified telemetry file path.
    /// </summary>
    /// <param name="fileName">The path to the telemetry file.</param>
    /// <returns>The <see cref="DataFileHeader"/> value read from the telemetry file.</returns>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is a null or empty string.</exception>
    /// <exception cref="IOException">The number of bytes read from the file was insufficient to complete the operation.</exception>
    public static DataFileHeader ReadHeader(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        using var fileHandle = OpenHandle(fileName);

        return ReadHeader(fileHandle);
    }

    /// <summary>
    /// Reads a <see cref="DataFileHeader"/> value from the specified <see cref="SafeFileHandle"/>.
    /// </summary>
    /// <param name="fileHandle">The handle to the telemetry file to read.</param>
    /// <returns>The <see cref="DataFileHeader"/> value read from the telemetry file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileHandle"/> is null.</exception>
    /// <exception cref="IOException">The number of bytes read from the file was insufficient to complete the operation.</exception>
    public static DataFileHeader ReadHeader(SafeFileHandle fileHandle)
    {
        ArgumentNullException.ThrowIfNull(fileHandle);

        Span<byte> headerBlob = new byte[DataFileHeader.Size];

        var bytesRead = RandomAccess.Read(fileHandle, headerBlob, DataFileConstants.HeaderOffset);

        if (bytesRead < DataFileHeader.Size)
        {
            throw new IOException(
                $"The number of bytes read from the file ({bytesRead}) was insufficient to complete the operation ({DataFileHeader.Size}).");
        }

        return MemoryMarshal.Read<DataFileHeader>(headerBlob);
    }

    /// <summary>
    /// Validates the specified <see cref="DataFileHeader"/> to ensure it has valid or expected values.
    /// </summary>
    /// <param name="fileHeader">The <see cref="DataFileHeader"/> to validate.</param>
    /// <returns><see langword="true"/> if the specified header is valid, otherwise <see langword="false" />.</returns>
    public static bool ValidateHeader(in DataFileHeader fileHeader)
    {
        if (fileHeader.HeaderVersion != DataFileConstants.HeaderVersion)
        {
            return false;
        }

        if (fileHeader.DataBufferCount < 1 || fileHeader.DataBufferCount > DataFileConstants.MaxDataVariableBuffers)
        {
            return false;
        }

        // Check that the data buffer length is at least one byte.
        if (fileHeader.DataBufferElementLength < 1)
        {
            return false;
        }

        if (fileHeader.VariableCount < 0)
        {
            return false;
        }

        if (fileHeader.SessionInfoLength < 0)
        {
            return false;
        }

        if (fileHeader.DiskSubHeader.SessionRecordCount < 0)
        {
            return false;
        }

        // Verify offsets are not negative or point to an offset inside the file header
        if (fileHeader.VariableHeaderOffset < 0 || fileHeader.VariableHeaderOffset < DataFileHeader.Size)
        {
            return false;
        }

        if (fileHeader.SessionInfoOffset < 0 || fileHeader.SessionInfoOffset < DataFileHeader.Size)
        {
            return false;
        }

        if (fileHeader.DataBufferHeaders[0].BufferOffset < DataFileHeader.Size)
        {
            return false;
        }

        return true;
    }
}
