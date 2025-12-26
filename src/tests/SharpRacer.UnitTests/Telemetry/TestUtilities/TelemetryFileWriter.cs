using System.Runtime.InteropServices;
using System.Text;
using SharpRacer.Interop;

namespace SharpRacer.Telemetry.TestUtilities;

internal class TelemetryFileWriter
{
    public static void Write(
        string fileName,
        TelemetryFileHeader fileHeader,
        TelemetryVariableHeader[]? variableHeaders,
        string? sessionInfoYaml,
        IReadOnlyList<Memory<byte>>? dataFrames,
        out TelemetryFileHeader writtenFileHeader)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        fileHeader = fileHeader.WithTelemetryBufferElementLength(variableHeaders.GetDataFrameLength());

        if (dataFrames != null && dataFrames.Any())
        {
            if (dataFrames.Select(x => x.Length).Distinct().Count() > 1)
            {
                throw new ArgumentException("Data frames must be of equal length.", nameof(dataFrames));
            }

            if (variableHeaders is null || variableHeaders.Length == 0)
            {
                throw new ArgumentException($"Data frames were passed without any variable headers.", nameof(dataFrames));
            }

            var dataFrameLength = variableHeaders.GetDataFrameLength();

            if (!dataFrames.All(x => x.Length == fileHeader.TelemetryBufferElementLength))
            {
                throw new ArgumentException(
                    $"One or more data frames has a length less than the required length {fileHeader.TelemetryBufferElementLength} based on the variable headers.",
                    nameof(dataFrames));
            }
        }

        try
        {
            using var stream = File.OpenWrite(fileName);

            WriteFileHeader(stream, fileHeader);

            WriteVariableHeaders(stream, variableHeaders, ref fileHeader);
            WriteSessionInfo(stream, sessionInfoYaml, ref fileHeader);
            WriteDataFrames(stream, dataFrames, ref fileHeader);

            // Update header with modified offsets etc.
            stream.Seek(0, SeekOrigin.Begin);
            WriteFileHeader(stream, fileHeader);

            writtenFileHeader = fileHeader;
        }
        catch (IOException)
        {
            File.Delete(fileName);

            throw;
        }
    }

    private static void WriteFileHeader(FileStream stream, in TelemetryFileHeader fileHeader)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var headerBytes = new byte[TelemetryFileHeader.Size];
        MemoryMarshal.Write(headerBytes, fileHeader);

        stream.Write(headerBytes);
    }

    private static void WriteDataFrames(FileStream stream, IReadOnlyList<Memory<byte>>? dataFrames, ref TelemetryFileHeader fileHeader)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var dataFramesHeader = new TelemetryBufferHeader(0, (int)stream.Position);

        fileHeader = fileHeader
            .WithTelemetryBufferHeaders(TelemetryBufferHeaderArray.FromArray([dataFramesHeader, default, default, default]))
            .WithDiskSubHeader(
                fileHeader.DiskSubHeader.WithSessionRecordCount(dataFrames?.Count ?? 0));

        if (dataFrames is null)
        {
            return;
        }

        foreach (var frame in dataFrames)
        {
            stream.Write(frame.Span);
        }
    }

    private static void WriteSessionInfo(FileStream stream, string? sessionInfoYaml, ref TelemetryFileHeader fileHeader)
    {
        fileHeader = fileHeader.WithSessionInfoOffset((int)stream.Position)
            .WithSessionInfoLength(sessionInfoYaml?.Length ?? 0);

        if (!string.IsNullOrEmpty(sessionInfoYaml))
        {
            stream.Write(Encoding.Latin1.GetBytes(sessionInfoYaml));
        }
    }

    private static void WriteVariableHeaders(FileStream stream, TelemetryVariableHeader[]? variableHeaders, ref TelemetryFileHeader fileHeader)
    {
        // Variable headers
        fileHeader = fileHeader
            .WithTelemetryVariableCount(variableHeaders?.Length ?? 0)
            .WithTelemetryVariableHeaderOffset((int)stream.Position);

        if (variableHeaders != null && variableHeaders.Length > 0)
        {
            var variableHeaderBytes = MemoryMarshal.AsBytes<TelemetryVariableHeader>(variableHeaders);

            stream.Write(variableHeaderBytes);
        }
    }
}
