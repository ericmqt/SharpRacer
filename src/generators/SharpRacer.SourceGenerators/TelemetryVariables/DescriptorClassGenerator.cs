﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;

internal class DescriptorClassGenerator
{
    private readonly List<string> _usingNamespaces;

    public DescriptorClassGenerator(DescriptorClassGeneratorModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));

        _usingNamespaces = new List<string>()
        {
            "System",
            "System.Collections.Generic",
            "System.Text",
            "SharpRacer.Telemetry.Variables"
        };
    }

    public DescriptorClassGeneratorModel Model { get; }

    public ClassDeclarationSyntax CreateClassDeclaration(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ClassDeclaration(Model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, isStatic: true, isPartial: true)
            .WithMembers(List(EnumerateMembers(cancellationToken)));
        //.WithBaseList(CreateBaseList(cancellationToken));
    }

    public CompilationUnitSyntax CreateCompilationUnit(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(cancellationToken);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(Model.TypeNamespace))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        var usingDirectives = _usingNamespaces.Select(x => UsingDirective(IdentifierName(x)));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(usingDirectives))
            .AddMembers(namespaceDecl);
    }

    private IEnumerable<MemberDeclarationSyntax> EnumerateMembers(CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < Model.DescriptorProperties.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var property = Model.DescriptorProperties[i];

            yield return DescriptorProperty(ref property);
        }
    }

    private static PropertyDeclarationSyntax DescriptorProperty(
        ref readonly DescriptorPropertyModel descriptorPropertyModel,
        Accessibility accessibility = Accessibility.Public)
    {
        if (descriptorPropertyModel == default)
        {
            throw new ArgumentException($"'{nameof(descriptorPropertyModel)}' cannot be a default value.", nameof(descriptorPropertyModel));
        }

        var decl = PropertyDeclaration(SharpRacerTypes.DataVariableDescriptor(), Identifier(descriptorPropertyModel.PropertyName))
            .WithModifiers(accessibility, isStatic: true)
            .WithGetOnlyAutoAccessor();

        // Initializer
        var variableNameArgument = Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(descriptorPropertyModel.VariableName)));
        var valueTypeArgument = Argument(DataVariableValueTypeSyntaxFactory.EnumMemberAccessExpression(descriptorPropertyModel.VariableValueType));
        var valueCountArgument = Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(descriptorPropertyModel.VariableValueCount)));

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