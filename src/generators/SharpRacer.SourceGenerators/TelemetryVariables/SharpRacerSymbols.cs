using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class SharpRacerSymbols
{
    public static bool IsTelemetryVariablesNamespace(INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(SharpRacerIdentifiers.TelemetryVariablesNamespace.ToGlobalQualifiedName());
    }

    public static bool IsIDataVariablesContextInterface(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Interface)
        {
            return false;
        }

        if (!IsTelemetryVariablesNamespace(symbol.ContainingNamespace))
        {
            return false;
        }

        return symbol.Name.Equals(SharpRacerIdentifiers.IDataVariablesContext);
    }
}
