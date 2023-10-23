namespace SharpRacer.Interop;

/// <summary>
/// Provides constant values that may be used when reading from simulator data files.
/// </summary>
public static class DataFileConstants
{
    /// <summary>
    /// The offset into the data file where <see cref="DataFileHeader"/> is located.
    /// </summary>
    public const int HeaderOffset = 0;

    /// <summary>
    /// Header version. See IRSDK_VER.
    /// </summary>
    public static int HeaderVersion = 2;

    /// <summary>
    /// The maximum length of a native string field, in characters.
    /// </summary>
    public const int MaxStringLength = 32;

    /// <summary>
    /// The maximum length of a description field, in characters.
    /// </summary>
    public const int MaxDescriptionLength = 64;

    /// <summary>
    /// The maximum number of data buffers that may be present in a data file.
    /// </summary>
    public const int MaxDataVariableBuffers = 4;
}
