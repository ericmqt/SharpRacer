using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal readonly struct PipelineValueResult<TResult> : IEquatable<PipelineValueResult<TResult>>
    where TResult : struct, IEquatable<TResult>
{
    private readonly ImmutableArray<Diagnostic> _diagnostics;
    private readonly bool _isInitialized;

    public PipelineValueResult()
        : this(default, false, ImmutableArray<Diagnostic>.Empty)
    {

    }

    public PipelineValueResult(TResult value, Diagnostic diagnostic)
        : this(value, true, ImmutableArray.Create(diagnostic))
    {

    }

    private PipelineValueResult(TResult value, bool hasValue, ImmutableArray<Diagnostic> diagnostics)
    {
        Value = value;
        HasValue = hasValue;
        _diagnostics = diagnostics.GetEmptyIfDefault();
        _isInitialized = true;
    }

    public ImmutableArray<Diagnostic> Diagnostics => _diagnostics.GetEmptyIfDefault();
    public bool HasErrors => Diagnostics.HasErrors();
    public readonly bool HasValue { get; }
    public readonly TResult Value { get; }

    public override bool Equals(object obj)
    {
        return obj is PipelineValueResult<TResult> other && Equals(other);
    }

    public bool Equals(PipelineValueResult<TResult> other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return HasErrors == other.HasErrors &&
            HasValue == other.HasValue &&
            Value.Equals(other.Value) &&
            _diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        hc.Add(Value);
        hc.AddDiagnosticArray(_diagnostics);

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

    public static implicit operator PipelineValueResult<TResult>(TResult result)
    {
        return new PipelineValueResult<TResult>(result, true, ImmutableArray<Diagnostic>.Empty);
    }

    public static implicit operator PipelineValueResult<TResult>(Diagnostic diagnostic)
    {
        return new PipelineValueResult<TResult>(default, false, ImmutableArray.Create(diagnostic));
    }

    public static implicit operator PipelineValueResult<TResult>(ImmutableArray<Diagnostic> diagnostics)
    {
        return new PipelineValueResult<TResult>(default, false, diagnostics);
    }
}
