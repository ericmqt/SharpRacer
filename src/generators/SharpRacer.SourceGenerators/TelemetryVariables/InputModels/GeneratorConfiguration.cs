namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal readonly struct GeneratorConfiguration : IEquatable<GeneratorConfiguration>
{
    public GeneratorConfiguration(
        VariableInfoFileName variableInfoFileName,
        VariableOptionsFileName variableOptionsFileName,
        bool generateVariableClasses,
        string variableClassesNamespace)
    {
        GenerateVariableClasses = generateVariableClasses;

        VariableInfoFileName = variableInfoFileName != default
            ? variableInfoFileName
            : throw new ArgumentException($"'{nameof(variableInfoFileName)}' cannot be the default value.", nameof(variableInfoFileName));

        VariableOptionsFileName = variableOptionsFileName != default
            ? variableOptionsFileName
            : throw new ArgumentException($"'{nameof(variableOptionsFileName)}' cannot be the default value.", nameof(variableOptionsFileName));

        VariableClassesNamespace = !string.IsNullOrEmpty(variableClassesNamespace)
            ? variableClassesNamespace
            : throw new ArgumentException($"'{nameof(variableClassesNamespace)}' cannot be null or empty.", nameof(variableClassesNamespace));
    }

    public bool GenerateVariableClasses { get; }
    public string VariableClassesNamespace { get; }
    public VariableInfoFileName VariableInfoFileName { get; }
    public VariableOptionsFileName VariableOptionsFileName { get; }

    public override bool Equals(object? obj)
    {
        return obj is GeneratorConfiguration other && Equals(other);
    }

    public bool Equals(GeneratorConfiguration other)
    {
        return GenerateVariableClasses == other.GenerateVariableClasses &&
            StringComparer.Ordinal.Equals(VariableClassesNamespace, other.VariableClassesNamespace) &&
            VariableInfoFileName == other.VariableInfoFileName &&
            VariableOptionsFileName == other.VariableOptionsFileName;
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(GenerateVariableClasses);
        hc.Add(VariableClassesNamespace);
        hc.Add(VariableInfoFileName);
        hc.Add(VariableOptionsFileName);

        return hc.ToHashCode();
    }

    public static bool operator ==(GeneratorConfiguration left, GeneratorConfiguration right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GeneratorConfiguration left, GeneratorConfiguration right)
    {
        return !(left == right);
    }
}
