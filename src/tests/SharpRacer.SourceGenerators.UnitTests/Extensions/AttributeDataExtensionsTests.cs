using Microsoft.CodeAnalysis;
using Moq;

namespace SharpRacer.SourceGenerators.Extensions;
public class AttributeDataExtensionsTests
{
    [Fact]
    public void NullApplicationSyntaxReferenceReturnsLocationNoneTest()
    {
        var mock = new Mock<AttributeData>(MockBehavior.Strict);

        mock.Setup(x => x.ApplicationSyntaxReference).Returns<SyntaxReference?>(_ => null!);

        var location = mock.Object.GetLocation();

        Assert.Equal(Location.None, location);
    }
}
