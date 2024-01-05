using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct DescriptorClassModelResult : IEquatable<DescriptorClassModelResult>
{
    private DescriptorClassModelResult(DescriptorClassModel model, ImmutableArray<Diagnostic> diagnostics, bool isValid)
    {
        Model = model;
        Diagnostics = diagnostics.GetEmptyIfDefault();
        IsValid = isValid;
    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool IsValid { get; }
    public readonly DescriptorClassModel Model { get; }

    public static DescriptorClassModelResult Create(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDeclarationNode = context.TargetNode as ClassDeclarationSyntax;
        var classDeclarationSymbol = context.TargetSymbol as INamedTypeSymbol;

        if (context.Attributes.Length > 1 ||
            classDeclarationNode is null ||
            classDeclarationSymbol is null ||
            classDeclarationSymbol.ContainingNamespace.IsGlobalNamespace)
        {
            return new DescriptorClassModelResult(default, ImmutableArray<Diagnostic>.Empty, isValid: false);
        }

        var attributeData = context.Attributes.First();
        var attributeLocation = attributeData.GetLocation();

        return Create(classDeclarationNode, classDeclarationSymbol, attributeData, attributeLocation, cancellationToken);
    }

    public static DescriptorClassModelResult Create(
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
            diagnosticsBuilder.Add(GeneratorDiagnostics.DescriptorClassMustBeDeclaredPartial(classIdentifier, classDeclLocation));

            isValid = false;
        }

        if (!classDeclaration.IsStaticClass())
        {
            diagnosticsBuilder.Add(GeneratorDiagnostics.DescriptorClassMustBeDeclaredStatic(classIdentifier, classDeclLocation));

            isValid = false;
        }

        var diagnostics = diagnosticsBuilder.ToImmutableArray();

        if (!isValid)
        {
            return new DescriptorClassModelResult(default, diagnostics, isValid: false);
        }

        var model = new DescriptorClassModel(
            classDeclarationSymbol.Name,
            classDeclarationSymbol.ContainingNamespace.ToString(),
            attributeLocation);

        return new DescriptorClassModelResult(model, diagnostics, isValid: true);
    }

    public override bool Equals(object obj)
    {
        return obj is DescriptorClassModelResult other && Equals(other);
    }

    public bool Equals(DescriptorClassModelResult other)
    {
        return IsValid == other.IsValid &&
            Model == other.Model &&
            Diagnostics.SequenceEqualDefaultTolerant(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(IsValid);
        hc.Add(Model);
        hc.AddDiagnosticArray(Diagnostics);

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorClassModelResult left, DescriptorClassModelResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorClassModelResult left, DescriptorClassModelResult right)
    {
        return !left.Equals(right);
    }
}
