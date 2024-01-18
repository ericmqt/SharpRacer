namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
public class VariableClassReferenceTests
{
    [Fact]
    public void Ctor_Test()
    {
        var variableName = "SessionTime";
        var className = "SessionTimeVariable";
        var classNamespace = "Test.App.Variables";

        var classRef = new VariableClassReference(variableName, className, classNamespace);

        Assert.Equal(variableName, classRef.VariableName);
        Assert.Equal(className, classRef.ClassName);
        Assert.Equal(classNamespace, classRef.ClassNamespace);
    }

    [Fact]
    public void Ctor_ThrowOnNullOrEmptyStringArgsTest()
    {
        Assert.Throws<ArgumentException>(() => new VariableClassReference(null!, "MyClass", "Test.App"));
        Assert.Throws<ArgumentException>(() => new VariableClassReference("SessionTime", null!, "Test.App"));
        Assert.Throws<ArgumentException>(() => new VariableClassReference("SessionTime", "MyClass", null!));
    }

    [Fact]
    public void GlobalQualifiedTypeName_Test()
    {
        var className = "SessionTimeVariable";
        var classNamespace = "Test.App.Variables";

        var classRef = new VariableClassReference("SessionTime", className, classNamespace);

        Assert.Equal($"global::{classNamespace}.{className}", classRef.GlobalQualifiedTypeName().ToFullString());
    }

    [Fact]
    public void Equals_IdenticalValueTest()
    {
        var classRef1 = new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables");
        var classRef2 = new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables");

        EquatableStructAssert.Equal(classRef1, classRef2);
    }

    [Theory]
    [MemberData(nameof(GetInequalityData))]
    public void Equals_InequalityTest(VariableClassReference model1, VariableClassReference model2)
    {
        EquatableStructAssert.NotEqual(model1, model2);
    }

    [Fact]
    public void Equals_WrongTypeObjectTest()
    {
        var classRef = new VariableClassReference("SessionTime", "SessionTimeVariable", "Test.App.Variables");

        EquatableStructAssert.ObjectEqualsMethod(false, classRef, int.MaxValue);
    }

    public static TheoryData<VariableClassReference, VariableClassReference> GetInequalityData()
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
}
