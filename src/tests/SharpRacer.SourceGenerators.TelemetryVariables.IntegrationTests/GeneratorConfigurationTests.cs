using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Pipeline;
using SharpRacer.SourceGenerators.TelemetryVariables.TestHelpers;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class GeneratorConfigurationTests
{
    [Fact]
    public void Default_Test()
    {
        var testModel = new VariablesGeneratorBuilder().Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);

        var runResult = driver.GetRunResult().Results.Single();

        var configResult = runResult.TrackedSteps[TrackingNames.GeneratorConfigurationValueProvider_GetValueProvider].Single();

        Assert.NotNull(configResult);

        var configStepOutput = configResult.Outputs.Single();
        var generatorConfig = (GeneratorConfiguration)configStepOutput.Value;

        Assert.True(configStepOutput.Reason == IncrementalStepRunReason.New);
        Assert.Equal(GeneratorConfigurationDefaults.GenerateTypedVariableClasses, generatorConfig.GenerateVariableClasses);
        Assert.Equal(GeneratorConfigurationDefaults.TelemetryVariableClassesNamespace, generatorConfig.VariableClassesNamespace);
        Assert.Equal(GeneratorConfigurationDefaults.VariableInfoFileName, generatorConfig.VariableInfoFileName);
        Assert.Equal(GeneratorConfigurationDefaults.VariableOptionsFileName, generatorConfig.VariableOptionsFileName);
    }

    [Fact]
    public void Default_MutateRootNamespaceTest()
    {
        var testModel = new VariablesGeneratorBuilder().Build();

        var driver = testModel.GeneratorDriver.RunGenerators(testModel.Compilation);

        var runResult = driver.GetRunResult().Results.Single();

        var configResult = runResult.TrackedSteps[TrackingNames.GeneratorConfigurationValueProvider_GetValueProvider].Single();

        Assert.NotNull(configResult);

        var configStepOutput = configResult.Outputs.Single();
        var generatorConfig = (GeneratorConfiguration)configStepOutput.Value;

        Assert.True(configStepOutput.Reason == IncrementalStepRunReason.New);

        // Mutate
        var newOptions = testModel.OptionsProvider.Mutate(options =>
        {
            options.RootNamespace = "MyTestAssembly.Generated";
        });

        driver = driver.WithUpdatedAnalyzerConfigOptions(newOptions)
            .RunGenerators(testModel.Compilation);

        var mutateResult = driver.GetRunResult().Results.Single();
        var mutatedConfigResult = mutateResult.TrackedSteps[TrackingNames.GeneratorConfigurationValueProvider_GetValueProvider].Single();
        var mutatedConfigResultOutput = mutatedConfigResult.Outputs.Single();
        var mutatedConfig = (GeneratorConfiguration)mutatedConfigResultOutput.Value;

        Assert.Equal(IncrementalStepRunReason.Modified, mutatedConfigResultOutput.Reason);

        Assert.Equal(generatorConfig.GenerateVariableClasses, mutatedConfig.GenerateVariableClasses);
        Assert.Equal("MyTestAssembly.Generated", mutatedConfig.VariableClassesNamespace);
        Assert.Equal(generatorConfig.VariableInfoFileName, mutatedConfig.VariableInfoFileName);
        Assert.Equal(generatorConfig.VariableOptionsFileName, mutatedConfig.VariableOptionsFileName);
    }
}
