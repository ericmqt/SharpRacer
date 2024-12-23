﻿using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.Diagnostics;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;
using SharpRacer.SourceGenerators.Testing.TelemetryVariables;
using ContextClassInfoResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo Model,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
using ContextClassTargetResult = (
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.ContextClassInfo Model,
    SharpRacer.SourceGenerators.TelemetryVariables.InputModels.IncludedVariablesFileName IncludedVariablesFileName,
    System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics);
namespace SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
public class ContextClassInfoValuesProviderTests
{
    [Fact]
    public void GetValuesProvider_Test()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoDiagnostics(runResult);
        var stepResult = GeneratorAssert.TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider).Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        Assert.Equal("Test.Assembly", result.Model.ClassNamespace);
        Assert.Equal("TelemetryVariables", result.Model.ClassName);
        Assert.NotNull(result.Model.GeneratorAttributeLocation);
        Assert.Single(result.Model.IncludedVariables.VariableNames);
        Assert.Single(result.Model.IncludedVariables.VariableNames, x => x.Value == "SessionTime");
    }

    [Fact]
    public void IncludedVariablesFileNameTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassTargetResult)stepOutput.Value;

        Assert.Equal("TelemetryVariables_VariableNames.json", result.IncludedVariablesFileName);
    }

    [Fact]
    public void IncludedVariablesFileNameNotProvidedTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoDiagnostics(runResult);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassTargetResult)stepOutput.Value;

        Assert.Equal(default, result.IncludedVariablesFileName);
    }

    [Fact]
    public void IncludedVariablesAmbiguousFileNameTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var includedVariables2 = IncludedVariablesDocumentBuilder.FromNames("SessionTime")
            .ToAdditionalTextFile("Assets\\TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithAdditionalText(includedVariables2)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariables_AmbiguousFileName);

        Assert.NotNull(stepResult);
        Assert.Single(stepResult.Inputs);
        Assert.Single(stepResult.Outputs);

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariables_AmbiguousFileName);
    }

    [Fact]
    public void IncludedVariablesFileAlreadyIncludesVariableTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime", "SessionTime")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        // Check that the pipeline did not stop and that we only have the one included variable
        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Model.IncludedVariables.VariableNames, x => x.Value == "SessionTime");
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariablesFile_VariableAlreadyIncluded);
    }

    [Fact]
    public void IncludedVariablesFileContainsEmptyVariableNameTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames("SessionTime", "")
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        // Check that the pipeline did not stop and that we only have the one included variable
        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Model.IncludedVariables.VariableNames, x => x.Value == "SessionTime");
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariablesFile_ContainsEmptyVariableName);
    }

    [Fact]
    public void IncludedVariablesFileContainsNoVariableNamesTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var includedVariables1 = IncludedVariablesDocumentBuilder.FromNames()
            .ToAdditionalTextFile("TelemetryVariables_VariableNames.json");

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText(includedVariables1)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        GeneratorAssert.NoException(runResult);
        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariablesFileContainsNoVariableNames);

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        // Check that the pipeline did not stop and that we only have the one included variable
        Assert.NotEqual(default, result.Model);
        Assert.Empty(result.Model.IncludedVariables.VariableNames);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariablesFileContainsNoVariableNames);
    }

    [Fact]
    public void IncludedVariablesFileNotFoundTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.IncludedVariables_FileNotFound);

        Assert.NotNull(stepResult);
        Assert.Single(stepResult.Inputs);
        Assert.Single(stepResult.Outputs);

        var stepOutput = stepResult.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, stepOutput.Reason);

        var result = (ContextClassInfoResult)stepOutput.Value;

        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.IncludedVariables_FileNotFound);
    }

    [Fact]
    public void IncludedVariablesFileReadExceptionTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithAdditionalText("TelemetryVariables_VariableNames.json", "[ \"TestVar")
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var stepResult = GeneratorAssert.TrackedStepExecuted(
            runResult, TrackingNames.ContextClassInfoValuesProvider_GetValuesProvider)
            .Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.AdditionalText_FileReadException);

        var stepOutput = stepResult.Outputs.Single();

        var result = (ContextClassInfoResult)stepOutput.Value;

        Assert.NotEqual(default, result.Model);
        Assert.Single(result.Diagnostics);
        Assert.Single(result.Diagnostics, x => x.Id == DiagnosticIds.AdditionalText_FileReadException);
    }

    [Fact]
    public void TargetClassDoesNotHaveIDataVariablesContextInterfaceTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal partial class TelemetryVariables { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var contextClassResultsStep = GeneratorAssert
            .TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults).Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustInheritIDataVariablesContextInterface);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        var contextClassResult = (ContextClassTargetResult)contextClassResultsValue.Value;

        Assert.Equal(default, contextClassResult.Model);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(contextClassResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustInheritIDataVariablesContextInterface);
    }

    [Fact]
    public void TargetClassIsNotPartialTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var contextClassResultsStep = GeneratorAssert
            .TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults).Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustBeDeclaredPartial);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassTargetResult)contextClassResultsValue.Value;

        Assert.Equal(default, contextClassResult.Model);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(contextClassResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustBeDeclaredPartial);
    }

    [Fact]
    public void TargetClassIsStaticTest()
    {
        var contextClassDefinition = @"using SharpRacer.Telemetry;
namespace Test.Assembly;
[GenerateDataVariablesContext(""TelemetryVariables_VariableNames.json"")]
internal static partial class TelemetryVariables : IDataVariablesContext { }";

        var variablesText = new VariableInfoDocumentBuilder()
            .AddScalar("SessionTime", VariableValueType.Double, "Seconds since session start", "s")
            .ToAdditionalTextFile(GeneratorConfigurationDefaults.VariableInfoFileName);

        var runResult = new VariablesGeneratorBuilder()
            .WithAdditionalText(variablesText)
            .WithCSharpSyntaxTree(contextClassDefinition)
            .Build()
            .RunGenerator();

        var contextClassResultsStep = GeneratorAssert
            .TrackedStepExecuted(runResult, TrackingNames.ContextClassInfoValuesProvider_GetContextClassResults).Single();

        GeneratorAssert.ContainsDiagnostic(runResult, DiagnosticIds.ContextClassMustNotBeDeclaredStatic);

        var contextClassResultsValue = contextClassResultsStep.Outputs.Single();
        Assert.Equal(IncrementalStepRunReason.New, contextClassResultsValue.Reason);

        var contextClassResult = (ContextClassTargetResult)contextClassResultsValue.Value;

        Assert.Equal(default, contextClassResult.Model);
        Assert.Single(contextClassResult.Diagnostics);
        Assert.Single(contextClassResult.Diagnostics, x => x.Id == DiagnosticIds.ContextClassMustNotBeDeclaredStatic);
    }
}
