using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class SharpRacerSymbols
{
    public static bool IsTelemetryVariablesNamespace(INamespaceSymbol namespaceSymbol)
    {
        var namespaceSymbolDisplayString = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return namespaceSymbolDisplayString.Equals(SharpRacerIdentifiers.TelemetryVariablesNamespace.ToGlobalQualifiedName());
    }

    public static bool IsIDataVariablesContextInterface(INamedTypeSymbol symbol)
    {
        if (symbol.TypeKind != TypeKind.Interface)
        {
            return false;
        }

        if (!SharpRacerIdentifiers.IDataVariablesContext.Namespace.Equals(symbol.ContainingNamespace))
        {
            return false;
        }

        return symbol.Name.Equals(SharpRacerIdentifiers.IDataVariablesContext);
    }
}
