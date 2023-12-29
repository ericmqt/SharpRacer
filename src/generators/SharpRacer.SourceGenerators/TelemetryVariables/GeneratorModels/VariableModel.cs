using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableModel : IEquatable<VariableModel>
{
    public VariableModel(VariableInfo variableInfo, VariableOptions options)
    {
        VariableInfo = variableInfo != default
            ? variableInfo
            : throw new ArgumentException($"'{nameof(variableInfo)}' cannot be a default value.", nameof(variableInfo));

        Options = options;
    }

    public VariableModel(VariableInfo variableInfo, VariableOptions options, VariableModel? deprecatingVariable)
        : this(variableInfo, options)
    {
        DeprecatingVariable = deprecatingVariable;
    }

    public readonly VariableModel? DeprecatingVariable { get; }
    public string Description => VariableInfo.Description;
    public readonly VariableOptions Options { get; }
    public int ValueCount => VariableInfo.ValueCount;
    public VariableValueType ValueType => VariableInfo.ValueType;
    public string? ValueUnit => VariableInfo.ValueUnit;
    public readonly VariableInfo VariableInfo { get; }
    public string VariableName => VariableInfo.Name;

    public TypeSyntax DataVariableTypeArgument()
    {
        return SharpRacerTypes.DataVariableTypeArgument(ValueType, ValueUnit);
    }

    public VariableModel WithDeprecatingVariable(VariableModel deprecatingVariable)
    {
        return new VariableModel(VariableInfo, Options, deprecatingVariable);
    }

    public override bool Equals(object obj)
    {
        return obj is VariableModel other && Equals(other);
    }

    public bool Equals(VariableModel other)
    {
        return VariableInfo == other.VariableInfo && Options == other.Options && DeprecatingVariable == other.DeprecatingVariable;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableInfo);
        hc.Add(Options);
        hc.Add(DeprecatingVariable);

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
}
