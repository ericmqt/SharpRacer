using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpRacer.SourceGenerators.TelemetryVariables.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal abstract class TypedVariableClassModelBase
{
    private readonly string _descriptorFieldName;
    private readonly bool _implementICreateDataVariableInterface;
    private readonly string? _variableValueUnit;

    public TypedVariableClassModelBase(
        string className,
        string variableName,
        VariableValueType variableValueType,
        string? variableValueUnit,
        DescriptorPropertyReference? descriptorPropertyReference)
    {
        ClassName = className;
        DescriptorPropertyReference = descriptorPropertyReference;

        VariableName = variableName;
        VariableValueType = variableValueType;
        _variableValueUnit = variableValueUnit;

        _descriptorFieldName = "_Descriptor";

        AddICreateDataVariableInterfaceBaseType = true;
        _implementICreateDataVariableInterface = true;
    }

    public bool AddICreateDataVariableInterfaceBaseType { get; }
    public string ClassName { get; }
    public DescriptorPropertyReference? DescriptorPropertyReference { get; }
    public bool ImplementICreateDataVariableInterface => _implementICreateDataVariableInterface || AddICreateDataVariableInterfaceBaseType;
    public bool IsClassInternal { get; }
    public bool IsClassPartial { get; }
    public string VariableName { get; }
    public VariableValueType VariableValueType { get; }

    public SyntaxToken ClassIdentifier() => Identifier(ClassName);
    public IdentifierNameSyntax ClassIdentifierName() => IdentifierName(ClassName);
    public SyntaxToken DescriptorFieldIdentifier() => Identifier(_descriptorFieldName);
    public IdentifierNameSyntax DescriptorFieldIdentifierName() => IdentifierName(_descriptorFieldName);
    public TypeSyntax VariableValueTypeArg() => SharpRacerTypes.DataVariableTypeArgument(VariableValueType, _variableValueUnit);
}
