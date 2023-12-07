using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
internal readonly struct VariableContextClassResult : IEquatable<VariableContextClassResult>
{
    public VariableContextClassResult(ClassWithGeneratorAttribute generatorClassResult, IncludedVariables includedVariables)
    {
        GeneratorClass = generatorClassResult;
        IncludedVariables = includedVariables;
    }

    public ClassWithGeneratorAttribute GeneratorClass { get; }
    public IncludedVariables IncludedVariables { get; }

    public override bool Equals(object obj)
    {
        return obj is VariableContextClassResult other && Equals(other);
    }

    public bool Equals(VariableContextClassResult other)
    {
        return GeneratorClass.Equals(other.GeneratorClass) &&
            IncludedVariables == other.IncludedVariables;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GeneratorClass, IncludedVariables);
    }

    public static bool operator ==(VariableContextClassResult lhs, VariableContextClassResult rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(VariableContextClassResult lhs, VariableContextClassResult rhs)
    {
        return !lhs.Equals(rhs);
    }
}
