using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

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

    public readonly VariableOptions Options { get; }
    public readonly VariableInfo VariableInfo { get; }

    public override bool Equals(object obj)
    {
        return obj is VariableModel other && Equals(other);
    }

    public bool Equals(VariableModel other)
    {
        return VariableInfo == other.VariableInfo && Options == other.Options;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(VariableInfo);
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
}
