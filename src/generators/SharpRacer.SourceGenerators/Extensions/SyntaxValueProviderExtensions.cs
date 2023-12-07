using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators;
internal static class SyntaxValueProviderExtensions
{
    public static IncrementalValuesProvider<ClassWithGeneratorAttribute> ForClassWithAttribute(
        this SyntaxValueProvider syntaxValueProvider,
        string attributeFullTypeName,
        Func<ClassDeclarationSyntax, bool> predicate)
    {
        return syntaxValueProvider.ForAttributeWithMetadataName(
            attributeFullTypeName,
            predicate: (node, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                return node is ClassDeclarationSyntax classDeclaration && predicate(classDeclaration);
            },
            transform: (context, cancellationToken) =>
            {
                if (context.TryGetClassSymbolWithAttribute(
                        attributeFullTypeName,
                        cancellationToken,
                        out INamedTypeSymbol? targetClassSymbol,
                        out AttributeData? attributeData,
                        out Location? attributeLocation))
                {
                    return new ClassWithGeneratorAttribute(targetClassSymbol!, attributeData!, attributeLocation);
                }

                return default;
            })
            .Where(static item => item != default);
    }
}
