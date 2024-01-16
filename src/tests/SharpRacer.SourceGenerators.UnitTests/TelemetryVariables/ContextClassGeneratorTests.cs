using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class ContextClassGeneratorTests
{
    [Fact]
    public void Create_Test()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var classRef = new VariableClassReference("Test", "TestVariable", "MyApp.Variables");
        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", classRef, descriptorRef);

        // Create class models
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo1 = CreateContextClassInfo(className, classNamespace);

        var model1 = new ContextClassModel(classInfo1, [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_Test_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_ContainsDeprecatedVariableTest()
    {
        // Create context variable models
        var testVariableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, true, "TestEx");
        var testVariableModel = new VariableModel(testVariableInfo, default);

        var testContextVariableModel = new ContextVariableModel(
            testVariableModel,
            "Test",
            "This is the test variable.",
            new VariableClassReference("Test", "TestVariable", "MyApp.Variables"),
            new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables"));

        var testExVariableInfo = new VariableInfo("TestEx", VariableValueType.Int, 1, "New and improved test variable!", "test/s", false, false, null);
        var testExVariableModel = new VariableModel(testExVariableInfo, default);

        var testExContextVariableModel = new ContextVariableModel(
            testExVariableModel,
            "TestEx",
            "This is the new and improved test variable.",
            new VariableClassReference("TestEx", "TestExVariable", "MyApp.Variables"),
            new DescriptorPropertyReference("TestEx", "TestExDescriptor", "VariableDescriptors", "MyApp.Variables"));



        // Create class model
        var model1 = new ContextClassModel(
            CreateContextClassInfo("MyContext", "TestAssembly.Variables"), [testContextVariableModel, testExContextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_ContainsDeprecatedVariableTest_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_ContainsDeprecatedVariableWithoutDeprecatingModelTest()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, true, "TestEx");
        var variableModel = new VariableModel(variableInfo, default);

        var contextVariableModel = new ContextVariableModel(
            variableModel,
            "Test",
            "This is the test variable.",
            new VariableClassReference("Test", "TestVariable", "MyApp.Variables"),
            new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables"));

        // Create class model
        var model1 = new ContextClassModel(CreateContextClassInfo("MyContext", "TestAssembly.Variables"), [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_ContainsDeprecatedVariableWithoutDeprecatingModelTest_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, true, null);
        var variableModel = new VariableModel(variableInfo, default);

        var contextVariableModel = new ContextVariableModel(
            variableModel,
            "Test",
            "This is the test variable.",
            new VariableClassReference("Test", "TestVariable", "MyApp.Variables"),
            new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables"));

        // Create class model
        var model1 = new ContextClassModel(CreateContextClassInfo("MyContext", "TestAssembly.Variables"), [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    private static ContextClassInfo CreateContextClassInfo(string className, string classNamespace)
    {
        var classTypeSymbol = new Mock<INamedTypeSymbol>(MockBehavior.Strict);
        var classContainingNamespaceSymbol = new Mock<INamespaceSymbol>();

        classContainingNamespaceSymbol.Setup(x => x.ToString())
            .Returns(classNamespace);

        classTypeSymbol.SetupGet(x => x.Name)
            .Returns(className);

        classTypeSymbol.SetupGet(x => x.ContainingNamespace)
            .Returns(classContainingNamespaceSymbol.Object);

        return new ContextClassInfo(classTypeSymbol.Object, Location.None);
    }

    private static string Create_Test_Source() =>
        @"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestAssembly.Variables
{
    partial class MyContext
    {
        public MyContext(IDataVariableInfoProvider dataVariableProvider)
        {
            if (dataVariableProvider is null)
            {
                throw new ArgumentNullException(""dataVariableProvider"");
            }

            var factory = new DataVariableFactory(dataVariableProvider);
            Test = factory.CreateType<global::MyApp.Variables.TestVariable>(global::MyApp.Variables.VariableDescriptors.TestDescriptor);
        }

        public global::MyApp.Variables.TestVariable Test { get; }

        /// <inheritdoc/>
        public IEnumerable<IDataVariable> EnumerateVariables()
        {
            yield return Test;
        }
    }
}";

    private static string Create_ContainsDeprecatedVariableTest_Source() =>
        @"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestAssembly.Variables
{
    partial class MyContext
    {
        public MyContext(IDataVariableInfoProvider dataVariableProvider)
        {
            if (dataVariableProvider is null)
            {
                throw new ArgumentNullException(""dataVariableProvider"");
            }

            var factory = new DataVariableFactory(dataVariableProvider);
            Test = factory.CreateType<global::MyApp.Variables.TestVariable>(global::MyApp.Variables.VariableDescriptors.TestDescriptor);
            TestEx = factory.CreateType<global::MyApp.Variables.TestExVariable>(global::MyApp.Variables.VariableDescriptors.TestExDescriptor);
        }

        [Obsolete(""Telemetry variable 'Test' is deprecated by variable 'TestEx'. Use context property 'TestEx' instead."")]
        public global::MyApp.Variables.TestVariable Test { get; }
        public global::MyApp.Variables.TestExVariable TestEx { get; }

        /// <inheritdoc/>
        public IEnumerable<IDataVariable> EnumerateVariables()
        {
            yield return Test;
            yield return TestEx;
        }
    }
}";

    private static string Create_ContainsDeprecatedVariableWithoutDeprecatingModelTest_Source() =>
        @"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestAssembly.Variables
{
    partial class MyContext
    {
        public MyContext(IDataVariableInfoProvider dataVariableProvider)
        {
            if (dataVariableProvider is null)
            {
                throw new ArgumentNullException(""dataVariableProvider"");
            }

            var factory = new DataVariableFactory(dataVariableProvider);
            Test = factory.CreateType<global::MyApp.Variables.TestVariable>(global::MyApp.Variables.VariableDescriptors.TestDescriptor);
        }

        [Obsolete(""Telemetry variable 'Test' is deprecated by variable 'TestEx'."")]
        public global::MyApp.Variables.TestVariable Test { get; }

        /// <inheritdoc/>
        public IEnumerable<IDataVariable> EnumerateVariables()
        {
            yield return Test;
        }
    }
}";

    private static string Create_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest_Source() =>
        @"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestAssembly.Variables
{
    partial class MyContext
    {
        public MyContext(IDataVariableInfoProvider dataVariableProvider)
        {
            if (dataVariableProvider is null)
            {
                throw new ArgumentNullException(""dataVariableProvider"");
            }

            var factory = new DataVariableFactory(dataVariableProvider);
            Test = factory.CreateType<global::MyApp.Variables.TestVariable>(global::MyApp.Variables.VariableDescriptors.TestDescriptor);
        }

        [Obsolete(""Telemetry variable 'Test' is deprecated."")]
        public global::MyApp.Variables.TestVariable Test { get; }

        /// <inheritdoc/>
        public IEnumerable<IDataVariable> EnumerateVariables()
        {
            yield return Test;
        }
    }
}";
}
