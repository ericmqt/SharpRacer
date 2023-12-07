using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal readonly struct PipelineValueResult<TResult> : IEquatable<PipelineValueResult<TResult>>
    where TResult : struct, IEquatable<TResult>
{
    public PipelineValueResult()
    {
        Value = default;
        Diagnostics = ImmutableArray<Diagnostic>.Empty;
        IsDefault = true;
    }

    public PipelineValueResult(TResult value, ImmutableArray<Diagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics;
        HasErrors = Diagnostics.HasErrors();
    }

    public PipelineValueResult(TResult value)
        : this(value, ImmutableArray<Diagnostic>.Empty)
    {

    }

    public PipelineValueResult(TResult value, Diagnostic diagnostic)
        : this(value, ImmutableArray.Create(diagnostic))
    {

    }

    public PipelineValueResult(ImmutableArray<Diagnostic> diagnostics)
        : this(default, diagnostics)
    {

    }

    public PipelineValueResult(Diagnostic diagnostic)
        : this(default, ImmutableArray.Create(diagnostic))
    {

    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool HasErrors { get; }
    public readonly bool IsDefault { get; }
    public readonly TResult Value { get; }

    public override bool Equals(object obj)
    {
        return obj is PipelineValueResult<TResult> other && Equals(other);
    }

    public bool Equals(PipelineValueResult<TResult> other)
    {
        return IsDefault == other.IsDefault &&
            HasErrors == other.HasErrors &&
            Value.Equals(other.Value) &&
            Diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value);
        hc.AddDiagnosticArray(Diagnostics);

        return hc.ToHashCode();
    }

    public static bool operator ==(PipelineValueResult<TResult> left, PipelineValueResult<TResult> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PipelineValueResult<TResult> left, PipelineValueResult<TResult> right)
    {
        return !left.Equals(right);
    }
}
