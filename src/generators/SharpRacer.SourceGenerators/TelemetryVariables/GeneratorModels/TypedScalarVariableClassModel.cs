namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class TypedScalarVariableClassModel : TypedVariableClassModelBase
{
    public TypedScalarVariableClassModel(
        string className,
        VariableModel variableModel,
        DescriptorPropertyReference? descriptorPropertyReference)
        : this(
              className,
              variableModel.VariableInfo.Name,
              variableModel.VariableValueType,
              variableModel.VariableValueUnit,
              descriptorPropertyReference)
    {

    }

    public TypedScalarVariableClassModel(
        string className,
        string variableName,
        VariableValueType variableValueType,
        string? variableValueUnit,
        DescriptorPropertyReference? descriptorPropertyReference)
        : base(className, variableName, variableValueType, variableValueUnit, descriptorPropertyReference)
    {
    }
}
