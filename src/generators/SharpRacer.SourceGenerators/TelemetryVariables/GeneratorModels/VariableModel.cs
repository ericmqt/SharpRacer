using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal readonly struct VariableModel : IEquatable<VariableModel>
{
    public VariableModel(
        VariableInfo variableInfo,
        string variableName,
        string contextPropertyName,
        string descriptorName)
    {
        if (variableInfo == default)
        {
            throw new ArgumentException($"'{nameof(variableInfo)}' cannot be a default value.", nameof(variableInfo));
        }

        VariableInfo = variableInfo;

        VariableName = !string.IsNullOrEmpty(variableName)
            ? variableName
            : throw new ArgumentException($"'{nameof(variableName)}' cannot be null or empty.", nameof(variableName));

        ContextPropertyName = !string.IsNullOrEmpty(contextPropertyName)
            ? contextPropertyName
            : throw new ArgumentException($"'{nameof(contextPropertyName)}' cannot be null or empty.", nameof(contextPropertyName));

        DescriptorName = !string.IsNullOrEmpty(descriptorName)
            ? descriptorName
            : throw new ArgumentException($"'{nameof(descriptorName)}' cannot be null or empty.", nameof(descriptorName));
    }

    public readonly string ContextPropertyName { get; }

    public readonly string DescriptorName { get; }

    public bool IsArray => VariableInfo.ValueCount > 1;

    public readonly string VariableName { get; }

    public readonly VariableInfo VariableInfo { get; }

    public int VariableValueCount => VariableInfo.ValueCount;
    public VariableValueType VariableValueType => VariableInfo.ValueType;
    public string? VariableValueUnit => VariableInfo.ValueUnit;

    public override bool Equals(object obj)
    {
        return obj is VariableModel other && Equals(other);
    }

    public bool Equals(VariableModel other)
    {
        return StringComparer.Ordinal.Equals(VariableName, other.VariableName) &&
            StringComparer.Ordinal.Equals(ContextPropertyName, other.ContextPropertyName) &&
            StringComparer.Ordinal.Equals(DescriptorName, other.DescriptorName) &&
            VariableInfo == other.VariableInfo;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(ContextPropertyName, StringComparer.Ordinal);
        hc.Add(DescriptorName, StringComparer.Ordinal);
        hc.Add(VariableName, StringComparer.Ordinal);
        hc.Add(VariableInfo);

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
