namespace SharpRacer.Tools.TelemetryVariables.Models;
internal class TelemetryVariablesDataModel
{
    public List<CarModel> Cars { get; set; } = [];
    public List<TelemetryVariableModel> Variables { get; set; } = [];
}
