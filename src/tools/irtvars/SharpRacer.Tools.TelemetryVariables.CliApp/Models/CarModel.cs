﻿using SharpRacer.SessionInfo.Yaml;
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
    {
        ArgumentNullException.ThrowIfNull(driverNode);
        ArgumentNullException.ThrowIfNull(carVariableNames);

        Name = driverNode.CarScreenName;
        Path = driverNode.CarPath;
        ShortName = driverNode.CarScreenNameShort;

        VariableNames = carVariableNames.Distinct().ToList();
    }

    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public IEnumerable<string> VariableNames { get; set; } = Enumerable.Empty<string>();
}
