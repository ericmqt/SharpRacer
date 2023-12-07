using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Json;
internal readonly struct JsonLocationFactory
{
    private readonly string _filePath;
    private readonly SourceText _sourceText;

    public JsonLocationFactory(string filePath, SourceText sourceText)
    {
        _filePath = !string.IsNullOrEmpty(filePath)
            ? filePath
            : throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));

        _sourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
    }

    public Location GetLocation(TextSpan textSpan)
    {
        return CreateLocation(textSpan, textSpan);
    }

    public Location? GetLocation(JsonException jsonException)
    {
        if (!jsonException.LineNumber.HasValue)
        {
            return null;
        }

        var lineNumber = (int)jsonException.LineNumber.Value;
        var exLinePositionStart = (int)jsonException.BytePositionInLine.GetValueOrDefault();

        var sourceLine = _sourceText.Lines[lineNumber];
        LinePositionSpan linePositionSpan;

        if (sourceLine.Span.Length >= exLinePositionStart + 1)
        {
            linePositionSpan = new LinePositionSpan(
                new LinePosition(lineNumber, exLinePositionStart),
                new LinePosition(lineNumber, exLinePositionStart + 1));
        }
        else
        {
            linePositionSpan = new LinePositionSpan(
                new LinePosition(sourceLine.LineNumber, sourceLine.Start),
                new LinePosition(sourceLine.LineNumber, sourceLine.End));
        }

        return CreateLocation(sourceLine.Span, linePositionSpan);
    }

    private Location CreateLocation(TextSpan textSpan, TextSpan linePositionTextSpan)
    {
        var linePositionSpan = _sourceText.Lines.GetLinePositionSpan(linePositionTextSpan);

        return CreateLocation(textSpan, linePositionSpan);
    }

    private Location CreateLocation(TextSpan textSpan, LinePositionSpan linePositionSpan)
    {
        return Location.Create(_filePath, textSpan, linePositionSpan);
    }
}
