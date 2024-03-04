using SharpRacer.SessionInfo.Yaml;
using SharpRacer.Tools.TelemetryVariables.Data.Entities;

namespace SharpRacer.Tools.TelemetryVariables.Models;
public class CarModel
{
    public CarModel()
    {

    }

    public CarModel(CarEntity carEntity)
    {
        ArgumentNullException.ThrowIfNull(carEntity);

        Name = carEntity.Name;
        Path = carEntity.Path;
        ShortName = carEntity.ShortName;
    }

    public CarModel(DriverNode driverNode, IEnumerable<string> carVariableNames)
        : this(driverNode, carVariableNames, default)
    {

    }

    public CarModel(DriverNode driverNode, IEnumerable<string> carVariableNames, ContentVersion contentVersion)
    {
        ArgumentNullException.ThrowIfNull(driverNode);
        ArgumentNullException.ThrowIfNull(carVariableNames);

        Name = driverNode.CarScreenName;
        Path = driverNode.CarPath;
        ShortName = driverNode.CarScreenNameShort;

        VariableNames = carVariableNames.Distinct().ToList();
        ContentVersion = contentVersion;
    }

    public ContentVersion ContentVersion { get; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public IEnumerable<string> VariableNames { get; set; } = Enumerable.Empty<string>();
}
