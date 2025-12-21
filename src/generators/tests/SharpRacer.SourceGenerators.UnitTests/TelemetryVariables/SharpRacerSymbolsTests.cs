using Microsoft.CodeAnalysis;
using Moq;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

public class SharpRacerSymbolsTests
{
    [Fact]
    public void IsITelemetryVariablesContextInterfaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns(SharpRacerIdentifiers.TelemetryNamespace.ToGlobalQualifiedName());

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Interface);
        symbolMock.SetupGet(x => x.Name).Returns(SharpRacerIdentifiers.ITelemetryVariablesContext.TypeName);
        symbolMock.SetupGet(x => x.ContainingNamespace).Returns(namespaceSymbolMock.Object);

        Assert.True(SharpRacerSymbols.IsITelemetryVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsITelemetryVariablesContextInterface_ReturnFalseIfNotInterfaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Class);

        Assert.False(SharpRacerSymbols.IsITelemetryVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsITelemetryVariablesContextInterface_ReturnFalseIfWrongNamespaceTest()
    {
        var symbolMock = new Mock<INamedTypeSymbol>();
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::TestApp");

        symbolMock.SetupGet(x => x.TypeKind).Returns(TypeKind.Interface);
        symbolMock.SetupGet(x => x.Name).Returns(SharpRacerIdentifiers.ITelemetryVariablesContext.TypeName);
        symbolMock.SetupGet(x => x.ContainingNamespace).Returns(namespaceSymbolMock.Object);

        Assert.False(SharpRacerSymbols.IsITelemetryVariablesContextInterface(symbolMock.Object));
    }

    [Fact]
    public void IsTelemetryNamespaceTest()
    {
        var namespaceSymbolMock = new Mock<INamespaceSymbol>();

        namespaceSymbolMock.Setup(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Returns("global::SharpRacer.Telemetry");

        Assert.True(SharpRacerSymbols.IsTelemetryNamespace(namespaceSymbolMock.Object));
    }
}
