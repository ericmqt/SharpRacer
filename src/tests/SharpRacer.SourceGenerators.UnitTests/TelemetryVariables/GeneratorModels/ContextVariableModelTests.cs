using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class ContextVariableModelTests
{
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
    public void DataVariableFactoryCreateMethodInvocation_CreateArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, null);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        // Add spacing
        methodExpr = methodExpr.NormalizeWhitespace();

        Assert.Equal("factory.CreateArray<int>(\"Test\", 3)", methodExpr.ToFullString());
    }

    [Fact]
    public void DataVariableFactoryCreateMethodInvocation_CreateScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, null);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        Assert.Equal("factory.CreateScalar<int>(\"Test\")", methodExpr.ToFullString());
    }

    [Fact]
    public void DataVariableFactoryCreateMethodInvocation_DescriptorReference_CreateArrayTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 3, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        Assert.Equal(
            "factory.CreateArray<int>(global::MyApp.Variables.VariableDescriptors.TestDescriptor)",
            methodExpr.ToFullString());
    }

    [Fact]
    public void DataVariableFactoryCreateMethodInvocation_DescriptorReference_CreateScalarTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        Assert.Equal(
            "factory.CreateScalar<int>(global::MyApp.Variables.VariableDescriptors.TestDescriptor)",
            methodExpr.ToFullString());
    }

    [Fact]
    public void DataVariableFactoryCreateMethodInvocation_VariableClassTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, null);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        Assert.Equal(
            "factory.CreateType<global::MyApp.Variables.TestVariable>(\"Test\")",
            methodExpr.ToFullString());
    }

    [Fact]
    public void DataVariableFactoryCreateMethodInvocation_VariableClassWithDescriptorRefTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var model = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        var methodExpr = model.DataVariableFactoryCreateMethodInvocation(SyntaxFactory.IdentifierName("factory"));

        Assert.Equal(
            "factory.CreateType<global::MyApp.Variables.TestVariable>(global::MyApp.Variables.VariableDescriptors.TestDescriptor)",
            methodExpr.ToFullString());
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
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(ContextVariableModel model1, ContextVariableModel model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    /*[Fact]
    public void Equals_DifferentValuesTest()
    {
        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var propertyXmlSummary = "This is the test variable.";

        // Different property names
        var model1 = new ContextVariableModel(variableModel1, "Test1", propertyXmlSummary, null, null);
        var model2 = new ContextVariableModel(variableModel1, "Test2", propertyXmlSummary, null, null);

        EquatableStructAssert.NotEqual(model1, model2);

        // Different property XML summaries
        model1 = new ContextVariableModel(variableModel1, "Test1", "this is test1", null, null);
        model2 = new ContextVariableModel(variableModel1, "Test1", "this is test2", null, null);

        EquatableStructAssert.NotEqual(model1, model2);

        // Different class refs
        var classRef1 = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var classRef2 = new VariableClassReference("Test2", "TestVariable", "MyApp.Variables");

        model1 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef1, null);
        model2 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef2, null);

        EquatableStructAssert.NotEqual(model1, model2);

        // Same class refs
        model1 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef1, null);
        model2 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef1, null);

        EquatableStructAssert.Equal(model1, model2);

        // Different descriptor refs
        var descriptorRef1 = new DescriptorPropertyReference("Test1", "TestDescriptor1", "VariableDescriptors", "MyApp.Variables");
        var descriptorRef2 = new DescriptorPropertyReference("Test2", "TestDescriptor2", "VariableDescriptors", "MyApp.Variables");

        model1 = new ContextVariableModel(variableModel1, "Test1", "this is test1", null, descriptorRef1);
        model2 = new ContextVariableModel(variableModel1, "Test1", "this is test1", null, descriptorRef2);

        EquatableStructAssert.NotEqual(model1, model2);

        // Same class refs, different descriptor refs
        model1 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef1, descriptorRef1);
        model2 = new ContextVariableModel(variableModel1, "Test1", "this is test1", classRef1, descriptorRef2);

        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void Equals_DifferentVariableModelsTest()
    {
        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var variableInfo2 = new VariableInfo("Test2", VariableValueType.Float, 3, "Test variable", "test/s", false, false, null);
        var variableModel2 = new VariableModel(variableInfo2, default);

        var propertyName = "Test";
        var propertyXmlSummary = "This is the test variable.";

        var model1 = new ContextVariableModel(variableModel1, propertyName, propertyXmlSummary, null, null);
        var model2 = new ContextVariableModel(variableModel2, propertyName, propertyXmlSummary, null, null);

        EquatableStructAssert.NotEqual(model1, model2);
    }*/

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

    public static TheoryData<ContextVariableModel, ContextVariableModel> GetInequalityData()
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
