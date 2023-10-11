namespace SharpRacer.Telemetry;
public enum DataVariableValueType : int
{
    /// <summary>
    /// An 8-bit variable value.
    /// </summary>
    Byte = 0,

    /// <summary>
    /// An 8-bit boolean variable value.
    /// </summary>
    Bool = 1,

    /// <summary>
    /// A 32-bit signed integer variable value.
    /// </summary>
    Int = 2,

    /// <summary>
    /// A 32-bit integer variable value.
    /// </summary>
    Bitfield = 3,

    /// <summary>
    /// A single-precision floating point variable value.
    /// </summary>
    Float = 4,

    /// <summary>
    /// A double-precision floating point variable value.
    /// </summary>
    Double = 5
}