using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class VariableModelDescriptorSyntax
{
    private readonly VariableModel _model;

    public VariableModelDescriptorSyntax(VariableModel model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public MemberAccessExpressionSyntax DescriptorPropertyMemberAccess(TypeSyntax descriptorClassType)
    {
        return MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            descriptorClassType,
            DescriptorPropertyIdentifier());
    }

    public IdentifierNameSyntax DescriptorPropertyIdentifier() => IdentifierName(DescriptorPropertyIdentifierToken());
    public SyntaxToken DescriptorPropertyIdentifierToken() => Identifier(_model.DescriptorName);
}
