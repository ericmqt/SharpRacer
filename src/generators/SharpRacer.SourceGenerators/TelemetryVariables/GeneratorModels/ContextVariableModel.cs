using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

internal readonly struct ContextVariableModel : IEquatable<ContextVariableModel>
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

    public InvocationExpressionSyntax DataVariableFactoryCreateMethodInvocation(IdentifierNameSyntax factoryInstanceIdentifier)
    {
        if (VariableClassReference != null)
        {
            var descriptorMemberAccessExpr = DescriptorReference.HasValue
                ? DescriptorReference.Value.StaticPropertyMemberAccess()
                : null;

            return VariableClassReference.Value.DataVariableFactoryCreateMethodInvocation(
                    factoryInstanceIdentifier,
                    descriptorMemberAccessExpr);
        }

        if (DescriptorReference != null)
        {
            var descriptorMemberAccessExpr = DescriptorReference.Value.StaticPropertyMemberAccess();

            if (IsArray)
            {
                return DataVariableFactorySyntaxFactory.CreateArrayFromDescriptorMethodInvocation(
                    factoryInstanceIdentifier,
                    VariableModel.DataVariableTypeArgument(),
                    descriptorMemberAccessExpr);
            }

            return DataVariableFactorySyntaxFactory.CreateScalarFromDescriptorMethodInvocation(
                factoryInstanceIdentifier,
                VariableModel.DataVariableTypeArgument(),
                descriptorMemberAccessExpr);
        }

        if (IsArray)
        {
            return DataVariableFactorySyntaxFactory.CreateArrayFromVariableNameAndArrayLengthMethodInvocation(
                factoryInstanceIdentifier,
                VariableModel.DataVariableTypeArgument(),
                VariableModel.VariableName,
                VariableModel.ValueCount);
        }

        return DataVariableFactorySyntaxFactory.CreateScalarFromVariableNameMethodInvocation(
            factoryInstanceIdentifier,
            VariableModel.DataVariableTypeArgument(),
            VariableModel.VariableName);
    }

    public SyntaxToken PropertyIdentifier()
    {
        return Identifier(PropertyName);
    }

    public IdentifierNameSyntax PropertyIdentifierName()
    {
        return IdentifierName(PropertyName);
    }

    public TypeSyntax PropertyType()
    {
        if (VariableClassReference != null)
        {
            return VariableClassReference.Value.GlobalQualifiedTypeName();
        }

        if (IsArray)
        {
            return SharpRacerTypes.IArrayDataVariableInterfaceType(VariableModel.DataVariableTypeArgument());
        }

        return SharpRacerTypes.IScalarDataVariableInterfaceType(VariableModel.DataVariableTypeArgument());
    }

    public override bool Equals(object obj)
    {
        return obj is ContextVariableModel other && Equals(other);
    }

    public bool Equals(ContextVariableModel other)
    {
        return VariableModel == other.VariableModel &&
            StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(PropertyXmlSummary, other.PropertyXmlSummary) &&
            VariableClassReference == other.VariableClassReference &&
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
