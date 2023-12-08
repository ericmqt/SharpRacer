using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables;
internal static class TypedScalarVariableClassFactory
{
    public static ClassDeclarationSyntax CreateClassDeclaration(
        TypedScalarVariableClassModel model,
        CancellationToken cancellationToken,
        out string[] requiredUsingNamespaces)
    {
        cancellationToken.ThrowIfCancellationRequested();

        requiredUsingNamespaces = GetRequiredUsingNamespaces(model);

        var accessibility = model.IsClassInternal ? Accessibility.Internal : Accessibility.Public;
        var baseTypeList = GetClassBaseTypeList(model, cancellationToken);

        var classMembers = CreateClassMemberList(model, cancellationToken);

        return ClassDeclaration(model.ClassName)
            .WithKeyword(Token(SyntaxKind.ClassKeyword))
            .WithModifiers(accessibility, isStatic: false, isPartial: model.IsClassPartial)
            .WithBaseList(baseTypeList)
            .WithMembers(classMembers);
    }

    private static SyntaxList<MemberDeclarationSyntax> CreateClassMemberList(TypedScalarVariableClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = new List<MemberDeclarationSyntax>();

        var descriptorField = CreateDescriptorField(model);

        var ctor1 = VariableClassSyntaxFactory.ConstructorFromDescriptor(model);
        var ctor2 = VariableClassSyntaxFactory.ConstructorFromDescriptorWithDataVariableInfoParameter(model);

        members.Add(descriptorField);
        members.Add(ctor1);
        members.Add(ctor2);

        if (model.ImplementICreateDataVariableInterface)
        {
            members.Add(VariableClassSyntaxFactory.ICreateDataVariableCreateMethodDeclaration(model.ClassName));
        }

        return List(members);
    }

    private static FieldDeclarationSyntax CreateDescriptorField(TypedScalarVariableClassModel model)
    {
        if (model.DescriptorPropertyReference != null)
        {
            var descriptorRef = model.DescriptorPropertyReference!.Value;

            var descriptorAccessExpr = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(descriptorRef.DescriptorClassName),
                IdentifierName(descriptorRef.PropertyName));

            return VariableClassSyntaxFactory.DescriptorStaticFieldFromDescriptorReferenceDeclaration(
                model.DescriptorFieldIdentifier(),
                descriptorAccessExpr);
        }

        return VariableClassSyntaxFactory.DescriptorStaticField(model);
    }

    private static BaseListSyntax GetClassBaseTypeList(TypedScalarVariableClassModel model, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var scalarDataVariableGenericBaseType = SharpRacerTypes.ScalarDataVariableBaseType(model.VariableValueTypeArg());

        if (model.AddICreateDataVariableInterfaceBaseType)
        {
            return BaseList(
                SeparatedList([
                    scalarDataVariableGenericBaseType,
                    VariableClassSyntaxFactory.ICreateDataVariableInterfaceBaseType(model.ClassIdentifierName())
                ]));
        }

        return BaseList(SingletonSeparatedList(scalarDataVariableGenericBaseType));
    }

    private static string[] GetRequiredUsingNamespaces(TypedScalarVariableClassModel model)
    {
        var telemetryVariablesNamespace = "SharpRacer.Telemetry.Variables";

        var requiredNamespaces = new List<string>(2)
        {
            telemetryVariablesNamespace
        };

        if (model.DescriptorPropertyReference.HasValue)
        {
            var descriptorNamespace = model.DescriptorPropertyReference.Value.DescriptorClassNamespace;

            if (!string.Equals(descriptorNamespace, telemetryVariablesNamespace, StringComparison.Ordinal))
            {
                requiredNamespaces.Add(descriptorNamespace);
            }
        }

        return requiredNamespaces.ToArray();
    }
}
