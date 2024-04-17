using Microsoft.CodeAnalysis;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
public class DataVariableFactorySyntaxFactoryTests
{
    [Fact]
    public void InstanceCreationExpression_Test()
    {
        var node = DataVariableFactorySyntaxFactory.InstanceCreationExpression(IdentifierName("provider"));

        var nodeString = node.NormalizeWhitespace(eol: "\n").ToFullString();

        Assert.Equal("new DataVariableFactory(provider)", nodeString);
    }

    [Fact]
    public void CreateArrayMethodAccessExpression_Test()
    {
        var typeArg = VariableValueTypes.Int();
        var node = DataVariableFactorySyntaxFactory.CreateArrayMethodAccessExpression(IdentifierName("factory"), typeArg);

        Assert.Equal("factory.CreateArray<int>", node.ToFullString());
    }

    [Fact]
    public void CreateArrayFromDescriptorMethodInvocation_Test()
    {
        var descriptorPropertyRef = new DescriptorPropertyReference("SessionNum", "SessionNum", "MyDescriptors", "TestAssembly.Variables");

        var node = DataVariableFactorySyntaxFactory.CreateArrayFromDescriptorMethodInvocation(
            IdentifierName("factory"),
            VariableValueTypes.Int(),
            descriptorPropertyRef.StaticPropertyMemberAccess())
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateArray<int>(global::TestAssembly.Variables.MyDescriptors.SessionNum)", node.ToString());
    }

    [Fact]
    public void CreateArrayFromVariableNameAndArrayLengthMethodInvocation_Test()
    {
        var typeArg = VariableValueTypes.Int();
        var node = DataVariableFactorySyntaxFactory.CreateArrayFromVariableNameAndArrayLengthMethodInvocation(
            IdentifierName("factory"),
            typeArg,
            variableName: "Lat",
            arrayLength: 3)
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateArray<int>(\"Lat\", 3)", node.ToFullString());
    }

    [Fact]
    public void CreateScalarMethodAccessExpression_Test()
    {
        var typeArg = VariableValueTypes.Int();
        var node = DataVariableFactorySyntaxFactory.CreateScalarMethodAccessExpression(IdentifierName("factory"), typeArg);

        Assert.Equal("factory.CreateScalar<int>", node.ToFullString());
    }

    [Fact]
    public void CreateScalarFromDescriptorMethodInvocation_Test()
    {
        var descriptorPropertyRef = new DescriptorPropertyReference("SessionNum", "SessionNum", "MyDescriptors", "TestAssembly.Variables");

        var node = DataVariableFactorySyntaxFactory.CreateScalarFromDescriptorMethodInvocation(
            IdentifierName("factory"),
            VariableValueTypes.Int(),
            descriptorPropertyRef.StaticPropertyMemberAccess())
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateScalar<int>(global::TestAssembly.Variables.MyDescriptors.SessionNum)", node.ToString());
    }

    [Fact]
    public void CreateScalarFromVariableNameMethodInvocation_Test()
    {
        var typeArg = VariableValueTypes.Int();
        var node = DataVariableFactorySyntaxFactory.CreateScalarFromVariableNameMethodInvocation(
            IdentifierName("factory"),
            typeArg,
            variableName: "Lat")
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateScalar<int>(\"Lat\")", node.ToFullString());
    }

    [Fact]
    public void CreateTypeMethodAccessExpression()
    {
        var typeArg = IdentifierName("LatitudeVariable");
        var node = DataVariableFactorySyntaxFactory.CreateTypeMethodAccessExpression(IdentifierName("factory"), typeArg);

        Assert.Equal("factory.CreateType<LatitudeVariable>", node.ToFullString());
    }

    [Fact]
    public void CreateTypeFromDescriptorMethodInvocation_Test()
    {
        var typeArg = IdentifierName("LatitudeVariable");
        var descriptorPropertyRef = new DescriptorPropertyReference("Lat", "Latitude", "MyDescriptors", "TestAssembly.Variables");

        var node = DataVariableFactorySyntaxFactory.CreateTypeFromDescriptorMethodInvocation(
            IdentifierName("factory"),
            typeArg,
            descriptorPropertyRef.StaticPropertyMemberAccess())
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateType<LatitudeVariable>(global::TestAssembly.Variables.MyDescriptors.Latitude)", node.ToString());
    }

    [Fact]
    public void CreateTypeFromVariableNameMethodInvocation_Test()
    {
        var typeArg = IdentifierName("LatitudeVariable");

        var node = DataVariableFactorySyntaxFactory.CreateTypeFromVariableNameMethodInvocation(
            IdentifierName("factory"), typeArg, "Lat")
            .NormalizeWhitespace(eol: "\n");

        Assert.Equal("factory.CreateType<LatitudeVariable>(\"Lat\")", node.ToString());
    }
}
