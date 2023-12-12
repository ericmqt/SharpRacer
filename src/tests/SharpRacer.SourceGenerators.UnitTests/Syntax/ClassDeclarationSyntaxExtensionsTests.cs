using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.Syntax;
public class ClassDeclarationSyntaxExtensionsTests
{
    [Fact]
    public void HasAttributes_Test()
    {
        var decl = ClassDeclaration("Foo")
            .WithAttributeLists(
                SingletonList(
                    AttributeList(
                        SingletonSeparatedList(
                        Attribute(IdentifierName("GenerateDataVariableDescriptorsAttribute"))))
            ));

        Assert.True(decl.HasAttributes());

        decl = ClassDeclaration("Foo");
        Assert.False(decl.HasAttributes());
    }

    [Fact]
    public void IsPartialClass_Test()
    {
        var partialClassDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.PartialKeyword)
                    ]));

        Assert.True(partialClassDecl.IsPartialClass());

        var publicNonPartialClassDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([Token(SyntaxKind.PublicKeyword)]));

        Assert.False(publicNonPartialClassDecl.IsPartialClass());
    }

    [Fact]
    public void IsStaticClass_Test()
    {
        var classDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword)
                    ]));

        Assert.True(classDecl.IsStaticClass());

        var publicNonStaticClassDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([Token(SyntaxKind.PublicKeyword)]));

        Assert.False(publicNonStaticClassDecl.IsStaticClass());
    }

    [Fact]
    public void IsStaticPartialClass_Test()
    {
        var staticPartialClassDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword),
                    Token(SyntaxKind.PartialKeyword)
                    ]));

        Assert.True(staticPartialClassDecl.IsStaticPartialClass());

        var classDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(
                TokenList([
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.StaticKeyword)
                    ]));

        Assert.False(classDecl.IsStaticPartialClass());
    }

    [Theory]
    [MemberData(nameof(GetModifiersAndStaticPartialInputData))]
    public void WithModifiers_Test(Accessibility accessibility, bool isStatic, bool isPartial)
    {
        var classDecl = ClassDeclaration(Identifier("Foo"))
            .WithModifiers(accessibility, isStatic, isPartial);

        var accessibilityTokens = SyntaxFactoryHelpers.ModifiersFromAccessibility(accessibility).ToList();

        var classDeclModifierTokenKindList = classDecl.Modifiers.Select(x => x.Kind()).ToList();

        foreach (var accessibilityToken in accessibilityTokens)
        {
            Assert.Contains(accessibilityToken.Kind(), classDeclModifierTokenKindList);
        }

        if (isStatic)
        {
            Assert.Contains(SyntaxKind.StaticKeyword, classDeclModifierTokenKindList);
        }
        else
        {
            Assert.DoesNotContain(SyntaxKind.StaticKeyword, classDeclModifierTokenKindList);
        }

        if (isPartial)
        {
            Assert.Contains(SyntaxKind.PartialKeyword, classDeclModifierTokenKindList);
        }
        else
        {
            Assert.DoesNotContain(SyntaxKind.PartialKeyword, classDeclModifierTokenKindList);
        }
    }

    public static IEnumerable<object[]> GetModifiersAndStaticPartialInputData()
    {
        var data = new List<object[]>();

        var accessibilityValues = new List<Accessibility>()
        {
            Accessibility.Internal,
            Accessibility.Private,
            Accessibility.Protected,
            Accessibility.ProtectedAndInternal,
            Accessibility.Public
        };

        foreach (var accessibility in accessibilityValues)
        {
            data.AddRange([
                [accessibility, false, false],
                [accessibility, false, true],
                [accessibility, true, true],
                [accessibility, true, false]
            ]);
        }

        return data;
    }
}
