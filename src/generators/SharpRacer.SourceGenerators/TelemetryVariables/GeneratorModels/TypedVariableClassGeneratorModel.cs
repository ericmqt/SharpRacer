using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class TypedVariableClassGeneratorModel
{
    private readonly string _descriptorFieldName;
    private readonly bool _implementCreateDataVariableInterface;
    private readonly string? _variableValueUnit;

    public TypedVariableClassGeneratorModel(
        string className,
        string classNamespace,
        VariableModel variableModel,
        DescriptorPropertyReference? descriptorPropertyReference,
        bool isClassInternal = false,
        bool isClassPartial = true)
    {
        ClassName = className;
        ClassNamespace = classNamespace;
        DescriptorPropertyReference = descriptorPropertyReference;
        IsClassInternal = isClassInternal;
        IsClassPartial = isClassPartial;

        VariableName = variableModel.VariableName;
        VariableValueCount = variableModel.VariableValueCount;
        VariableValueType = variableModel.VariableValueType;
        _variableValueUnit = variableModel.VariableValueUnit;

        _descriptorFieldName = "_Descriptor";

        AddCreateDataVariableInterfaceBaseType = true;
        _implementCreateDataVariableInterface = true;
    }

    public bool AddCreateDataVariableInterfaceBaseType { get; }
    public string ClassName { get; }
    public string ClassNamespace { get; }
    public DescriptorPropertyReference? DescriptorPropertyReference { get; }
    public bool ImplementCreateDataVariableInterface => _implementCreateDataVariableInterface || AddCreateDataVariableInterfaceBaseType;
    public bool IsClassInternal { get; }
    public bool IsClassPartial { get; }
    public string VariableName { get; }
    public int VariableValueCount { get; }
    public VariableValueType VariableValueType { get; }

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

    internal class EqualityComparer : IEqualityComparer<TypedVariableClassGeneratorModel?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<TypedVariableClassGeneratorModel?> Default { get; } = new EqualityComparer();

        public bool Equals(TypedVariableClassGeneratorModel? x, TypedVariableClassGeneratorModel? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(x.VariableName, y.VariableName) &&
                StringComparer.Ordinal.Equals(x.ClassName, y.ClassName) &&
                StringComparer.Ordinal.Equals(x.ClassNamespace, y.ClassNamespace) &&
                x.AddCreateDataVariableInterfaceBaseType == y.AddCreateDataVariableInterfaceBaseType &&
                x.DescriptorPropertyReference == y.DescriptorPropertyReference &&
                x.ImplementCreateDataVariableInterface == y.ImplementCreateDataVariableInterface &&
                x.IsClassInternal == y.IsClassInternal &&
                x.IsClassPartial == y.IsClassPartial &&
                x.VariableValueCount == y.VariableValueCount &&
                x.VariableValueType == y.VariableValueType &&
                StringComparer.Ordinal.Equals(x._variableValueUnit, y._variableValueUnit);
        }

        public int GetHashCode(TypedVariableClassGeneratorModel? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.AddCreateDataVariableInterfaceBaseType);
            hc.Add(obj.ClassName, StringComparer.Ordinal);
            hc.Add(obj.ClassNamespace, StringComparer.Ordinal);
            hc.Add(obj.DescriptorPropertyReference);
            hc.Add(obj.ImplementCreateDataVariableInterface);
            hc.Add(obj.IsClassInternal);
            hc.Add(obj.IsClassPartial);
            hc.Add(obj.VariableName, StringComparer.Ordinal);
            hc.Add(obj.VariableValueCount);
            hc.Add(obj.VariableValueType);
            hc.Add(obj._variableValueUnit, StringComparer.Ordinal);

            return hc.ToHashCode();
        }
    }

}
