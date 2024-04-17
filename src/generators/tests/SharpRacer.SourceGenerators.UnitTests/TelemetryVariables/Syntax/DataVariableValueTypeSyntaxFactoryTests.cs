namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class DataVariableValueTypeSyntaxFactoryTests
{
    [Theory]
    [InlineData(VariableValueType.Bitfield, "DataVariableValueType.Bitfield")]
    [InlineData(VariableValueType.Bool, "DataVariableValueType.Bool")]
    [InlineData(VariableValueType.Byte, "DataVariableValueType.Byte")]
    [InlineData(VariableValueType.Double, "DataVariableValueType.Double")]
    [InlineData(VariableValueType.Float, "DataVariableValueType.Float")]
    [InlineData(VariableValueType.Int, "DataVariableValueType.Int")]
    public void EnumMemberAccessExpression_Test(VariableValueType valueType, string expected)
    {
        var expr = DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(valueType);

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
        var expr = DataVariableValueTypeSyntaxFactory.MemberIdentifier(valueType);

        Assert.Equal(expected, expr.ToFullString());
    }

    [Fact]
    public void MemberIdentifier_ThrowOnInvalidVariableValueTypeTest()
    {
        Assert.Throws<ArgumentException>(() => DataVariableValueTypeSyntaxFactory.MemberIdentifier((VariableValueType)999));
    }
}
