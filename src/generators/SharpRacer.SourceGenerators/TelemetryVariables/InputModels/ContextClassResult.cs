using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct ContextClassResult : IEquatable<ContextClassResult>
{
    private ContextClassResult(ContextClassInfo contextClassInfo, ImmutableArray<Diagnostic> diagnostics, bool isValid)
    {
        ContextClassInfo = contextClassInfo;
        Diagnostics = diagnostics.GetEmptyIfDefault();
        IsValid = isValid;
    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly ContextClassInfo ContextClassInfo { get; }
    public readonly bool IsValid { get; }

    public static ContextClassResult Create(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDeclarationNode = context.TargetNode as ClassDeclarationSyntax;
        var classDeclarationSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (context.Attributes.Length > 1 ||
            classDeclarationNode is null ||
            classDeclarationSymbol is null ||
            classDeclarationSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            return new ContextClassResult(default, ImmutableArray<Diagnostic>.Empty, isValid: false);
        }

        var attributeData = context.Attributes.First();
        var attributeLocation = attributeData.GetLocation();

        return Create(classDeclarationNode, classDeclarationSymbol, attributeData, attributeLocation, cancellationToken);
    }

    public static ContextClassResult Create(
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classDeclarationSymbol,
        AttributeData attributeData,
        Location? attributeLocation,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool isValid = true;
        var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

        var classIdentifier = classDeclarationSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var classDeclLocation = classDeclaration.Identifier.GetLocation();

        if (!classDeclaration.IsPartialClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.ContextClassMustBeDeclaredPartial(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (classDeclaration.IsStaticClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.ContextClassMustNotBeDeclaredStatic(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (!classDeclarationSymbol.Interfaces.Any(SharpRacerSymbols.IsIDataVariablesContextInterface))
        {
            diagnosticsBuilder.Add(
                GeneratorDiagnostics.ContextClassMustInheritIDataVariablesContextInterface(classIdentifier, classDeclLocation));

            isValid = false;
        }

        var diagnostics = diagnosticsBuilder.ToImmutableArray();

        if (!isValid)
        {
            return new ContextClassResult(default, diagnostics, isValid: false);
        }

        var includedVariablesFileName = GenerateDataVariablesContextAttributeInfo.GetIncludedVariablesFileNameOrDefault(attributeData);
        var classInfo = new ContextClassInfo(classDeclarationSymbol, attributeLocation, includedVariablesFileName);

        return new ContextClassResult(classInfo, diagnostics, isValid: true);
    }

    public override bool Equals(object obj)
    {
        return obj is ContextClassResult other && Equals(other);
    }

    public bool Equals(ContextClassResult other)
    {
        return IsValid == other.IsValid &&
            ContextClassInfo == other.ContextClassInfo &&
            Diagnostics.SequenceEqualDefaultTolerant(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(IsValid);
        hc.Add(ContextClassInfo);
        hc.AddDiagnosticArray(Diagnostics);

        return hc.ToHashCode();
    }

    public static bool operator ==(ContextClassResult left, ContextClassResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContextClassResult left, ContextClassResult right)
    {
        return !left.Equals(right);
    }
}
