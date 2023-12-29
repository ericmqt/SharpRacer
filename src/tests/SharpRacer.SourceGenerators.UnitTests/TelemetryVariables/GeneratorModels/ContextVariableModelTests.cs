using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class ContextVariableModelTests
{
    [Fact]
    public void Ctor_ScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var propertyXmlSummary = "This is the test variable.";
        var model = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, descriptorRef);

        Assert.Equal(descriptorRef, model.DescriptorReference);
        Assert.False(model.IsArray);
        Assert.Equal(propertyName, model.PropertyName);
        Assert.Equal(propertyXmlSummary, model.PropertyXmlSummary);
        Assert.Equal(classRef, model.VariableClassReference);
        Assert.Equal(variableModel, model.VariableModel);
    }
}
