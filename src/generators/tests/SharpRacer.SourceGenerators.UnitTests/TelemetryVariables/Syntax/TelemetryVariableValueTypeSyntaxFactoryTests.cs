namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

public class TelemetryVariableValueTypeSyntaxFactoryTests
{
    [Theory]
    [InlineData(VariableValueType.Bitfield, "TelemetryVariableValueType.Bitfield")]
    [InlineData(VariableValueType.Bool, "TelemetryVariableValueType.Bool")]
    [InlineData(VariableValueType.Byte, "TelemetryVariableValueType.Byte")]
    [InlineData(VariableValueType.Double, "TelemetryVariableValueType.Double")]
    [InlineData(VariableValueType.Float, "TelemetryVariableValueType.Float")]
    [InlineData(VariableValueType.Int, "TelemetryVariableValueType.Int")]
    public void EnumMemberAccessExpression_Test(VariableValueType valueType, string expected)
    {
        var expr = TelemetryVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType);

        Assert.Equal(expected, expr.ToFullString());
    }

    [Theory]
    [InlineData(VariableValueType.Bitfield, "Bitfield")]
    [InlineData(VariableValueType.Bool, "Bool")]
    [InlineData(VariableValueType.Byte, "Byte")]
    [InlineData(VariableValueType.Double, "Double")]
    [InlineData(VariableValueType.Float, "Float")]
    [InlineData(VariableValueType.Int, "Int")]
    public void MemberIdentifier_Test(VariableValueType valueType, string expected)
    {
        var expr = TelemetryVariableValueTypeSyntaxFactory.MemberIdentifier(valueType);

        Assert.Equal(expected, expr.ToFullString());
    }

    [Fact]
    public void MemberIdentifier_ThrowOnInvalidVariableValueTypeTest()
    {
        Assert.Throws<ArgumentException>(() => TelemetryVariableValueTypeSyntaxFactory.MemberIdentifier((VariableValueType)999));
    }
}
