using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

public class VariableClassGeneratorTests
{
    [Fact]
    public void ScalarVariableClass_NoDescriptorReferenceTest()
    {
        var variableInfo = new VariableInfo(
            "Test",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            false,
            false,
            null);

        var variableModel = new VariableModel(variableInfo, default);

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            null,
            false,
            true);

        var compilationUnit = VariableClassGenerator.Create(ref classModel, default);
        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(ScalarVariableClass_NoDescriptorReference_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void ScalarVariableClass_WithDescriptorReferenceTest()
    {
        var variableInfo = new VariableInfo(
            "Test",
            VariableValueType.Int,
            1,
            "Test variable",
            "test/s",
            false,
            false,
            null);

        var variableModel = new VariableModel(variableInfo, default);

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "MyApp.Variables");

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        var compilationUnit = VariableClassGenerator.Create(ref classModel, default);
        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(ScalarVariableClass_WithDescriptorReference_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    private static string ScalarVariableClass_NoDescriptorReference_Source() =>
        $@"#nullable enable
namespace TestApp.Variables
{{
    public partial class TestVariable : global::SharpRacer.Telemetry.ScalarTelemetryVariable<int>
    {{
        private static readonly global::SharpRacer.Telemetry.TelemetryVariableDescriptor _Descriptor = new global::SharpRacer.Telemetry.TelemetryVariableDescriptor(""Test"", global::SharpRacer.Telemetry.TelemetryVariableValueType.Int, 1);
        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""global::SharpRacer.Telemetry.TelemetryVariableInfo""/>.
        /// </summary>
        /// <exception cref = ""SharpRacer.Telemetry.TelemetryVariableInitializationException"">
        /// <paramref name = ""variableInfo""/> is not compatible with the telemetry variable represented by this instance.
        /// </exception>
        /// <remarks>
        /// If <paramref name = ""variableInfo""/> is <see langword=""null""/>, the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(global::SharpRacer.Telemetry.TelemetryVariableInfo? variableInfo) : base(_Descriptor, variableInfo)
        {{
        }}

        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider""/>.
        /// </summary>
        /// <param name = ""variableInfoProvider"">
        /// The <see cref = ""global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider""/> instance used to perform delayed initialization of <see cref = ""TestVariable""/> when the associated telemetry variable is activated by the data source.
        /// </param>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider variableInfoProvider) : base(_Descriptor, variableInfoProvider)
        {{
        }}
    }}
}}";

    private static string ScalarVariableClass_WithDescriptorReference_Source() =>
        $@"#nullable enable
namespace TestApp.Variables
{{
    public partial class TestVariable : global::SharpRacer.Telemetry.ScalarTelemetryVariable<int>
    {{
        private static readonly global::SharpRacer.Telemetry.TelemetryVariableDescriptor _Descriptor = global::MyApp.Variables.VariableDescriptors.TestDescriptor;
        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""global::SharpRacer.Telemetry.TelemetryVariableInfo""/>.
        /// </summary>
        /// <exception cref = ""SharpRacer.Telemetry.TelemetryVariableInitializationException"">
        /// <paramref name = ""variableInfo""/> is not compatible with the telemetry variable represented by this instance.
        /// </exception>
        /// <remarks>
        /// If <paramref name = ""variableInfo""/> is <see langword=""null""/>, the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(global::SharpRacer.Telemetry.TelemetryVariableInfo? variableInfo) : base(_Descriptor, variableInfo)
        {{
        }}

        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider""/>.
        /// </summary>
        /// <param name = ""variableInfoProvider"">
        /// The <see cref = ""global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider""/> instance used to perform delayed initialization of <see cref = ""TestVariable""/> when the associated telemetry variable is activated by the data source.
        /// </param>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(global::SharpRacer.Telemetry.ITelemetryVariableInfoProvider variableInfoProvider) : base(_Descriptor, variableInfoProvider)
        {{
        }}
    }}
}}";
}
