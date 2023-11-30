using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal class VariableContextClassGeneratorTarget
{
    public VariableContextClassGeneratorTarget(VariableContextClassGeneratorInfo contextTypeInfo, IncludedVariableNamesFile? includedVariableNamesFile)
    {
        TargetClassSymbol = contextTypeInfo.TargetClassSymbol;
        GeneratorAttributeLocation = contextTypeInfo.GeneratorAttributeLocation;
        IncludedVariableNamesFile = includedVariableNamesFile;
    }

    public INamedTypeSymbol TargetClassSymbol { get; }
    public Location? GeneratorAttributeLocation { get; }
    public IncludedVariableNamesFile? IncludedVariableNamesFile { get; }

    public static VariableContextClassGeneratorTarget Create(VariableContextClassGeneratorInfo contextTypeInfo, ImmutableArray<AdditionalText> additionalTexts, CancellationToken cancellationToken = default)
    {
        if (contextTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(contextTypeInfo));
        }

        cancellationToken.ThrowIfCancellationRequested();

        IncludedVariableNamesFile? includedVariableNamesFile = null;

        if (!string.IsNullOrEmpty(contextTypeInfo.IncludedVariableNamesArgumentValue))
        {
            includedVariableNamesFile = IncludedVariableNamesFile.Create(
                contextTypeInfo.IncludedVariableNamesArgumentValue!,
                contextTypeInfo,
                additionalTexts,
                cancellationToken);
        }

        return new VariableContextClassGeneratorTarget(contextTypeInfo, includedVariableNamesFile);
    }
}
