using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class DescriptorPropertyModelTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var model = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        Assert.Equal(propertyName, model.PropertyName);
        Assert.Equal(propertyXmlSummary, model.PropertyXmlSummary);
        Assert.Equal(variableModel, model.VariableModel);
        Assert.Equal(variableModel.VariableName, model.VariableName);
    }

    [Fact]
    public void Ctor_ThrowsOnDefaultVariableModelValueTest()
    {
        Assert.Throws<ArgumentException>(() => new DescriptorPropertyModel("Test", "test summary", variableModel: default));
    }

    [Fact]
    public void Equals_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";

        var model1 = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);
        var model2 = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        EquatableStructAssert.Equal(model1, model2);
        EquatableStructAssert.NotEqual(model1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, model1, variableInfo);
    }

    [Fact]
    public void PropertyIdentifier_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var model = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        Assert.Equal(propertyName, model.PropertyIdentifier().ValueText);
    }

    [Fact]
    public void PropertyIdentifierName_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var model = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        Assert.Equal(propertyName, model.PropertyIdentifierName().ToFullString());
    }
}
