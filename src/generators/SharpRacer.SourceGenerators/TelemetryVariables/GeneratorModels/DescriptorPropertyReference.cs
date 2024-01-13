using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public readonly struct DescriptorPropertyReference : IEquatable<DescriptorPropertyReference>
{
    public DescriptorPropertyReference(DescriptorClassModel descriptorClass, DescriptorPropertyModel propertyModel)
    {
        if (descriptorClass == default)
        {
            throw new ArgumentException($"'{nameof(descriptorClass)}' cannot be a default value.", nameof(descriptorClass));
        }

        if (propertyModel == default)
        {
            throw new ArgumentException($"'{nameof(propertyModel)}' cannot be a default value.", nameof(propertyModel));
        }

        DescriptorClassNamespace = descriptorClass.TypeNamespace;
        DescriptorClassName = descriptorClass.TypeName;
        PropertyName = propertyModel.PropertyName;
        VariableName = propertyModel.VariableName;
    }

    public DescriptorPropertyReference(string variableName, string propertyName, string descriptorClassName, string descriptorClassNamespace)
    {
        VariableName = !string.IsNullOrEmpty(variableName)
            ? variableName
            : throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));

        PropertyName = !string.IsNullOrEmpty(propertyName)
            ? propertyName
            : throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));

        DescriptorClassName = !string.IsNullOrEmpty(descriptorClassName)
            ? descriptorClassName
            : throw new ArgumentException($"'{nameof(descriptorClassName)}' cannot be null or empty.", nameof(descriptorClassName));

        DescriptorClassNamespace = !string.IsNullOrEmpty(descriptorClassNamespace)
            ? descriptorClassNamespace
            : throw new ArgumentException($"'{nameof(descriptorClassNamespace)}' cannot be null or empty.", nameof(descriptorClassNamespace));
    }

    public readonly string DescriptorClassNamespace { get; }
    public readonly string DescriptorClassName { get; }
    public readonly string PropertyName { get; }
    public readonly string VariableName { get; }

    public NameSyntax GlobalQualifiedTypeName()
    {
        return ParseName($"global::{DescriptorClassNamespace}.{DescriptorClassName}");
    }

    public MemberAccessExpressionSyntax StaticPropertyMemberAccess()
    {
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            GlobalQualifiedTypeName(),
            IdentifierName(PropertyName));
    }

    public override bool Equals(object obj)
    {
        return obj is DescriptorPropertyReference other && Equals(other);
    }

    public bool Equals(DescriptorPropertyReference other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
            StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(DescriptorClassNamespace, other.DescriptorClassNamespace) &&
            StringComparer.Ordinal.Equals(DescriptorClassName, other.DescriptorClassName);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(DescriptorClassNamespace);
        hc.Add(DescriptorClassName);
        hc.Add(PropertyName);
        hc.Add(VariableName);

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorPropertyReference left, DescriptorPropertyReference right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorPropertyReference left, DescriptorPropertyReference right)
    {
        return !left.Equals(right);
    }
}
