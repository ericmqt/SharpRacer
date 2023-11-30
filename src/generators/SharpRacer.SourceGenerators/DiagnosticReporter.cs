using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
public readonly ref struct DiagnosticReporter
{
    private readonly Action<Diagnostic> _action;

    public DiagnosticReporter(Action<Diagnostic> reportDiagnosticAction)
    {
        _action = reportDiagnosticAction;
    }

    public void Report(Diagnostic diagnostic)
    {
        _action(diagnostic);
    }
}