using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    }

    [Fact]
    public void HasAttributes_NoAttributesTest()
    {
        var decl = ClassDeclaration("Foo");

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
    }

    [Fact]
    public void IsPartialClass_NoPartialModifierTest()
    {
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
    }

    [Fact]
    public void IsStaticClass_NoStaticModifierTest()
    {
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
    }

    [Theory]
    [MemberData(nameof(NotStaticPartialClassData))]
    public void IsStaticPartialClass_NotStaticPartialClassTest(ClassDeclarationSyntax classDecl)
    {
        Assert.False(classDecl.IsStaticPartialClass());
    }

    [Theory]
    [MemberData(nameof(WithModifiers_AccessibilityStaticPartialData))]
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

    [Fact]
    public void WithModifiers_ThrowOnNullNodeArg()
    {
        ClassDeclarationSyntax node = null!;

        Assert.Throws<ArgumentNullException>(() => node.WithModifiers(Accessibility.Public, false, true));
    }

    public static TheoryData<ClassDeclarationSyntax> NotStaticPartialClassData()
    {
        var data = new TheoryData<ClassDeclarationSyntax>();

        var classDecl = ClassDeclaration(Identifier("Foo"));

        // No modifiers
        data.Add(classDecl);

        // Public, not partial or static
        data.Add(classDecl.WithModifiers(TokenList([Token(SyntaxKind.PublicKeyword)])));

        // Public partial, not static
        data.Add(classDecl.WithModifiers(TokenList([
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.PartialKeyword)])));

        // Public static, not partial
        data.Add(classDecl.WithModifiers(TokenList([
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword)])));

        return data;
    }

    public static TheoryData<Accessibility, bool, bool> WithModifiers_AccessibilityStaticPartialData()
    {
        var data = new TheoryData<Accessibility, bool, bool>();

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
            foreach (var isStatic in (bool[])[false, true])
            {
                foreach (var isPartial in (bool[])[false, true])
                {
                    data.Add(accessibility, isStatic, isPartial);
                }
            }
        }

        return data;
    }
}
