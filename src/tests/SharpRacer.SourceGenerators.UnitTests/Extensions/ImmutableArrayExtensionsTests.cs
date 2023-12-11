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
}
