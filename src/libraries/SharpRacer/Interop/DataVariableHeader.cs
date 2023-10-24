﻿using System.Runtime.InteropServices;

namespace SharpRacer.Interop;

/// <summary>
/// Describes a telemetry variable in a simulator data file.
/// </summary>
/// <remarks>
/// See: irsdk_varHeader
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = Size)]
public readonly struct DataVariableHeader
{
    /// <summary>
    /// The length, in bytes, of an instance of <see cref="DataVariableHeader"/>.
    /// </summary>
    public const int Size = 144;

    /// <summary>
    /// Initializes an instance of <see cref="DataVariableHeader"/> with default values.
    /// </summary>
    public DataVariableHeader()
    {

    }

    /// <summary>
    /// Initializes an instance of <see cref="DataVariableHeader"/> with the specified values.
    /// </summary>
    /// <param name="name">The value to assign to the <see cref="Name"/> field.</param>
    /// <param name="type">The value to assign to the <see cref="Type"/> field.</param>
    /// <param name="count">The value to assign to the <see cref="Count"/> field.</param>
    /// <param name="countAsTime">The value to assign to the <see cref="CountAsTime"/> field.</param>
    /// <param name="offset">The value to assign to the <see cref="Offset"/> field.</param>
    /// <param name="description">The value to assign to the <see cref="Description"/> field.</param>
    /// <param name="unit">The value to assign to the <see cref="Unit"/> field.</param>
    public DataVariableHeader(
        IRSDKString name,
        int type,
        int count,
        bool countAsTime,
        int offset,
        IRSDKDescString description,
        IRSDKString unit)
    {
        Name = name;
        Type = type;
        Count = count;
        CountAsTime = countAsTime;
        Offset = offset;
        Description = description;
        Unit = unit;
    }

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
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Type"/> field is located.
        /// </summary>
        public const int TypeOffset = 0;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Offset"/> field is located.
        /// </summary>
        public const int OffsetOffset = 4;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Count"/> field is located.
        /// </summary>
        public const int CountOffset = 8;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="CountAsTime"/> field is located.
        /// </summary>
        public const int CountAsTimeOffset = 12;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Name"/> field is located.
        /// </summary>
        public const int NameOffset = 16;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Description"/> field is located.
        /// </summary>
        public const int DescriptionOffset = 48;

        /// <summary>
        /// The offset into a <see cref="DataVariableHeader"/> structure where the <see cref="Unit"/> field is located.
        /// </summary>
        public const int UnitOffset = 112;
    }
}
