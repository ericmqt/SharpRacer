namespace SharpRacer.Extensions.Xunit.TestObjects;

public readonly struct PlainStruct
{
    public PlainStruct(int x)
    {
        X = x;
    }

    public readonly int X { get; }
}
