using System.Collections.Immutable;

namespace SharpRacer.SourceGenerators.Extensions;
public class ImmutableArrayExtensionsTests
{
    [Fact]
    public void GetEmptyIfDefault_Test()
    {
        ImmutableArray<int> defaultArray = default;

        Assert.Throws<NullReferenceException>(() => defaultArray.Length);
        var emptyDefaultArray = defaultArray.GetEmptyIfDefault();
        Assert.Empty(emptyDefaultArray);

        var intArray = ImmutableArray.Create(1, 2, 3);
        var intArrayOrEmptyIfDefault = intArray.GetEmptyIfDefault();

        Assert.Equal(3, intArrayOrEmptyIfDefault.Length);
        Assert.True(intArrayOrEmptyIfDefault.SequenceEqual(intArray));
    }

    [Fact]
    public void SequenceEqualDefaultTolerant_Test()
    {
        var array1 = ImmutableArray.Create(1, 2, 3);
        var array2 = ImmutableArray.Create(1, 2, 3);

        Assert.True(array1.SequenceEqualDefaultTolerant(array2));
        Assert.True(array2.SequenceEqualDefaultTolerant(array1));
    }

    [Fact]
    public void SequenceEqualDefaultTolerant_NotEqualTest()
    {
        var array1 = ImmutableArray.Create(1, 2, 3);
        var array2 = ImmutableArray.Create(3, 2, 1);

        Assert.False(array1.SequenceEqualDefaultTolerant(array2));
        Assert.False(array2.SequenceEqualDefaultTolerant(array1));
    }

    [Fact]
    public void SequenceEqualDefaultTolerant_ReturnFalseIfOneOperandIsDefaultTest()
    {
        var array1 = ImmutableArray.Create(1, 2, 3);

        Assert.False(array1.SequenceEqualDefaultTolerant(default));
    }
}
