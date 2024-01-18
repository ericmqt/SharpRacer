using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public readonly struct VariableModel : IEquatable<VariableModel>
{
    private readonly VariableInfo _variableInfo;

    public VariableModel(VariableInfo variableInfo, VariableOptions options)
    {
        _variableInfo = variableInfo != default
            ? variableInfo
            : throw new ArgumentException($"'{nameof(variableInfo)}' cannot be a default value.", nameof(variableInfo));

        Options = options;
    }

    public readonly string? DeprecatingVariableName => _variableInfo.DeprecatedBy;
    public readonly string Description => _variableInfo.Description;
    public readonly bool IsDeprecated => _variableInfo.IsDeprecated;
    public readonly VariableOptions Options { get; }
    public readonly int ValueCount => _variableInfo.ValueCount;
    public readonly VariableValueType ValueType => _variableInfo.ValueType;
    public readonly string? ValueUnit => _variableInfo.ValueUnit;
    public readonly string VariableName => _variableInfo.Name;

    public readonly TypeSyntax DataVariableTypeArgument()
    {
        return SharpRacerTypes.DataVariableTypeArgument(ValueType, ValueUnit, TypeNameFormat.Qualified);
    }

    public readonly string DescriptorPropertyName()
    {
        if (Options != default && !string.IsNullOrWhiteSpace(Options.Name))
        {
            return Options.Name!;
        }

        return VariableName;
    }

    #region IEquatable Implementation

    public override bool Equals(object obj)
    {
        return obj is VariableModel other && Equals(other);
    }

    public bool Equals(VariableModel other)
    {
        return _variableInfo == other._variableInfo && Options == other.Options;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(_variableInfo);
        hc.Add(Options);

        return hc.ToHashCode();
    }

    public static bool operator ==(VariableModel left, VariableModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableModel left, VariableModel right)
    {
        return !left.Equals(right);
    }

    #endregion IEquatable Implementation
}
