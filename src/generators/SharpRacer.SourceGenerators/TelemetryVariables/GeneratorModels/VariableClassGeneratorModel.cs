using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableClassGeneratorModel : IEquatable<VariableClassGeneratorModel>
{
    private readonly string _descriptorFieldName;
    private readonly bool _implementCreateDataVariableInterface;
    private readonly string? _variableValueUnit;

    public VariableClassGeneratorModel(
        string className,
        string classNamespace,
        VariableModel variableModel,
        ImmutableArray<Diagnostic> diagnostics,
        DescriptorPropertyReference? descriptorPropertyReference,
        bool isClassInternal = false,
        bool isClassPartial = true)
    {
        ClassName = className;
        ClassNamespace = classNamespace;
        DescriptorPropertyReference = descriptorPropertyReference;
        Diagnostics = diagnostics.GetEmptyIfDefault();
        IsClassInternal = isClassInternal;
        IsClassPartial = isClassPartial;

        VariableName = variableModel.VariableName;
        VariableValueCount = variableModel.ValueCount;
        VariableValueType = variableModel.ValueType;
        _variableValueUnit = variableModel.ValueUnit;

        _descriptorFieldName = "_Descriptor";

        AddCreateDataVariableInterfaceBaseType = true;
        _implementCreateDataVariableInterface = true;
    }

    public readonly bool AddCreateDataVariableInterfaceBaseType { get; }
    public readonly string ClassName { get; }
    public readonly string ClassNamespace { get; }
    public readonly DescriptorPropertyReference? DescriptorPropertyReference { get; }
    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool ImplementCreateDataVariableInterface => _implementCreateDataVariableInterface || AddCreateDataVariableInterfaceBaseType;
    public readonly bool IsClassInternal { get; }
    public readonly bool IsClassPartial { get; }
    public readonly string VariableName { get; }
    public readonly int VariableValueCount { get; }
    public readonly VariableValueType VariableValueType { get; }

    public bool Equals(VariableClassGeneratorModel other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
                StringComparer.Ordinal.Equals(ClassName, other.ClassName) &&
                StringComparer.Ordinal.Equals(ClassNamespace, other.ClassNamespace) &&
                AddCreateDataVariableInterfaceBaseType == other.AddCreateDataVariableInterfaceBaseType &&
                DescriptorPropertyReference == other.DescriptorPropertyReference &&
                ImplementCreateDataVariableInterface == other.ImplementCreateDataVariableInterface &&
                IsClassInternal == other.IsClassInternal &&
                IsClassPartial == other.IsClassPartial &&
                VariableValueCount == other.VariableValueCount &&
                VariableValueType == other.VariableValueType &&
                StringComparer.Ordinal.Equals(_variableValueUnit, other._variableValueUnit);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(AddCreateDataVariableInterfaceBaseType);
        hc.Add(ClassName);
        hc.Add(ClassNamespace);
        hc.Add(DescriptorPropertyReference);
        hc.Add(ImplementCreateDataVariableInterface);
        hc.Add(IsClassInternal);
        hc.Add(IsClassPartial);
        hc.Add(VariableName);
        hc.Add(VariableValueCount);
        hc.Add(VariableValueType);
        hc.Add(_variableValueUnit);

        return hc.ToHashCode();
    }

    public BaseTypeSyntax BaseClassType()
    {
        if (VariableValueCount > 1)
        {
            return SharpRacerTypes.ArrayDataVariableBaseType(VariableValueTypeArg());
        }

        return SharpRacerTypes.ScalarDataVariableBaseType(VariableValueTypeArg());
    }

    public Accessibility ClassAccesibility()
    {
        return IsClassInternal ? Accessibility.Internal : Accessibility.Public;
    }

    public SyntaxToken ClassIdentifier()
    {
        return Identifier(ClassName);
    }

    public IdentifierNameSyntax ClassIdentifierName()
    {
        return IdentifierName(ClassName);
    }

    public FieldDeclarationSyntax DescriptorFieldDeclaration()
    {
        if (DescriptorPropertyReference != null)
        {
            var descriptorAccessExpr = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(DescriptorPropertyReference.Value.DescriptorClassName),
                IdentifierName(DescriptorPropertyReference.Value.PropertyName));

            return VariableClassSyntaxFactory.DescriptorStaticFieldFromDescriptorReferenceDeclaration(
                DescriptorFieldIdentifier(),
                descriptorAccessExpr);
        }

        return VariableClassSyntaxFactory.DescriptorStaticField(
            DescriptorFieldIdentifier(),
            VariableName,
            VariableValueType,
            VariableValueCount);
    }

    public SyntaxToken DescriptorFieldIdentifier()
    {
        return Identifier(_descriptorFieldName);
    }

    public IdentifierNameSyntax DescriptorFieldIdentifierName()
    {
        return IdentifierName(_descriptorFieldName);
    }

    public TypeSyntax VariableValueTypeArg()
    {
        return SharpRacerTypes.DataVariableTypeArgument(VariableValueType, _variableValueUnit);
    }

    public override bool Equals(object obj)
    {
        return obj is VariableClassGeneratorModel other && Equals(other);
    }

    public static bool operator ==(VariableClassGeneratorModel left, VariableClassGeneratorModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableClassGeneratorModel left, VariableClassGeneratorModel right)
    {
        return !left.Equals(right);
    }
}
