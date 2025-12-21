using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

public readonly struct VariableClassModel : IEquatable<VariableClassModel>
{
    private readonly string _descriptorFieldName;
    private readonly string? _variableValueUnit;

    public VariableClassModel(
        string className,
        string classNamespace,
        VariableModel variableModel,
        DescriptorPropertyReference? descriptorPropertyReference,
        bool isClassInternal = false,
        bool isClassPartial = true)
        : this(className,
              classNamespace,
              descriptorPropertyReference,
              variableModel.VariableName,
              variableModel.ValueType,
              variableModel.ValueCount,
              variableModel.ValueUnit,
              isClassInternal,
              isClassPartial,
              ImmutableArray<Diagnostic>.Empty)
    {

    }

    private VariableClassModel(
        string className,
        string classNamespace,
        DescriptorPropertyReference? descriptorPropertyReference,
        string variableName,
        VariableValueType variableValueType,
        int variableValueCount,
        string? variableValueUnit,
        bool isClassInternal,
        bool isClassPartial,
        ImmutableArray<Diagnostic> diagnostics)
    {
        ClassName = className;
        ClassNamespace = classNamespace;
        DescriptorPropertyReference = descriptorPropertyReference;
        IsClassInternal = isClassInternal;
        IsClassPartial = isClassPartial;

        VariableName = variableName;
        VariableValueType = variableValueType;
        VariableValueCount = variableValueCount;
        _variableValueUnit = variableValueUnit;

        Diagnostics = diagnostics.GetEmptyIfDefault();

        _descriptorFieldName = "_Descriptor";
    }

    public readonly string ClassName { get; }
    public readonly string ClassNamespace { get; }
    public readonly DescriptorPropertyReference? DescriptorPropertyReference { get; }
    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool IsClassInternal { get; }
    public readonly bool IsClassPartial { get; }
    public readonly string VariableName { get; }
    public readonly int VariableValueCount { get; }
    public readonly VariableValueType VariableValueType { get; }

    public readonly BaseTypeSyntax BaseClassType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        var baseTypeClassName = VariableValueCount == 1 ? "ScalarTelemetryVariable" : "ArrayTelemetryVariable";
        var typeArgument = VariableValueTypeArg(typeNameFormat);

        var baseTypeName =
            QualifiedName(
                QualifiedName(
                    AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("SharpRacer")),
                    IdentifierName("Telemetry")),
                GenericName(Identifier(baseTypeClassName))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList(typeArgument))));

        return SimpleBaseType(baseTypeName);
    }

    public readonly Accessibility ClassAccessibility()
    {
        return IsClassInternal ? Accessibility.Internal : Accessibility.Public;
    }

    public readonly SyntaxToken ClassIdentifier()
    {
        return Identifier(ClassName);
    }

    public readonly IdentifierNameSyntax ClassIdentifierName()
    {
        return IdentifierName(ClassName);
    }

    public readonly FieldDeclarationSyntax DescriptorFieldDeclaration()
    {
        if (DescriptorPropertyReference != null)
        {
            return VariableClassSyntaxFactory.DescriptorStaticFieldFromDescriptorReferenceDeclaration(
                in this,
                DescriptorPropertyReference.Value.StaticPropertyMemberAccess());
        }

        return VariableClassSyntaxFactory.DescriptorStaticField(in this);
    }

    public readonly SyntaxToken DescriptorFieldIdentifier()
    {
        return Identifier(_descriptorFieldName);
    }

    public readonly IdentifierNameSyntax DescriptorFieldIdentifierName()
    {
        return IdentifierName(_descriptorFieldName);
    }

    public readonly TypeSyntax VariableValueTypeArg(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        return SharpRacerTypes.TelemetryVariableTypeArgument(VariableValueType, _variableValueUnit, typeNameFormat);
    }

    public readonly VariableClassModel WithDiagnostics(Diagnostic diagnostic)
    {
        return WithDiagnostics(ImmutableArray.Create(diagnostic));
    }

    public readonly VariableClassModel WithDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        return new VariableClassModel(
            ClassName,
            ClassNamespace,
            DescriptorPropertyReference,
            VariableName,
            VariableValueType,
            VariableValueCount,
            _variableValueUnit,
            IsClassInternal,
            IsClassPartial,
            diagnostics.GetEmptyIfDefault());
    }

    public override bool Equals(object obj)
    {
        return obj is VariableClassModel other && Equals(other);
    }

    public bool Equals(VariableClassModel other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
                StringComparer.Ordinal.Equals(ClassName, other.ClassName) &&
                StringComparer.Ordinal.Equals(ClassNamespace, other.ClassNamespace) &&
                DescriptorPropertyReference == other.DescriptorPropertyReference &&
                IsClassInternal == other.IsClassInternal &&
                IsClassPartial == other.IsClassPartial &&
                VariableValueCount == other.VariableValueCount &&
                VariableValueType == other.VariableValueType &&
                StringComparer.Ordinal.Equals(_variableValueUnit, other._variableValueUnit) &&
                Diagnostics.SequenceEqualDefaultTolerant(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(ClassName);
        hc.Add(ClassNamespace);
        hc.Add(DescriptorPropertyReference);
        hc.Add(IsClassInternal);
        hc.Add(IsClassPartial);
        hc.Add(VariableName);
        hc.Add(VariableValueCount);
        hc.Add(VariableValueType);
        hc.Add(_variableValueUnit);

        if (!Diagnostics.IsDefault)
        {
            for (int i = 0; i < Diagnostics.Length; i++)
            {
                hc.Add(Diagnostics[i]);
            }
        }

        return hc.ToHashCode();
    }

    public static bool operator ==(VariableClassModel left, VariableClassModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableClassModel left, VariableClassModel right)
    {
        return !left.Equals(right);
    }
}
