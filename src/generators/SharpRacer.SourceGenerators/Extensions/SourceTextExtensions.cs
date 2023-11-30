using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators;
public static class SourceTextExtensions
{
    public static bool TryGetString(this SourceText sourceText, out string sourceString)
    {
        if (sourceText is null)
        {
            throw new ArgumentNullException(nameof(sourceText));
        }

        sourceString = sourceText.ToString() ?? string.Empty;

        return !string.IsNullOrEmpty(sourceString);
    }
}
