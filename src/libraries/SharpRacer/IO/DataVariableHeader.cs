using System.Runtime.InteropServices;

namespace SharpRacer.IO;

/// <summary>
/// Describes a telemetry variable in a simulator data file.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = DataFileConstants.DataVariableHeaderLength)]
public unsafe struct DataVariableHeader
{
    /// <summary>
    /// Indicates the type of variable value according to irsdk_VarType.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.TypeOffset)]
    public int Type;

    /// <summary>
    /// Offset from the start of the buffer.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.OffsetOffset)]
    public int Offset;

    /// <summary>
    /// The length of the array of values represented by this variable.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.CountOffset)]
    public int Count;

    /// <summary>
    /// When <see langword="true"/>, indicates the variable is a time-sliced array of a single value.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.CountAsTimeOffset)]
    public bool CountAsTime;

    /// <summary>
    /// The name of the variable.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.NameOffset)]
    public fixed byte Name[DataFileConstants.MaxStringLength];

    /// <summary>
    /// The variable description.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.DescriptionOffset)]
    public fixed byte Description[DataFileConstants.MaxDescriptionLength];

    /// <summary>
    /// The unit of the value of the variable, if present.
    /// </summary>
    [FieldOffset(DataVariableHeaderOffsets.UnitOffset)]
    public fixed byte Unit[DataFileConstants.MaxStringLength];
}
