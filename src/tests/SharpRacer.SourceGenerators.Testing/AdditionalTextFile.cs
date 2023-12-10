using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.Testing;
public sealed class AdditionalTextFile : AdditionalText
{
    private readonly string _contents;

    public AdditionalTextFile(string path, string contents)
    {
        Path = !string.IsNullOrEmpty(path)
            ? path
            : throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        _contents = !string.IsNullOrEmpty(contents)
            ? contents
            : throw new ArgumentException($"'{nameof(contents)}' cannot be null or empty.", nameof(contents));
    }

    public override string Path { get; }

    public override SourceText? GetText(CancellationToken cancellationToken = default)
    {
        return SourceText.From(_contents, Encoding.UTF8);
    }
}
