﻿using SharpRacer.Interop;

namespace SharpRacer.Telemetry;

/// <summary>
/// Provides information about a telemetry variable exposed by the simulator.
/// </summary>
public sealed class DataVariableInfo
{
    /// <summary>
    /// Creates an instance of <see cref="DataVariableInfo"/> from the specified <see cref="DataVariableHeader"/>.
    /// </summary>
    /// <param name="variableHeader"></param>
    public DataVariableInfo(DataVariableHeader variableHeader)
    {
        Description = variableHeader.Description.ToString();
        IsTimeSliceArray = variableHeader.CountAsTime;
        Name = variableHeader.Name.ToString();
        Offset = variableHeader.Offset;
        ValueCount = variableHeader.Count;
        ValueType = (DataVariableValueType)variableHeader.Type;
        ValueUnit = variableHeader.Unit.GetLength() > 0 ? variableHeader.Unit.ToString() : null;
    }

    /// <summary>
    /// Gets a description of the telemetry variable.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// If this is an array variable, gets a value indicating if the array represents a single value over time, otherwise <see langword="false"/>.
    /// </summary>
    public bool IsTimeSliceArray { get; }

    /// <summary>
    /// Gets the name of the telemetry variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the offset into the telemetry buffer line where the variable data is located.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// If the variable is an array of values, gets the number of values in the array, otherwise 1.
    /// </summary>
    public int ValueCount { get; }

    /// <summary>
    /// Gets the type of variable.
    /// </summary>
    public DataVariableValueType ValueType { get; }

    /// <summary>
    /// Gets the unit of the variable value, if present.
    /// </summary>
    public string? ValueUnit { get; }
}
