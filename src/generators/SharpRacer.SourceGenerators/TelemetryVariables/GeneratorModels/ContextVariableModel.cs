using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

public readonly struct ContextVariableModel : IEquatable<ContextVariableModel>
{
    public ContextVariableModel(
        VariableModel variableModel,
        string propertyName,
        string propertyXmlSummary,
        VariableClassReference? variableClassReference,
        DescriptorPropertyReference? descriptorPropertyReference)
    {
        VariableModel = variableModel;

        PropertyName = propertyName;
        PropertyXmlSummary = propertyXmlSummary;
        VariableClassReference = variableClassReference;
        DescriptorReference = descriptorPropertyReference;
    }

    public readonly DescriptorPropertyReference? DescriptorReference { get; }
    public bool IsArray => VariableModel.ValueCount > 1;
    public readonly string PropertyName { get; }
    public readonly string PropertyXmlSummary { get; }
    public readonly VariableClassReference? VariableClassReference { get; }
    public readonly VariableModel VariableModel { get; }

    public SyntaxToken PropertyIdentifier()
    {
        return Identifier(PropertyName);
    }

    public IdentifierNameSyntax PropertyIdentifierName()
    {
        return IdentifierName(PropertyName);
    }

    public ObjectCreationExpressionSyntax PropertyObjectCreationExpression(IdentifierNameSyntax variableInfoProviderIdentifier)
    {
        if (VariableClassReference != null)
        {
            return VariableClassReference.Value.ConstructorInvocation(variableInfoProviderIdentifier);
        }

        // Create ArrayTelemetryVariable<T> and ScalarTelemetryVariable<T> instances
        var typeArg = SharpRacerTypes.TelemetryVariableTypeArgument(
            VariableModel.ValueType, VariableModel.ValueUnit, TypeNameFormat.GlobalQualified);

        ExpressionSyntax descriptorCtorArgumentExpr;

        if (DescriptorReference != null)
        {
            descriptorCtorArgumentExpr = DescriptorReference.Value.StaticPropertyMemberAccess();
        }
        else
        {
            descriptorCtorArgumentExpr = TelemetryVariableDescriptorSyntaxFactory.StaticFactoryMethodInvocation(
                VariableModel.VariableName, VariableModel.ValueType, VariableModel.ValueCount, VariableModel.ValueUnit);
        }

        if (IsArray)
        {
            return TelemetryVariableTypeSyntaxFactory.ArrayTelemetryVariableConstructor(
                typeArg, descriptorCtorArgumentExpr, variableInfoProviderIdentifier);
        }

        return TelemetryVariableTypeSyntaxFactory.ScalarTelemetryVariableConstructor(
                typeArg, descriptorCtorArgumentExpr, variableInfoProviderIdentifier);
    }

    public TypeSyntax PropertyType(TypeNameFormat typeNameFormat = TypeNameFormat.Default)
    {
        if (VariableClassReference != null)
        {
            return VariableClassReference.Value.GlobalQualifiedTypeName();
        }

        var typeArg = VariableModel.TelemetryVariableTypeArgument(typeNameFormat);

        if (IsArray)
        {
            return SharpRacerTypes.IArrayTelemetryVariableInterfaceType(typeArg, typeNameFormat);
        }

        return SharpRacerTypes.IScalarTelemetryVariableInterfaceType(typeArg, typeNameFormat);
    }

    public override bool Equals(object obj)
    {
        return obj is ContextVariableModel other && Equals(other);
    }

    public bool Equals(ContextVariableModel other)
    {
        if (VariableModel != other.VariableModel)
        {
            return false;
        }

        if (!StringComparer.Ordinal.Equals(PropertyName, other.PropertyName))
        {
            return false;
        }

        if (!StringComparer.Ordinal.Equals(PropertyXmlSummary, other.PropertyXmlSummary))
        {
            return false;
        }

        return VariableClassReference == other.VariableClassReference &&
            DescriptorReference == other.DescriptorReference;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(DescriptorReference);
        hc.Add(PropertyName);
        hc.Add(PropertyXmlSummary);
        hc.Add(VariableClassReference);
        hc.Add(VariableModel);

        return hc.ToHashCode();
    }

    public static bool operator ==(ContextVariableModel left, ContextVariableModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContextVariableModel left, ContextVariableModel right)
    {
        return !left.Equals(right);
    }
}
