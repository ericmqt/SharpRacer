using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.Extensions;
public class GeneratorAttributeSyntaxContextExtensionsTests
{
    [Fact]
    public void TryGetClassSymbolWithAttribute_ThrowOnNullOrEmptyAttributeTypeNameTest()
    {
        GeneratorAttributeSyntaxContext context = default;

        Assert.Throws<ArgumentException>(() => context.TryGetClassSymbolWithAttribute(null!, default, out _, out _, out _));
        Assert.Throws<ArgumentException>(() => context.TryGetClassSymbolWithAttribute(string.Empty, default, out _, out _, out _));
    }
}
