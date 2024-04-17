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
    }

    [Fact]
    public void Equals_DefaultValueTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";

        var model1 = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        EquatableStructAssert.NotEqual(model1, default);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(DescriptorPropertyModel model1, DescriptorPropertyModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void EqualsObject_WrongObjectTypeTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";

        var model1 = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);
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

    public static TheoryData<DescriptorPropertyModel, DescriptorPropertyModel> GetInequalityData()
    {
        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var variableInfo2 = new VariableInfo("TestEx", VariableValueType.Float, 1, "New and improved test variable", "test/s", false, false, null);
        var variableModel2 = new VariableModel(variableInfo2, default);

        return new TheoryData<DescriptorPropertyModel, DescriptorPropertyModel>()
        {
            // Property name
            {
                new DescriptorPropertyModel("TestProperty1", "This is the test property", variableModel1),
                new DescriptorPropertyModel("TestProperty2", "This is the test property", variableModel1)
            },

            // Property XML summary
            {
                new DescriptorPropertyModel("TestProperty", "This is the first test property", variableModel1),
                new DescriptorPropertyModel("TestProperty", "This is the second test property", variableModel1)
            },

            // VariableModel
            {
                new DescriptorPropertyModel("TestProperty", "This is the test property", variableModel1),
                new DescriptorPropertyModel("TestProperty", "This is the test property", variableModel2)
            }
        };
    }
}
