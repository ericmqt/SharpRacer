using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators;

internal static class AdditionalTextExtensions
{
    internal static bool TryGetSourceTextString(this AdditionalText additionalText, CancellationToken cancellationToken, out string sourceTextString)
    {
        if (additionalText is null)
        {
            throw new ArgumentNullException(nameof(additionalText));
        }

        var sourceText = additionalText.GetText(cancellationToken);

        if (sourceText is null)
        {
            sourceTextString = string.Empty;
            return false;
        }

        return sourceText.TryGetString(out sourceTextString);
    }

    internal static bool TryGetSourceTextString(this AdditionalText additionalText, CancellationToken cancellationToken, out SourceText sourceText, out string sourceTextString)
    {
        if (additionalText is null)
        {
            throw new ArgumentNullException(nameof(additionalText));
        }

        sourceText = additionalText.GetText(cancellationToken)!;

        if (sourceText is null)
        {
            sourceTextString = string.Empty;
            return false;
        }

        return sourceText.TryGetString(out sourceTextString);
    }
}
