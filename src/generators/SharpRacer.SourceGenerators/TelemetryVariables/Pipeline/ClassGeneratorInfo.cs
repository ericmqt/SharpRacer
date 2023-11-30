using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal abstract class ClassGeneratorInfo
{
    public ClassGeneratorInfo(
        INamedTypeSymbol targetClassSymbol,
        AttributeData generatorAttributeData,
        Location? generatorAttributeLocation)
    {
        TargetClassSymbol = targetClassSymbol ?? throw new ArgumentNullException(nameof(targetClassSymbol));
        GeneratorAttributeData = generatorAttributeData ?? throw new ArgumentNullException(nameof(generatorAttributeData));
        GeneratorAttributeLocation = generatorAttributeLocation;
    }

    public AttributeData GeneratorAttributeData { get; }
    public Location? GeneratorAttributeLocation { get; }
    public INamedTypeSymbol TargetClassSymbol { get; }

    public string GetTargetClassFullTypeName()
    {
        if (TargetClassSymbol.ContainingNamespace is null)
        {
            return TargetClassSymbol.Name;
        }

        return $"{TargetClassSymbol.ContainingNamespace}.{TargetClassSymbol.Name}";
    }

    protected static IncrementalValuesProvider<T> GetValuesProvider<T>(
        SyntaxValueProvider syntaxValueProvider,
        string attributeTypeName,
        Func<ClassDeclarationSyntax, bool> predicate,
        Func<INamedTypeSymbol, AttributeData, Location?, T> factory,
        IEqualityComparer<T> comparer)
        where T : ClassGeneratorInfo
    {
        return GetValuesProvider(
            syntaxValueProvider,
            attributeTypeName,
            (node, ct) => IsTargetCandidate(node, ct, predicate),
            factory,
            comparer);
    }

    private static IncrementalValuesProvider<T> GetValuesProvider<T>(
        SyntaxValueProvider syntaxValueProvider,
        string attributeTypeName,
        Func<SyntaxNode, CancellationToken, bool> predicate,
        Func<INamedTypeSymbol, AttributeData, Location?, T> factory,
        IEqualityComparer<T> comparer)
        where T : ClassGeneratorInfo
    {
        return syntaxValueProvider.ForAttributeWithMetadataName(
            attributeTypeName,
            predicate,
            transform: (context, cancellationToken) =>
            {
                if (!TryGetClassWithAttribute(
                    context,
                    attributeTypeName,
                    cancellationToken,
                    out INamedTypeSymbol? targetClassSymbol,
                    out AttributeData? attributeData,
                    out Location? attributeLocation))
                {
                    return null;
                }

                return factory(targetClassSymbol!, attributeData!, attributeLocation);
                //return new ClassGeneratorTarget(targetClassSymbol!, attributeData!, attributeLocation);
            })
            .WhereNotNull()
            .WithComparer(comparer);
    }

    private static bool IsTargetCandidate(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken,
        Func<ClassDeclarationSyntax, bool> classPredicate)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classPredicate(classDeclarationSyntax);
        }

        return false;
    }

    private static bool TryGetClassWithAttribute(
        GeneratorAttributeSyntaxContext context,
        string attributeFullTypeName,
        CancellationToken cancellationToken,
        out INamedTypeSymbol? targetClassSymbol,
        out AttributeData? attributeData,
        out Location? attributeLocation)
    {
        if (string.IsNullOrEmpty(attributeFullTypeName))
        {
            throw new ArgumentException($"'{nameof(attributeFullTypeName)}' cannot be null or empty.", nameof(attributeFullTypeName));
        }

        attributeData = null;
        attributeLocation = null;

        // Find attribute symbol
        var attributeTypeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(attributeFullTypeName);

        cancellationToken.ThrowIfCancellationRequested();

        // Ensure we have a named type
        targetClassSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (attributeTypeSymbol is null || targetClassSymbol is null || targetClassSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            return false;
        }

        attributeData = AttributeDataLocator.Find(targetClassSymbol!, attributeTypeSymbol!, out attributeLocation);

        return attributeData != null;
    }
}
