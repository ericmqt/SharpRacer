using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class VariableContextSyntaxFactoryTests
{
    [Fact]
    public void ConstructorDataVariableFactoryLocal_ThrowOnNullOrEmptyFactoryIdentifierArgTest()
    {
        Assert.Throws<ArgumentException>(() =>
            VariableContextSyntaxFactory.ConstructorDataVariableFactoryLocal(
                factoryIdentifier: null!, IdentifierName("dataVariableProvider")));

        Assert.Throws<ArgumentException>(() =>
            VariableContextSyntaxFactory.ConstructorDataVariableFactoryLocal(
                factoryIdentifier: string.Empty, IdentifierName("dataVariableProvider")));
    }

    [Fact]
    public void ConstructorDataVariableFactoryLocal_ThrowOnNullDataVariableProviderIdentifierArgTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
            VariableContextSyntaxFactory.ConstructorDataVariableFactoryLocal(
                factoryIdentifier: "factory", null!));
    }
}
