using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public readonly struct ContextClassModel : IEquatable<ContextClassModel>
{
    public ContextClassModel(ContextClassInfo classInfo, ImmutableArray<ContextVariableModel> variables)
    {
        ClassName = classInfo.ClassName;
        ClassNamespace = classInfo.ClassNamespace;
        Variables = variables.GetEmptyIfDefault();
    }

    public ContextClassModel(string className, string classNamespace, ImmutableArray<ContextVariableModel> variables)
    {
        ClassName = className;
        ClassNamespace = classNamespace;
        Variables = variables.GetEmptyIfDefault();
    }

    public readonly string ClassName { get; }
    public readonly string ClassNamespace { get; }
    public readonly ImmutableArray<ContextVariableModel> Variables { get; }

    public SyntaxToken ClassIdentifier()
    {
        return Identifier(ClassName);
    }

    public IdentifierNameSyntax ClassIdentifierName()
    {
        return IdentifierName(ClassName);
    }

    public override bool Equals(object obj)
    {
        return obj is ContextClassModel other && Equals(other);
    }

    public bool Equals(ContextClassModel other)
    {
        return StringComparer.Ordinal.Equals(ClassName, other.ClassName) &&
            StringComparer.Ordinal.Equals(ClassNamespace, other.ClassNamespace) &&
            Variables.SequenceEqualDefaultTolerant(other.Variables);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(ClassName);
        hc.Add(ClassNamespace);

        if (!Variables.IsDefault)
        {
            for (int i = 0; i < Variables.Length; i++)
            {
                hc.Add(Variables[i]);
            }
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
