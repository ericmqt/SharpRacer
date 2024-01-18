using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public readonly struct DescriptorPropertyModel : IEquatable<DescriptorPropertyModel>
{
    public DescriptorPropertyModel(string propertyName, string propertyXmlSummary, VariableModel variableModel)
    {
        PropertyName = propertyName;
        PropertyXmlSummary = propertyXmlSummary;
        VariableModel = variableModel != default
            ? variableModel
            : throw new ArgumentException(
                $"'{nameof(variableModel)}' cannot be equal to default({nameof(VariableModel)}.", nameof(variableModel));
    }

    public readonly string PropertyName { get; }
    public readonly string? PropertyXmlSummary { get; }
    public readonly VariableModel VariableModel { get; }
    public readonly string VariableName => VariableModel.VariableName;

    public readonly SyntaxToken PropertyIdentifier()
    {
        return Identifier(PropertyName);
    }

    public readonly IdentifierNameSyntax PropertyIdentifierName()
    {
        return IdentifierName(PropertyName);
    }

    public override bool Equals(object obj)
    {
        return obj is DescriptorPropertyModel other && Equals(other);
    }

    public bool Equals(DescriptorPropertyModel other)
    {
        return StringComparer.Ordinal.Equals(PropertyName, other.PropertyName) &&
            StringComparer.Ordinal.Equals(PropertyXmlSummary, other.PropertyXmlSummary) &&
            VariableModel == other.VariableModel;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(PropertyName);
        hc.Add(PropertyXmlSummary);
        hc.Add(VariableModel);

        return hc.ToHashCode();
    }

    public static bool operator ==(DescriptorPropertyModel left, DescriptorPropertyModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DescriptorPropertyModel left, DescriptorPropertyModel right)
    {
        return !left.Equals(right);
    }
}
