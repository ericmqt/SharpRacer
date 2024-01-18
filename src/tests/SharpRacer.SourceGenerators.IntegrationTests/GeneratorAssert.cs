using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Xunit.Sdk;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class GeneratorAssert
{
    public static void ContainsDiagnostic(GeneratorRunResult runResult, string diagnosticId)
    {
        if (!runResult.Diagnostics.Any(x => x.Id == diagnosticId))
        {
            throw new XunitException($"Run result does not contain a diagnostic with ID '{diagnosticId}'");
        }
    }

    public static void DoesNotContainDiagnostic(GeneratorRunResult runResult, string diagnosticId)
    {
        if (runResult.Diagnostics.Any(x => x.Id == diagnosticId))
        {
            throw new XunitException($"Run result contains a diagnostic with ID '{diagnosticId}'");
        }
    }

    public static void NoDiagnostics(GeneratorRunResult runResult)
    {
        if (runResult.Diagnostics.Any())
        {
            throw new XunitException($"Run result contains diagnostics.");
        }
    }

    public static void NoException(GeneratorRunResult runResult)
    {
        if (runResult.Exception != null)
        {
            throw new XunitException($"Run result threw {runResult.Exception.GetType()}: {runResult.Exception.Message}");
        }
    }

    public static void NoErrorDiagnostics(GeneratorRunResult runResult)
    {
        if (runResult.Diagnostics.Where(x => x.IsError()).Any())
        {
            throw new XunitException($"Run result contains error diagnostics.");
        }
    }

    public static ImmutableArray<IncrementalGeneratorRunStep> TrackedStepExecuted(GeneratorRunResult runResult, string trackedStepName)
    {
        if (string.IsNullOrEmpty(trackedStepName))
        {
            throw new ArgumentException($"'{nameof(trackedStepName)}' cannot be null or empty.", nameof(trackedStepName));
        }

        if (!runResult.TrackedSteps.Any(x => x.Key == trackedStepName))
        {
            throw new XunitException($"Generator did not execute tracked step '{trackedStepName}'");
        }

        return runResult.TrackedSteps[trackedStepName];
    }

    public static void TrackedStepNotExecuted(GeneratorRunResult runResult, string trackedStepName)
    {
        if (string.IsNullOrEmpty(trackedStepName))
        {
            throw new ArgumentException($"'{nameof(trackedStepName)}' cannot be null or empty.", nameof(trackedStepName));
        }

        if (runResult.TrackedSteps.Any(x => x.Key == trackedStepName))
        {
            throw new XunitException($"Generator executed tracked step '{trackedStepName}'");
        }
    }
}
