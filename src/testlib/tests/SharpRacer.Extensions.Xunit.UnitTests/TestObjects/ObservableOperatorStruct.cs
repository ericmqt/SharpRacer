using System.Diagnostics.CodeAnalysis;

namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct ObservableOperatorStruct
{
    private readonly IOperatorCallbacks<ObservableOperatorStruct> _callbacks;

    public ObservableOperatorStruct(int x, IOperatorCallbacks<ObservableOperatorStruct> callbacks)
    {
        X = x;

        _callbacks = callbacks ?? throw new ArgumentNullException(nameof(callbacks));
    }

    public readonly int X { get; }

    public int CompareTo(ObservableOperatorStruct other)
    {
        return X.CompareTo(other.X);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ObservableOperatorStruct other && Equals(other);
    }

    public bool Equals(ObservableOperatorStruct other)
    {
        return X == other.X;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X);
    }

    public static bool operator ==(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        left._callbacks.OnEqualityOperator(left, right);

        return left.Equals(right);
    }

    public static bool operator !=(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        // Best practice is to use the expr. !(left == right) but we don't use this here to avoid invoking our operator observation
        // methods multiple times.

        left._callbacks.OnInequalityOperator(left, right);

        return !left.Equals(right);
    }

    public static bool operator <(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        left._callbacks.OnLessThanOperator(left, right);

        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        left._callbacks.OnLessThanOrEqualOperator(left, right);

        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        left._callbacks.OnGreaterThanOperator(left, right);

        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(ObservableOperatorStruct left, ObservableOperatorStruct right)
    {
        left._callbacks.OnGreaterThanOrEqualOperator(left, right);

        return left.CompareTo(right) >= 0;
    }
}
