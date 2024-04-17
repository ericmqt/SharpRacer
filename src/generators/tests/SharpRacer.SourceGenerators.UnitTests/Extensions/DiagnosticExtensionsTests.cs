using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.Extensions;
public class DiagnosticExtensionsTests
{
    [Fact]
    public void HasErrors_Test()
    {
        var errorDiagnostics = ImmutableArray.Create(CreateErrorDiagnostic());
        Assert.True(errorDiagnostics.HasErrors());

        var warningDiagnostics = ImmutableArray.Create(CreateWarningDiagnostic());
        Assert.False(warningDiagnostics.HasErrors());
    }

    [Fact]
    public void HasErrors_DefaultArrayReturnsFalseTest()
    {
        ImmutableArray<Diagnostic> diagnostics = default;

        Assert.False(diagnostics.HasErrors());
    }

    [Fact]
    public void IsError_Test()
    {
        var errorDiagnostic = CreateErrorDiagnostic();
        Assert.True(errorDiagnostic.IsError());

        var warningDiagnostic = CreateWarningDiagnostic();
        Assert.False(warningDiagnostic.IsError());
    }

    private static Diagnostic CreateErrorDiagnostic()
    {
        var descriptor = new DiagnosticDescriptor("TEST01", "Test error", "Test!", "Test", DiagnosticSeverity.Error, isEnabledByDefault: true);
        return Diagnostic.Create(descriptor, Location.None);
    }

    private static Diagnostic CreateWarningDiagnostic()
    {
        var descriptor = new DiagnosticDescriptor("TEST02", "Test warning", "Test!", "Test", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        return Diagnostic.Create(descriptor, Location.None);
    }
}
