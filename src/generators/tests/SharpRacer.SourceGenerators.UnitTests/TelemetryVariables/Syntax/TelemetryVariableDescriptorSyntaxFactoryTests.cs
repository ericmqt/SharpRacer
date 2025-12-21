using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

public class TelemetryVariableDescriptorSyntaxFactoryTests
{
    [Fact]
    public void CreateNewInstanceExpression_Test()
    {
        var expr = TelemetryVariableDescriptorSyntaxFactory.CreateNewInstanceExpression("Foo", VariableValueType.Int, 3);

        var exprStr = expr.NormalizeWhitespace(eol: "\n").ToFullString();

        Assert.Equal("new global::SharpRacer.Telemetry.TelemetryVariableDescriptor(\"Foo\", global::SharpRacer.Telemetry.TelemetryVariableValueType.Int, 3)", exprStr);
    }
}
