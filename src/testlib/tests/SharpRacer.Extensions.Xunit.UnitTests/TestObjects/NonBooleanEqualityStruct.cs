namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct NonBooleanEqualityStruct
{
    public NonBooleanEqualityStruct(int x)
    {
        X = x;
    }

    public readonly int X { get; }

    public static int operator ==(NonBooleanEqualityStruct left, NonBooleanEqualityStruct right)
    {
        return left.Equals(right) ? 1 : 0;
    }

    public static int operator !=(NonBooleanEqualityStruct left, NonBooleanEqualityStruct right)
    {
        return left.Equals(right) ? 0 : 1;
    }
}
