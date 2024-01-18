using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.Testing;
public sealed class AdditionalTextFile : AdditionalText
{
    private readonly Lazy<SourceText?> _sourceText;

    public AdditionalTextFile(string path, string contents)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _sourceText = new Lazy<SourceText?>(SourceText.From(contents, Encoding.UTF8));
    }

    public AdditionalTextFile(string path, SourceText sourceText)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _sourceText = new Lazy<SourceText?>(sourceText);
    }

    public override string Path { get; }

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return _sourceText.Value;
    }
}
