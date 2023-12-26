using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct ContextClassModel : IEquatable<ContextClassModel>
{
    private readonly bool _isInitialized;

    public ContextClassModel(ContextClassInfo classInfo, ImmutableArray<ContextVariableModel> variables)
    {
        TypeName = classInfo.ClassName;
        TypeNamespace = classInfo.ClassNamespace;
        Variables = variables.GetEmptyIfDefault();

        _isInitialized = true;
    }

    public readonly bool HasDataVariablesContextInterface { get; }
    public readonly string TypeName { get; }
    public readonly string TypeNamespace { get; }
    public readonly ImmutableArray<ContextVariableModel> Variables { get; }

    public SyntaxToken ClassIdentifier()
    {
        return Identifier(TypeName);
    }

    public IdentifierNameSyntax ClassIdentifierName()
    {
        return IdentifierName(TypeName);
    }

    public override bool Equals(object obj)
    {
        return obj is ContextClassModel other && Equals(other);
    }

    public bool Equals(ContextClassModel other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        if (!other._isInitialized)
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(TypeName, other.TypeName) &&
            StringComparer.Ordinal.Equals(TypeNamespace, other.TypeNamespace) &&
            HasDataVariablesContextInterface == other.HasDataVariablesContextInterface &&
            Variables.SequenceEqual(other.Variables);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(_isInitialized);

        if (!_isInitialized)
        {
            return hc.ToHashCode();
        }

        hc.Add(TypeName);
        hc.Add(TypeNamespace);
        hc.Add(HasDataVariablesContextInterface);

        for (int i = 0; i < Variables.Length; i++)
        {
            hc.Add(Variables[i]);
        }

        return hc.ToHashCode();
    }

    public static bool operator ==(ContextClassModel left, ContextClassModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContextClassModel left, ContextClassModel right)
    {
        return !left.Equals(right);
    }
}
