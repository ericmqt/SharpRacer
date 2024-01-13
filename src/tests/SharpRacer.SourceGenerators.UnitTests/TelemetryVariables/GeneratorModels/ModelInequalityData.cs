using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public static class ModelInequalityData
{
    public static TheoryData<ContextClassModel, ContextClassModel> ContextClassModelData()
    {
        var data = new TheoryData<ContextClassModel, ContextClassModel>()
        {
            // Type name
            {
                new ContextClassModel("MyContext", "Test.App.Contexts", []),
                new ContextClassModel("MyContext2", "Test.App.Contexts", [])
            },

            // Type namespace
            {
                new ContextClassModel("MyContext", "Test.App.Contexts", []),
                new ContextClassModel("MyContext", "Test.App.Variables.Contexts", [])
            }
        };

        // Different variables
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        data.Add(
            new ContextClassModel("MyContext", "Test.App.Contexts", []),
            new ContextClassModel("MyContext", "Test.App.Contexts", [contextVariableModel]));

        return data;
    }

    public static TheoryData<ContextVariableModel, ContextVariableModel> ContextVariableModelData()
    {
        var data = new TheoryData<ContextVariableModel, ContextVariableModel>();

        var variableInfo1 = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel1 = new VariableModel(variableInfo1, default);

        var variableInfo2 = new VariableInfo("Test2", VariableValueType.Float, 3, "Test variable", "test/s", false, false, null);
        var variableModel2 = new VariableModel(variableInfo2, default);

        var propertyXmlSummary = "This is the test variable.";

        // Variable models
        data.Add(
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null),
            new ContextVariableModel(variableModel2, "Test", propertyXmlSummary, null, null));

        // Property names
        data.Add(
            new ContextVariableModel(variableModel1, "Test1", propertyXmlSummary, null, null),
            new ContextVariableModel(variableModel1, "Test2", propertyXmlSummary, null, null));

        // Property XML summaries
        data.Add(
            new ContextVariableModel(variableModel1, "Test", "this is test1", null, null),
            new ContextVariableModel(variableModel1, "Test", "this is test2", null, null));

        // Class refs
        var classRef1 = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var classRef2 = new VariableClassReference("Test2", "TestVariable", "MyApp.Variables");

        data.Add(
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, null),
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef2, null));

        // Class refs, one is null
        data.Add(
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, null),
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null));

        // Descriptor refs, null class refs
        var descriptorRef1 = new DescriptorPropertyReference("Test1", "TestDescriptor1", "VariableDescriptors", "MyApp.Variables");
        var descriptorRef2 = new DescriptorPropertyReference("Test2", "TestDescriptor2", "VariableDescriptors", "MyApp.Variables");

        data.Add(
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef1),
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef2));

        // Descriptor refs, one is null
        data.Add(
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, descriptorRef1),
            new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, null, null));

        // Descriptor refs, same class refs
        data.Add(
           new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, descriptorRef1),
           new ContextVariableModel(variableModel1, "Test", propertyXmlSummary, classRef1, descriptorRef2));

        return data;
    }

    public static TheoryData<VariableClassModel, VariableClassModel> VariableClassModelData()
    {
        var data = new TheoryData<VariableClassModel, VariableClassModel>
        {
            // Class name
            { CreateVariableClassModel(className: "Test"), CreateVariableClassModel(className: "Test2") },

            // Namespace
            { CreateVariableClassModel(classNamespace: "Test.App"), CreateVariableClassModel(classNamespace: "Test.App.Variables") },

            // Variable name
            { CreateVariableClassModel(variableName: "Test"), CreateVariableClassModel(variableName: "Test2") },

            // Value type
            { CreateVariableClassModel(valueType: VariableValueType.Float), CreateVariableClassModel(valueType: VariableValueType.Double) },

            // Value count
            { CreateVariableClassModel(valueCount: 1), CreateVariableClassModel(valueCount: 2) },

            // Value unit
            { CreateVariableClassModel(valueUnit: "s"), CreateVariableClassModel(valueUnit: "test/s") },

            // IsClassInternal
            { CreateVariableClassModel(isClassInternal: true), CreateVariableClassModel(isClassInternal: false) },

            // IsClassPartial
            { CreateVariableClassModel(isClassPartial: true), CreateVariableClassModel(isClassPartial: false) },

            // Non-null descriptor property references
            {
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty", "MyDescriptors", "Test.App")),
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty2", "MyDescriptors", "Test.App"))
            },

            // One null descriptor property reference
            {
                CreateVariableClassModel(descriptorPropertyReferenceFactory: x=> new DescriptorPropertyReference(x, "TestProperty", "MyDescriptors", "Test.App")),
                CreateVariableClassModel(descriptorPropertyReferenceFactory: _ => null)
            }
        };

        return data;
    }

    public static TheoryData<VariableClassReference, VariableClassReference> VariableClassReferenceData()
    {
        return new TheoryData<VariableClassReference, VariableClassReference>()
        {
            // Variable name
            {
                new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables"),
                new VariableClassReference("SessionTick", "SessionTimeVariable", "Test.App.Variables")
            },

            // Class name
            {
                new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables"),
                new VariableClassReference("SessionTime", "TimeVariable", "Test.App.Variables")
            },

            // Class namespace
            {
                new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables"),
                new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Telemetry.Variables")
            }
        };
    }

    public static TheoryData<VariableModel, VariableModel> VariableModelData()
    {
        var data = new TheoryData<VariableModel, VariableModel>();

        var variable1 = new VariableInfo(
            "Lat",
            VariableValueType.Int,
            4,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variable2 = new VariableInfo(
            "Lon",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            true,
            false,
            null);

        var variableOptions1 = new VariableOptions("Lat", "Latitude", "Latitude");
        var variableOptions2 = new VariableOptions("Lon", "Longitude", "Longitude");

        // Different variables, default options
        data.Add(new VariableModel(variable1, default), new VariableModel(variable2, default));

        // Same variables, one with option
        data.Add(new VariableModel(variable1, default), new VariableModel(variable1, variableOptions1));

        // Same variables, different non-default options
        data.Add(new VariableModel(variable1, variableOptions1), new VariableModel(variable1, variableOptions2));

        return data;
    }

    private static VariableClassModel CreateVariableClassModel(
        string className = "TestClass",
        string classNamespace = "Test.App.Variables",
        string variableName = "Test",
        VariableValueType valueType = VariableValueType.Int,
        int valueCount = 1,
        string? valueUnit = null,
        Func<string, DescriptorPropertyReference?>? descriptorPropertyReferenceFactory = null,
        bool isClassInternal = false,
        bool isClassPartial = true)
    {
        var variableInfo = new VariableInfo(variableName, valueType, valueCount, "test", valueUnit, false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        DescriptorPropertyReference? descriptorPropertyReference = null;

        descriptorPropertyReference = descriptorPropertyReferenceFactory?.Invoke(variableName);

        return new VariableClassModel(className, classNamespace, variableModel, descriptorPropertyReference, isClassInternal, isClassPartial);
    }
}
