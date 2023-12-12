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
    private readonly List<UsingDirectiveSyntax> _usingDirectives;

    public DescriptorClassGenerator(DescriptorClassModel model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));

        _usingDirectives =
        [
            UsingDirective(IdentifierName("System")),
            UsingDirective(ParseName("System.Collections.Generic")),
            UsingDirective(ParseName("System.Text")),
            UsingDirective(ParseName("SharpRacer.Telemetry.Variables"))
        ];
    }

    public DescriptorClassModel Model { get; }

    public ClassDeclarationSyntax CreateClassDeclaration(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ClassDeclaration(Model.TypeName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(Accessibility.NotApplicable, isStatic: true, isPartial: true)
            .WithMembers(List(EnumerateMembers(cancellationToken)));
    }

    public CompilationUnitSyntax CreateCompilationUnit(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var classDecl = CreateClassDeclaration(cancellationToken);

        var namespaceDecl = NamespaceDeclaration(IdentifierName(Model.TypeNamespace))
            .WithMembers(List(new MemberDeclarationSyntax[] { classDecl }));

        return CompilationUnit()
            .WithUsings(new SyntaxList<UsingDirectiveSyntax>(_usingDirectives))
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

        var objectCreationExpr = DataVariableDescriptorSyntaxFactory.CreateNewInstanceExpression(
            descriptorPropertyModel.VariableName,
            descriptorPropertyModel.VariableValueType,
            descriptorPropertyModel.VariableValueCount);

        return decl.WithInitializer(EqualsValueClause(objectCreationExpr))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}
