using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

public class DescriptorClassGeneratorTests
{
    [Fact]
    public void Create_SingleVariableTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, false, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var descriptorPropertyModel = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        var classModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
            .WithDescriptorProperties([descriptorPropertyModel]);

        var compilationUnit = DescriptorClassGenerator.CreateCompilationUnit(ref classModel, default);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(SingleVariableTest_Source()).GetCompilationUnitRoot();

        var cuStr = compilationUnit.NormalizeWhitespace().ToFullString();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_ContainsDeprecatedVariableWithoutDeprecatingModelTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, true, "TestEx");
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var descriptorPropertyModel = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        var classModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
            .WithDescriptorProperties([descriptorPropertyModel]);

        var compilationUnit = DescriptorClassGenerator.CreateCompilationUnit(ref classModel, default);
        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(DeprecatedVariableWithoutDeprecatingModel_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    [Fact]
    public void Create_ContainsDeprecatedVariableWithoutDeprecatingVariableNameTest()
    {
        var variableInfo = new VariableInfo("Test", VariableValueType.Int, 1, "Test variable", "test/s", false, true, null);
        var variableModel = new VariableModel(variableInfo, default);

        var propertyName = "TestProperty";
        var propertyXmlSummary = "Test XML summary";
        var descriptorPropertyModel = new DescriptorPropertyModel(propertyName, propertyXmlSummary, variableModel);

        var classModel = new DescriptorClassModel("MyDescriptors", "TestApp.Variables", Location.None)
            .WithDescriptorProperties([descriptorPropertyModel]);

        var compilationUnit = DescriptorClassGenerator.CreateCompilationUnit(ref classModel, default);

        var expectedCompilationUnit = SyntaxAssert.ParseSyntaxTree(DeprecatedVariableWithoutDeprecatingVariableName_Source()).GetCompilationUnitRoot();

        SyntaxAssert.StructuralEquivalent(expectedCompilationUnit, compilationUnit);
        SyntaxAssert.CompilationUnitStringEqual(expectedCompilationUnit, compilationUnit);
    }

    private static string DeprecatedVariableWithoutDeprecatingModel_Source() =>
        $@"#nullable enable
namespace TestApp.Variables
{{
    /// <summary>
    /// Provides <see cref = ""global::SharpRacer.Telemetry.TelemetryVariableDescriptor""/> values that describe telemetry variables.
    /// </summary>
    static partial class MyDescriptors
    {{
        /// <summary>
        /// Test XML summary
        /// </summary>
        [System.ObsoleteAttribute(""Telemetry variable 'Test' is deprecated by variable 'TestEx'."")]
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static global::SharpRacer.Telemetry.TelemetryVariableDescriptor TestProperty {{ get; }} = new(""Test"", global::SharpRacer.Telemetry.TelemetryVariableValueType.Int, 1);
    }}
}}";

    private static string DeprecatedVariableWithoutDeprecatingVariableName_Source() =>
        $@"#nullable enable
namespace TestApp.Variables
{{
    /// <summary>
    /// Provides <see cref = ""global::SharpRacer.Telemetry.TelemetryVariableDescriptor""/> values that describe telemetry variables.
    /// </summary>
    static partial class MyDescriptors
    {{
        /// <summary>
        /// Test XML summary
        /// </summary>
        [System.ObsoleteAttribute(""Telemetry variable 'Test' is deprecated."")]
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static global::SharpRacer.Telemetry.TelemetryVariableDescriptor TestProperty {{ get; }} = new(""Test"", global::SharpRacer.Telemetry.TelemetryVariableValueType.Int, 1);
    }}
}}";

    private static string SingleVariableTest_Source() =>
        $@"#nullable enable
namespace TestApp.Variables
{{
    /// <summary>
    /// Provides <see cref = ""global::SharpRacer.Telemetry.TelemetryVariableDescriptor""/> values that describe telemetry variables.
    /// </summary>
    static partial class MyDescriptors
    {{
        /// <summary>
        /// Test XML summary
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCodeAttribute(""{TelemetryVariablesGenerator.ToolName}"", ""{TelemetryVariablesGenerator.ToolVersion}"")]
        public static global::SharpRacer.Telemetry.TelemetryVariableDescriptor TestProperty {{ get; }} = new(""Test"", global::SharpRacer.Telemetry.TelemetryVariableValueType.Int, 1);
    }}
}}";
}
