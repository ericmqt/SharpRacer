using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal readonly struct PipelineValueResult<TResult> : IEquatable<PipelineValueResult<TResult>>
    where TResult : struct, IEquatable<TResult>
{
    private readonly bool _isInitialized;

    public PipelineValueResult()
    {
        Value = new TResult();
        Diagnostics = ImmutableArray<Diagnostic>.Empty;

        _isInitialized = true;
    }

    public PipelineValueResult(TResult value, ImmutableArray<Diagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics;
        HasErrors = Diagnostics.HasErrors();

        _isInitialized = true;
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
        : this(new TResult(), diagnostics)
    {

    }

    public PipelineValueResult(Diagnostic diagnostic)
        : this(new TResult(), ImmutableArray.Create(diagnostic))
    {

    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool HasErrors { get; }
    public bool IsDefault => !_isInitialized;
    public bool IsEmpty => this == Empty;
    public bool IsDefaultOrEmpty => IsDefault || IsEmpty;
    public readonly TResult Value { get; }

    public static PipelineValueResult<TResult> Empty { get; } = new PipelineValueResult<TResult>();

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
