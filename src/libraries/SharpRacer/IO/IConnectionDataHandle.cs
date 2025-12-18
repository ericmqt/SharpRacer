namespace SharpRacer.IO;

/// <summary>
/// Represents a handle to a read-only block of memory over the simulator connection data file.
/// </summary>
/// <remarks>
/// Because <see cref="IConnectionDataHandle"/> implements the <see cref="IDisposable"/> interface, its
/// <see cref="IDisposable.Dispose">Dispose</see> method should be called when it is no longer needed.
/// </remarks>
public interface IConnectionDataHandle : IDisposable
{
    /// <summary>
    /// Gets the read-only memory object owned by the handle.
    /// </summary>
    ReadOnlyMemory<byte> Memory { get; }
}
