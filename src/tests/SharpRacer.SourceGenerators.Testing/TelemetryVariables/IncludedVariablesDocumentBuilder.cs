using System.Text.Json;
using SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
using SharpRacer.SourceGenerators.Testing.Text;

namespace SharpRacer.SourceGenerators.Testing.TelemetryVariables;
public class IncludedVariablesDocumentBuilder
{
    private readonly List<string> _variableNames;

    public IncludedVariablesDocumentBuilder()
    {
        _variableNames = new List<string>();
    }

    public IncludedVariablesDocumentBuilder(IEnumerable<string> variableNames)
    {
        _variableNames = variableNames?.ToList() ?? new List<string>();
    }

    public static IncludedVariablesDocumentBuilder FromNames(params string[] variableNames)
    {
        return new IncludedVariablesDocumentBuilder(variableNames);
    }

    public IncludedVariablesDocumentBuilder Add(string variableName)
    {
        _variableNames.Add(variableName);

        return this;
    }

    public AdditionalTextFile ToAdditionalTextFile(string documentPath)
    {
        var json = JsonSerializer.Serialize(_variableNames);

        var jsonSourceText = new JsonSourceText(json);

        return new AdditionalTextFile(documentPath, jsonSourceText);
    }

    public IncludedVariablesFile ToIncludedVariablesFile(string documentPath)
    {
        var fileName = new IncludedVariablesFileName(Path.GetFileName(documentPath));

        return ToIncludedVariablesFile(documentPath, fileName);
    }

    public IncludedVariablesFile ToIncludedVariablesFile(string documentPath, IncludedVariablesFileName fileName)
    {
        var json = JsonSerializer.Serialize(_variableNames);

        var jsonSourceText = new JsonSourceText(json);
        var additionalText = new AdditionalTextFile(documentPath, jsonSourceText);

        return new IncludedVariablesFile(fileName, additionalText, jsonSourceText);
    }
}
