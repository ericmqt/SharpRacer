using System.Runtime.InteropServices;
using SharpRacer.IO.Primitives;

namespace SharpRacer.IO;

/// <summary>
/// Describes a telemetry variable in a simulator data file.
/// </summary>
/// <remarks>
/// See: irsdk_varHeader
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = DataVariableHeader.Size)]
public readonly struct DataVariableHeader
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataVariableHeader"/>.
    /// </summary>
    public const int Size = 144;

    /// <summary>
    /// Indicates the type of variable value according to irsdk_VarType.
    /// </summary>
    [FieldOffset(FieldOffsets.TypeOffset)]
    public readonly int Type;

    /// <summary>
    /// Offset from the start of the buffer.
    /// </summary>
    [FieldOffset(FieldOffsets.OffsetOffset)]
    public readonly int Offset;

    /// <summary>
    /// The length of the array of values represented by this variable.
    /// </summary>
    [FieldOffset(FieldOffsets.CountOffset)]
    public readonly int Count;

    /// <summary>
    /// When <see langword="true"/>, indicates the variable is a time-sliced array of a single value.
    /// </summary>
    [FieldOffset(FieldOffsets.CountAsTimeOffset)]
    public readonly bool CountAsTime;

    /// <summary>
    /// The name of the variable.
    /// </summary>
    [FieldOffset(FieldOffsets.NameOffset)]
    public readonly IRSDKString Name;

    /// <summary>
    /// The variable description.
    /// </summary>
    [FieldOffset(FieldOffsets.DescriptionOffset)]
    public readonly IRSDKDescString Description;

    /// <summary>
    /// The unit of the value of the variable, if present.
    /// </summary>
    [FieldOffset(FieldOffsets.UnitOffset)]
    public readonly IRSDKString Unit;

    /// <summary>
    /// Provides field offsets for a <see cref="DataVariableHeader"/> structure.
    /// </summary>
    public static class FieldOffsets
    {
        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Type"/> field is located.
        /// </summary>
        public const int TypeOffset = 0;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Offset"/> field is located.
        /// </summary>
        public const int OffsetOffset = 4;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Count"/> field is located.
        /// </summary>
        public const int CountOffset = 8;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.CountAsTime"/> field is located.
        /// </summary>
        public const int CountAsTimeOffset = 12;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Name"/> field is located.
        /// </summary>
        public const int NameOffset = 16;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Description"/> field is located.
        /// </summary>
        public const int DescriptionOffset = 48;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="DataVariableHeader.Unit"/> field is located.
        /// </summary>
        public const int UnitOffset = 112;
    }
}
