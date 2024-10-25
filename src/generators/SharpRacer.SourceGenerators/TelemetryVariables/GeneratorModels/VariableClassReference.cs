using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;

public readonly struct VariableClassReference : IEquatable<VariableClassReference>
{
    public VariableClassReference(string variableName, string className, string classNamespace)
    {
        VariableName = !string.IsNullOrEmpty(variableName)
            ? variableName
            : throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));

        ClassName = !string.IsNullOrEmpty(className)
            ? className
            : throw new ArgumentException($"'{nameof(className)}' cannot be null or empty.", nameof(className));

        ClassNamespace = !string.IsNullOrEmpty(classNamespace)
            ? classNamespace
            : throw new ArgumentException($"'{nameof(classNamespace)}' cannot be null or empty.", nameof(classNamespace));
    }

    public readonly string ClassName { get; }
    public readonly string ClassNamespace { get; }
    public readonly string VariableName { get; }

    public ObjectCreationExpressionSyntax ConstructorInvocation(IdentifierNameSyntax dataVariableInfoProviderIdentifier)
    {
        return ObjectCreationExpression(GlobalQualifiedTypeName())
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(dataVariableInfoProviderIdentifier))));
    }

    public NameSyntax GlobalQualifiedTypeName()
    {
        return ParseName($"global::{ClassNamespace}.{ClassName}");
    }

    public override bool Equals(object obj)
    {
        return obj is VariableClassReference other && Equals(other);
    }

    public bool Equals(VariableClassReference other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
            StringComparer.Ordinal.Equals(ClassName, other.ClassName) &&
            StringComparer.Ordinal.Equals(ClassNamespace, other.ClassNamespace);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableName);
        hc.Add(ClassName);
        hc.Add(ClassNamespace);

        return hc.ToHashCode();
    }

    public static bool operator ==(VariableClassReference left, VariableClassReference right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableClassReference left, VariableClassReference right)
    {
        return !left.Equals(right);
    }
}
