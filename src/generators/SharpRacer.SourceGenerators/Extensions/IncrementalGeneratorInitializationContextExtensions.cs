using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
internal static class IncrementalGeneratorInitializationContextExtensions
{
    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValueProvider<ImmutableArray<Diagnostic>> diagnosticsProvider)
    {
        context.RegisterSourceOutput(
            diagnosticsProvider,
            static (context, diagnostics) =>
            {
                if (diagnostics.IsDefaultOrEmpty)
                {
                    return;
                }

                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            });
    }

    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<ImmutableArray<Diagnostic>> diagnosticsProvider)
    {
        context.RegisterSourceOutput(
            diagnosticsProvider.Where(static x => !x.IsDefaultOrEmpty),
            static (context, diagnostics) =>
            {
                foreach (var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            });
    }

    public static void ReportDiagnostics(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<Diagnostic> diagnosticsProvider)
    {
        context.RegisterSourceOutput(
            diagnosticsProvider,
            static (context, diagnostic) =>
            {
                if (diagnostic != null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            });
    }
}
