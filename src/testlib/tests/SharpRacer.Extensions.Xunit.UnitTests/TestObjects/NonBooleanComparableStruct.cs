namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct NonBooleanComparableStruct : IComparable<NonBooleanComparableStruct>
{
    public NonBooleanComparableStruct(int x)
    {
        X = x;
    }

    public readonly int X { get; }

    public int CompareTo(NonBooleanComparableStruct other)
    {
        return X.CompareTo(other.X);
    }

    public static int operator <(NonBooleanComparableStruct left, NonBooleanComparableStruct right)
    {
        return left.CompareTo(right);
    }

    public static int operator >(NonBooleanComparableStruct left, NonBooleanComparableStruct right)
    {
        return left.CompareTo(right);
    }

    public static int operator <=(NonBooleanComparableStruct left, NonBooleanComparableStruct right)
    {
        return left.CompareTo(right);
    }

    public static int operator >=(NonBooleanComparableStruct left, NonBooleanComparableStruct right)
    {
        return left.CompareTo(right);
    }
}
