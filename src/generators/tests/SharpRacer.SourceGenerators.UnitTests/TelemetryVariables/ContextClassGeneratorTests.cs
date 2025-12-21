using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

public class ContextClassGeneratorTests
{
    [Fact]
    public void Create_EnumTypeArg_Test()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Bitfield, 1, "Test variable", "irsdk_CameraState", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        // Create class models
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo1 = CreateContextClassInfo(className, classNamespace);

        var model1 = new ContextClassModel(classInfo1, [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_EnumTypeArg_Test_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_WithDescriptorReference_Test()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, descriptorRef);

        // Create class models
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo1 = CreateContextClassInfo(className, classNamespace);

        var model1 = new ContextClassModel(classInfo1, [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_WithDescriptorReference_Test_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_WithoutDescriptorReference_Test()
    {
        // Create context variable model
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "Test";
        var contextVariableModel = new ContextVariableModel(variableModel, propertyName, "This is the test variable.", null, null);

        // Create class models
        var className = "MyContext";
        var classNamespace = "TestAssembly.Variables";

        var classInfo1 = CreateContextClassInfo(className, classNamespace);

        var model1 = new ContextClassModel(classInfo1, [contextVariableModel]);

        var compilationUnit = ContextClassGenerator.Create(ref model1, default);
        Assert.NotNull(compilationUnit);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_WithoutDescriptorReference_Test_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_VariableClasses_Test()
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

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_VariableClasses_Test_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_VariableClasses_ContainsDeprecatedVariableTest()
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

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_VariableClasses_ContainsDeprecatedVariableTest_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingModelTest()
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

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingModelTest_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest()
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

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest_Source())
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

    private static string Create_EnumTypeArg_Test_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}

            Test = new global::SharpRacer.Telemetry.ScalarTelemetryVariable<global::SharpRacer.CameraState>(global::MyApp.Variables.VariableDescriptors.TestDescriptor, dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        public global::SharpRacer.Telemetry.IScalarTelemetryVariable<global::SharpRacer.CameraState> Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";

    private static string Create_WithDescriptorReference_Test_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}

            Test = new global::SharpRacer.Telemetry.ScalarTelemetryVariable<int>(global::MyApp.Variables.VariableDescriptors.TestDescriptor, dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        public global::SharpRacer.Telemetry.IScalarTelemetryVariable<int> Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";

    private static string Create_WithoutDescriptorReference_Test_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}

            Test = new global::SharpRacer.Telemetry.ScalarTelemetryVariable<int>(global::SharpRacer.Telemetry.TelemetryVariableDescriptor.CreateScalar<int>(""Test""), dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        public global::SharpRacer.Telemetry.IScalarTelemetryVariable<int> Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";

    private static string Create_VariableClasses_Test_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}

            Test = new global::MyApp.Variables.TestVariable(dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        public global::MyApp.Variables.TestVariable Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";

    private static string Create_VariableClasses_ContainsDeprecatedVariableTest_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}
            
            Test = new global::MyApp.Variables.TestVariable(dataVariableInfoProvider);
            TestEx = new global::MyApp.Variables.TestExVariable(dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        [System.ObsoleteAttribute(""Telemetry variable 'Test' is deprecated by variable 'TestEx'. Use context property 'TestEx' instead."")]
        public global::MyApp.Variables.TestVariable Test {{ get; }}
        /// <summary>
        /// This is the new and improved test variable.
        /// </summary>
        public global::MyApp.Variables.TestExVariable TestEx {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
            yield return TestEx;
        }}
    }}
}}";

    private static string Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingModelTest_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}
            
            Test = new global::MyApp.Variables.TestVariable(dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        [System.ObsoleteAttribute(""Telemetry variable 'Test' is deprecated by variable 'TestEx'."")]
        public global::MyApp.Variables.TestVariable Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";

    private static string Create_VariableClasses_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest_Source() =>
        $@"#nullable enable
namespace TestAssembly.Variables
{{
    partial class MyContext
    {{
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public MyContext(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider dataVariableInfoProvider)
        {{
            if (dataVariableInfoProvider is null)
            {{
                throw new System.ArgumentNullException(""dataVariableInfoProvider"");
            }}
            
            Test = new global::MyApp.Variables.TestVariable(dataVariableInfoProvider);
        }}

        /// <summary>
        /// This is the test variable.
        /// </summary>
        [System.ObsoleteAttribute(""Telemetry variable 'Test' is deprecated."")]
        public global::MyApp.Variables.TestVariable Test {{ get; }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public System.Collections.Generic.IEnumerable<global::SharpRacer.Telemetry.ITelemetryVariable> EnumerateVariables()
        {{
            yield return Test;
        }}
    }}
}}";
}
