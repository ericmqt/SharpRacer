using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;

namespace SharpRacer.SourceGenerators.TelemetryVariables.GeneratorModels;
internal class VariableModel
{
    public VariableModel(VariableInfo variableInfo)
    {
        VariableInfo = variableInfo ?? throw new ArgumentNullException(nameof(variableInfo));

        VariableName = variableInfo.Name;
        ContextPropertyName = variableInfo.Name;
        DescriptorName = variableInfo.Name;
    }

    public VariableModel(VariableInfo variableInfo, VariableOptions variableOptions)
    {
        if (variableOptions is null)
        {
            throw new ArgumentNullException(nameof(variableOptions));
        }

        VariableInfo = variableInfo ?? throw new ArgumentNullException(nameof(variableInfo));

        VariableName = !string.IsNullOrEmpty(variableOptions.Name)
            ? variableOptions.Name!
            : variableInfo.Name;

        ContextPropertyName = !string.IsNullOrEmpty(variableOptions.ContextPropertyName)
            ? variableOptions.ContextPropertyName!
            : VariableName;

        DescriptorName = !string.IsNullOrEmpty(variableOptions.DescriptorName)
            ? variableOptions.DescriptorName!
            : VariableName;
    }

    public string ContextPropertyName { get; }
    public string DescriptorName { get; }
    public string VariableName { get; }
    public VariableInfo VariableInfo { get; }
}
