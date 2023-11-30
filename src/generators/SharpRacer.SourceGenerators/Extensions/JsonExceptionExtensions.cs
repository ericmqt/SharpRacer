using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators;
internal static class JsonExceptionExtensions
{
    public static Location? GetJsonTextLocation(this JsonException jsonException, AdditionalText additionalText, SourceText sourceText)
    {
        if (jsonException is null)
        {
            throw new ArgumentNullException(nameof(jsonException));
        }

        if (additionalText is null)
        {
            throw new ArgumentNullException(nameof(additionalText));
        }

        if (sourceText is null)
        {
            throw new ArgumentNullException(nameof(sourceText));
        }

        if (!jsonException.LineNumber.HasValue)
        {
            return null;
        }

        var lineNumber = (int)jsonException.LineNumber.Value;
        var exLinePositionStart = (int)jsonException.BytePositionInLine.GetValueOrDefault();

        var sourceLine = sourceText.Lines[lineNumber];
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

        return Location.Create(additionalText.Path, sourceLine.Span, linePositionSpan);
    }
}
