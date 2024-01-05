using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableModel : IEquatable<VariableModel>
{
    private readonly VariableInfo _variableInfo;

    public VariableModel(VariableInfo variableInfo, VariableOptions options)
    {
        _variableInfo = variableInfo != default
            ? variableInfo
            : throw new ArgumentException($"'{nameof(variableInfo)}' cannot be a default value.", nameof(variableInfo));

        Options = options;
    }

    public string? DeprecatingVariableName => _variableInfo.DeprecatedBy;
    public string Description => _variableInfo.Description;
    public bool IsDeprecated => _variableInfo.IsDeprecated;
    public readonly VariableOptions Options { get; }
    public int ValueCount => _variableInfo.ValueCount;
    public VariableValueType ValueType => _variableInfo.ValueType;
    public string? ValueUnit => _variableInfo.ValueUnit;
    public string VariableName => _variableInfo.Name;

    public TypeSyntax DataVariableTypeArgument()
    {
        return SharpRacerTypes.DataVariableTypeArgument(ValueType, ValueUnit, TypeNameFormat.Qualified);
    }

    public string DescriptorPropertyName()
    {
        if (Options != default && !string.IsNullOrWhiteSpace(Options.Name))
        {
            return Options.Name!;
        }

        return VariableName;
    }

    public string VariableClassName()
    {
        if (Options != default && !string.IsNullOrWhiteSpace(Options.ClassName))
        {
            return $"{Options.ClassName}Variable";
        }

        return $"{VariableName}Variable";
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
