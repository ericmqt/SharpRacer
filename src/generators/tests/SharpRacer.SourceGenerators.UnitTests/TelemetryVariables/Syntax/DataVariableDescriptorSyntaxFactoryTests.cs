﻿using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class DataVariableDescriptorSyntaxFactoryTests
{
    [Fact]
    public void CreateNewInstanceExpression_Test()
    {
        var expr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression("Foo", VariableValueType.Int, 3);

        var exprStr = expr.NormalizeWhitespace(eol: "\n").ToFullString();

        Assert.Equal("new global::SharpRacer.Telemetry.DataVariableDescriptor(\"Foo\", global::SharpRacer.Telemetry.DataVariableValueType.Int, 3)", exprStr);
    }
}
