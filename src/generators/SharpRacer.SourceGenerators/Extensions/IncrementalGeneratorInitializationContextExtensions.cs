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
}
