using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal readonly struct PipelineValuesResult<TResult> : IEquatable<PipelineValuesResult<TResult>>
    where TResult : struct, IEquatable<TResult>
{
    private readonly bool _isInitialized;

    public PipelineValuesResult()
    {
        Values = ImmutableArray<TResult>.Empty;
        Diagnostics = ImmutableArray<Diagnostic>.Empty;

        _isInitialized = true;
    }

    public PipelineValuesResult(ImmutableArray<TResult> values, ImmutableArray<Diagnostic> diagnostics)
    {
        Values = values;
        Diagnostics = diagnostics.GetEmptyIfDefault();
        HasErrors = Diagnostics.HasErrors();

        _isInitialized = true;
    }

    public PipelineValuesResult(ImmutableArray<TResult> values)
        : this(values, ImmutableArray<Diagnostic>.Empty)
    {

    }

    public PipelineValuesResult(ImmutableArray<Diagnostic> diagnostics)
        : this(ImmutableArray<TResult>.Empty, diagnostics)
    {

    }

    public PipelineValuesResult(Diagnostic diagnostic)
        : this(ImmutableArray<TResult>.Empty, ImmutableArray.Create(diagnostic))
    {

    }

    public readonly ImmutableArray<Diagnostic> Diagnostics { get; }
    public readonly bool HasErrors { get; }
    public bool IsDefault => !_isInitialized;
    public readonly ImmutableArray<TResult> Values { get; }

    public static PipelineValuesResult<TResult> Empty { get; } = new PipelineValuesResult<TResult>();

    public override bool Equals(object obj)
    {
        return obj is PipelineValuesResult<TResult> other && Equals(other);
    }

    public bool Equals(PipelineValuesResult<TResult> other)
    {
        if (!_isInitialized)
        {
            return !other._isInitialized;
        }

        return HasErrors == other.HasErrors &&
            Values.SequenceEqual(other.Values) &&
            Diagnostics.SequenceEqual(other.Diagnostics);
    }

    public override int GetHashCode()
    {
        var hc = new HashCode();

        for (int i = 0; i < Values.Length; i++)
        {
            hc.Add(Values[i]);
        }

        hc.AddDiagnosticArray(Diagnostics);

        return hc.ToHashCode();
    }

    public static bool operator ==(PipelineValuesResult<TResult> left, PipelineValuesResult<TResult> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PipelineValuesResult<TResult> left, PipelineValuesResult<TResult> right)
    {
        return !left.Equals(right);
    }
}
