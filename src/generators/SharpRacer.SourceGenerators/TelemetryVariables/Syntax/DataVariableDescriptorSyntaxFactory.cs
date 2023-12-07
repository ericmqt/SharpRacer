using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.Syntax;

internal static class DataVariableDescriptorSyntaxFactory
{
    internal static PropertyDeclarationSyntax ReadOnlyStaticProperty(string propertyName, Accessibility accessibility = Accessibility.Public)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
        }

        return PropertyDeclaration(SharpRacerTypes.DataVariableDescriptor(), Identifier(propertyName))
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor();
    }

    internal static PropertyDeclarationSyntax ReadOnlyStaticPropertyWithInitializer(VariableModel model, Accessibility accessibility = Accessibility.Public)
    {
        if (model == default)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return ReadOnlyStaticPropertyWithInitializer(model.DescriptorName, model.VariableInfo, accessibility);
    }

    internal static PropertyDeclarationSyntax ReadOnlyStaticPropertyWithInitializer(string propertyName, VariableInfo variableInfo, Accessibility accessibility = Accessibility.Public)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
        }

        if (variableInfo == default)
        {
            throw new ArgumentException($"'{nameof(variableInfo)}' cannot be a default value.", nameof(variableInfo));
        }

        var decl = PropertyDeclaration(SharpRacerTypes.DataVariableDescriptor(), Identifier(propertyName))
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor();

        // Initializer
        var variableNameArgument = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(variableInfo.Name)));
        var valueTypeArgument = Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(variableInfo.ValueType));
        var valueCountArgument = Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(variableInfo.ValueCount)));

        var argumentList = ArgumentList(
            SeparatedList(
                new ArgumentSyntax[]
                {
                    variableNameArgument,
                    valueTypeArgument,
                    valueCountArgument
                }));

        return decl.WithInitializer(
            EqualsValueClause(
                ObjectCreationExpression(SharpRacerTypes.DataVariableDescriptor())
                    .WithArgumentList(argumentList)))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}
