namespace SharpRacer.IO;

/// <summary>
/// Describes offsets into a <see cref="DataVariableHeader"/> structure.
/// </summary>
public static class DataVariableHeaderOffsets
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
