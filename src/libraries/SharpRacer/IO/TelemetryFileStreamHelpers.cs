using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Interop;

namespace SharpRacer.IO;
internal static class TelemetryFileStreamHelpers
{
    public static DataVariableHeader[] ReadDataVariableHeaders(Stream stream, in DataFileHeader header)
    {
        return ReadDataVariableHeaders(stream, header, restorePositionAfterRead: false);
    }

    public static DataVariableHeader[] ReadDataVariableHeaders(Stream stream, in DataFileHeader header, bool restorePositionAfterRead)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Store current position and seek to header
        var initialPosition = stream.Position;
        stream.Seek(header.VariableHeaderOffset, SeekOrigin.Begin);

        // Create an array of variable headers
        var variableHeaders = new DataVariableHeader[header.VariableCount];

        // Read from the stream into the byte array representation of the header array
        stream.ReadExactly(MemoryMarshal.AsBytes<DataVariableHeader>(variableHeaders));

        // Restore original position if requested
        if (restorePositionAfterRead)
        {
            stream.Seek(initialPosition, SeekOrigin.Begin);
        }

        return variableHeaders;
    }

    public static DataFileHeader ReadHeader(Stream stream)
    {
        return ReadHeader(stream, restorePositionAfterRead: false);
    }

    public static DataFileHeader ReadHeader(Stream stream, bool restorePositionAfterRead)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Store current position and seek to header
        var initialPosition = stream.Position;
        stream.Seek(0, SeekOrigin.Begin);

        // Read header byte array
        var headerBlob = new byte[DataFileHeader.Size];
        stream.ReadExactly(headerBlob);

        var header = MemoryMarshal.Read<DataFileHeader>(headerBlob);

        // Restore original position if requested
        if (restorePositionAfterRead)
        {
            stream.Seek(initialPosition, SeekOrigin.Begin);
        }

        return header;
    }

    public static string ReadSessionInfo(Stream stream, in DataFileHeader header)
    {
        return ReadSessionInfo(stream, header, restorePositionAfterRead: false);
    }

    public static string ReadSessionInfo(Stream stream, in DataFileHeader header, bool restorePositionAfterRead)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Store current position and seek
        var initialPosition = stream.Position;
        stream.Seek(header.SessionInfoOffset, SeekOrigin.Begin);

        // Read session info blob
        var sessionInfoBlob = new byte[header.SessionInfoLength];
        stream.ReadExactly(sessionInfoBlob);

        var sessionInfoStr = Encoding.UTF8.GetString(sessionInfoBlob);

        // Restore original position if requested
        if (restorePositionAfterRead)
        {
            stream.Seek(initialPosition, SeekOrigin.Begin);
        }

        return sessionInfoStr;
    }
}
