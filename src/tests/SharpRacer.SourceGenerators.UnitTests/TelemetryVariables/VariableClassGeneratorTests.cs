using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
public class VariableClassGeneratorTests
{
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

    [Fact]
    public void ScalarVariableClass_WithDescriptorReferenceInSameNamespaceTest()
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

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "TestApp.Variables");

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        var compilationUnit = VariableClassGenerator.Create(ref classModel, default);
        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(ScalarVariableClass_WithDescriptorReferenceInSameNamespace_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void ScalarVariableClass_WithDescriptorReferenceInSharpRacerTelemetryVariablesNamespaceTest()
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

        var descriptorRef = new DescriptorPropertyReference("Test", "TestDescriptor", "VariableDescriptors", "SharpRacer.Telemetry.Variables");

        var classModel = new VariableClassModel(
            "TestVariable",
            "TestApp.Variables",
            variableModel,
            descriptorRef,
            false,
            true);

        var compilationUnit = VariableClassGenerator.Create(ref classModel, default);
        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(ScalarVariableClass_WithDescriptorReferenceInSharpRacerTelemetryVariablesNamespace_Source())
            .GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    private static string ScalarVariableClass_WithDescriptorReference_Source() =>
        $@"using MyApp.Variables;
using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestApp.Variables
{{
    public partial class TestVariable : ScalarDataVariable<int>
    {{
        private static readonly DataVariableDescriptor _Descriptor = global::MyApp.Variables.VariableDescriptors.TestDescriptor;
        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> configured as a placeholder for the underlying telemetry variable which is unavailable in the current context.
        /// </summary>
        /// <remarks>
        /// The returned instance cannot be used to read data because the value for property <see cref = ""IsAvailable""/> is <see langword=""false""/>.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable() : base(_Descriptor, variableInfo: null)
        {{
        }}

        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""DataVariableInfo""/>.
        /// </summary>
        /// <exception cref = ""SharpRacer.Telemetry.Variables.DataVariableInitializationException"">
        /// <paramref name = ""dataVariableInfo""/> is not compatible with the telemetry variable represented by this instance.
        /// </exception>
        /// <remarks>
        /// If <paramref name = ""dataVariableInfo""/> is <see langword=""null""/>, the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(DataVariableInfo? dataVariableInfo) : base(_Descriptor, dataVariableInfo)
        {{
        }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static TestVariable Create(DataVariableInfo dataVariableInfo)
        {{
            return new TestVariable(dataVariableInfo);
        }}
    }}
}}";

    private static string ScalarVariableClass_WithDescriptorReferenceInSameNamespace_Source() =>
        $@"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestApp.Variables
{{
    public partial class TestVariable : ScalarDataVariable<int>
    {{
        private static readonly DataVariableDescriptor _Descriptor = global::TestApp.Variables.VariableDescriptors.TestDescriptor;
        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> configured as a placeholder for the underlying telemetry variable which is unavailable in the current context.
        /// </summary>
        /// <remarks>
        /// The returned instance cannot be used to read data because the value for property <see cref = ""IsAvailable""/> is <see langword=""false""/>.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable() : base(_Descriptor, variableInfo: null)
        {{
        }}

        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""DataVariableInfo""/>.
        /// </summary>
        /// <exception cref = ""SharpRacer.Telemetry.Variables.DataVariableInitializationException"">
        /// <paramref name = ""dataVariableInfo""/> is not compatible with the telemetry variable represented by this instance.
        /// </exception>
        /// <remarks>
        /// If <paramref name = ""dataVariableInfo""/> is <see langword=""null""/>, the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(DataVariableInfo? dataVariableInfo) : base(_Descriptor, dataVariableInfo)
        {{
        }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static TestVariable Create(DataVariableInfo dataVariableInfo)
        {{
            return new TestVariable(dataVariableInfo);
        }}
    }}
}}";

    private static string ScalarVariableClass_WithDescriptorReferenceInSharpRacerTelemetryVariablesNamespace_Source() =>
        $@"using SharpRacer.Telemetry.Variables;

#nullable enable
namespace TestApp.Variables
{{
    public partial class TestVariable : ScalarDataVariable<int>
    {{
        private static readonly DataVariableDescriptor _Descriptor = global::SharpRacer.Telemetry.Variables.VariableDescriptors.TestDescriptor;
        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> configured as a placeholder for the underlying telemetry variable which is unavailable in the current context.
        /// </summary>
        /// <remarks>
        /// The returned instance cannot be used to read data because the value for property <see cref = ""IsAvailable""/> is <see langword=""false""/>.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable() : base(_Descriptor, variableInfo: null)
        {{
        }}

        /// <summary>
        /// Creates an instance of <see cref = ""TestVariable""/> from the specified <see cref = ""DataVariableInfo""/>.
        /// </summary>
        /// <exception cref = ""SharpRacer.Telemetry.Variables.DataVariableInitializationException"">
        /// <paramref name = ""dataVariableInfo""/> is not compatible with the telemetry variable represented by this instance.
        /// </exception>
        /// <remarks>
        /// If <paramref name = ""dataVariableInfo""/> is <see langword=""null""/>, the returned instance represents a telemetry variable which is unavailable in the current context and cannot be used to read data.
        /// </remarks>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public TestVariable(DataVariableInfo? dataVariableInfo) : base(_Descriptor, dataVariableInfo)
        {{
        }}

        /// <inheritdoc/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static TestVariable Create(DataVariableInfo dataVariableInfo)
        {{
            return new TestVariable(dataVariableInfo);
        }}
    }}
}}";
}
