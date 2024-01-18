using Microsoft.CodeAnalysis;
using Moq;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class SharpRacerSymbolsTests
{
    [Fact]
    public void IsIDataVariablesContextInterfaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns(SharpRacerIdentifiers.TelemetryVariablesNamespace.ToGlobalQualifiedName());

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Interface);
        symbolMock.SetupGet(x => x.Name).Returns(SharpRacerIdentifiers.IDataVariablesContext.TypeName);
        symbolMock.SetupGet(x => x.ContainingNamespace).Returns(namespaceSymbolMock.Object);

        Assert.True(SharpRacerSymbols.IsIDataVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsIDataVariablesContextInterface_ReturnFalseIfNotInterfaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Class);

        Assert.False(SharpRacerSymbols.IsIDataVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsIDataVariablesContextInterface_ReturnFalseIfWrongNamespaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::TestApp");

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Interface);
        symbolMock.SetupGet(x => x.Name).Returns(SharpRacerIdentifiers.IDataVariablesContext.TypeName);
        symbolMock.SetupGet(x => x.ContainingNamespace).Returns(namespaceSymbolMock.Object);

        Assert.False(SharpRacerSymbols.IsIDataVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsTelemetryVariablesNamespaceTest()
    {
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::SharpRacer.Telemetry.Variables");

        Assert.True(SharpRacerSymbols.IsTelemetryVariablesNamespace(namespaceSymbolMock.Object));

    }
}
