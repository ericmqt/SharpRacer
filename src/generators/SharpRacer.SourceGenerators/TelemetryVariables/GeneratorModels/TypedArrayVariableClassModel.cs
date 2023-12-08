using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class TypedArrayVariableClassModel : TypedVariableClassModelBase
{
    private readonly string _arrayLengthFieldName;

    public TypedArrayVariableClassModel(
        string className,
        VariableModel variableModel,
        DescriptorPropertyReference? descriptorPropertyReference)
        : this(
              className,
              variableModel.VariableInfo.Name,
              variableModel.VariableValueType,
              variableModel.VariableValueCount,
              variableModel.VariableValueUnit,
              descriptorPropertyReference)
    {

    }

    public TypedArrayVariableClassModel(
        string className,
        string variableName,
        VariableValueType variableValueType,
        int variableValueCount,
        string? variableValueUnit,
        DescriptorPropertyReference? descriptorPropertyReference)
        : base(className, variableName, variableValueType, variableValueUnit, descriptorPropertyReference)
    {
        ArrayLength = variableValueCount;

        _arrayLengthFieldName = "_ArrayLength";
    }

    public int ArrayLength { get; }

    public SyntaxToken ArrayLengthFieldIdentifier() => Identifier(_arrayLengthFieldName);
    public IdentifierNameSyntax ArrayLengthFieldIdentifierName() => IdentifierName(_arrayLengthFieldName);
}
