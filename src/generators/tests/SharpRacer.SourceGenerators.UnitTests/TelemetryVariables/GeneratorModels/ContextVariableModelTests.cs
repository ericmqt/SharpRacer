using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class ContextVariableModelTests
{
    public static TheoryData<ContextVariableModel, ContextVariableModel> InequalityData => GetInequalityData();

    [Fact]
    public void Ctor_ArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var propertyXmlSummary = "This is the test variable.";
        var model = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, descriptorRef);

        Assert.Equal(descriptorRef, model.DescriptorReference);
        Assert.True(model.IsArray);
        Assert.Equal(propertyName, model.PropertyName);
        Assert.Equal(propertyXmlSummary, model.PropertyXmlSummary);
        Assert.Equal(classRef, model.VariableClassReference);
        Assert.Equal(variableModel, model.VariableModel);
    }

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

    [Fact]
    public void Equals_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var propertyXmlSummary = "This is the test variable.";

        var model1 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, descriptorRef);
        var model2 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, descriptorRef);

        EquatableStructAssert.Equal(model1, model2);
        EquatableStructAssert.NotEqual(model1, default);
        EquatableStructAssert.ObjectEqualsMethod(false, model1, int.MaxValue);

        model1 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, null, null);
        model2 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, null, null);

        EquatableStructAssert.Equal(model1, model2);

        model1 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, null);
        model2 = new ContextVariableModel(variableModel, propertyName, propertyXmlSummary, classRef, null);

        EquatableStructAssert.Equal(model1, model2);
    }

    [Theory]
    [MemberData(nameof(InequalityData))]
    public void Equals_InequalityTest(ContextVariableModel model1, ContextVariableModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void PropertyIdentifier_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var propertyIdentifier = model.PropertyIdentifier();

        Assert.Equal(propertyName, propertyIdentifier.ValueText);
    }

    [Fact]
    public void PropertyIdentifierName_Test()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var propertyIdentifier = model.PropertyIdentifierName();

        Assert.Equal(propertyName, propertyIdentifier.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_CreateArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, null);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::SharpRacer.Telemetry.ArrayDataVariable<int>(global::SharpRacer.Telemetry.DataVariableDescriptor.CreateArray<int>(\"Test\", 3), dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_CreateScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, null);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::SharpRacer.Telemetry.ScalarDataVariable<int>(global::SharpRacer.Telemetry.DataVariableDescriptor.CreateScalar<int>(\"Test\"), dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_DescriptorReference_CreateArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::SharpRacer.Telemetry.ArrayDataVariable<int>(global::MyApp.Variables.VariableDescriptors.TestDescriptor, dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_DescriptorReference_CreateScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::SharpRacer.Telemetry.ScalarDataVariable<int>(global::MyApp.Variables.VariableDescriptors.TestDescriptor, dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_VariableClassTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, null);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::MyApp.Variables.TestVariable(dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyObjectCreationExpression_VariableClassWithDescriptorRefTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        var expr = model.PropertyObjectCreationExpression(SyntaxFactory.IdentifierName("dataVariableInfoProvider"));

        expr = expr.NormalizeWhitespace();

        Assert.Equal(
            "new global::MyApp.Variables.TestVariable(dataVariableInfoProvider)",
            expr.ToFullString());
    }

    [Fact]
    public void PropertyType_ArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var propertyType = model.PropertyType();

        Assert.Equal("IArrayDataVariable<int>", propertyType.ToFullString());
    }

    [Fact]
    public void PropertyType_ScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var propertyType = model.PropertyType();

        Assert.Equal("IScalarDataVariable<int>", propertyType.ToFullString());
    }

    [Fact]
    public void PropertyType_VariableClassTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        var propertyType = model.PropertyType();

        Assert.Equal($"global::{classRef.ClassNamespace}.{classRef.ClassName}", propertyType.ToFullString());
    }

    private static TheoryData<ContextVariableModel, ContextVariableModel> GetInequalityData()
    {
        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var variableInfo2 = new VariableInfo("Test2", VariableValueType.Float, 3, "Test variable", "test/s", false, false, null);
        var variableModel2 = new VariableModel(variableInfo2, default);

        var classRef1 = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var classRef2 = new VariableClassReference("Test2", "TestVariable", "MyApp.Variables");

        var descriptorRef1 = new DescriptorPropertyReference("Test1", "TestDescriptor1", "VariableDescriptors", "MyApp.Variables");
        var descriptorRef2 = new DescriptorPropertyReference("Test2", "TestDescriptor2", "VariableDescriptors", "MyApp.Variables");

        var propertyXmlSummary = "This is the test variable.";

        return new TheoryData<ContextVariableModel, ContextVariableModel>
        {
            // Variable models
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null),
                new ContextVariableModel(variableModel2, "Test", propertyXmlSummary, null, null)
            },

            // Property names
            {
                new ContextVariableModel(variableModel1, "Test1", propertyXmlSummary, null, null),
                new ContextVariableModel(variableModel1, "Test2", propertyXmlSummary, null, null)
            },

            // Property XML summaries
            {
                new ContextVariableModel(variableModel1, "Test", "this is test1", null, null),
                new ContextVariableModel(variableModel1, "Test", "this is test2", null, null)
            },

            // Class refs
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, null),
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef2, null)
            },

            // Class refs, one is null
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, null),
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null)
            },

            // Descriptor refs, null class refs
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef1),
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef2)
            },

            // Descriptor refs, one is null
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef1),
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null)
            },

            // Descriptor refs, same class refs
            {
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, descriptorRef1),
                new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, descriptorRef2)
            }
        };
    }
}
