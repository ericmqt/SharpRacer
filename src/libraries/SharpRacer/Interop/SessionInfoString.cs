using System.Text;

namespace SharpRacer.Interop;

/// <summary>
/// Assists with reading the session information string and its version number from a simulator connection.
/// </summary>
public static class SessionInfoString
{
    /// <summary>
    /// Gets the encoding for the session info string.
    /// </summary>
    public static Encoding Encoding => Encoding.Latin1;

    /// <summary>
    /// Gets a read-only span of bytes from the specified connection that represents the session information string.
    /// </summary>
    /// <param name="connection">
    /// The <see cref="ISimulatorConnection" /> instance from which to obtain the read-only span over the session information string.
    /// </param>
    /// <returns>A read-only span of bytes over the session information string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="connection"/> is not in a state that supports reading.</exception>
    public static ReadOnlySpan<byte> GetSpan(ISimulatorConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!connection.CanRead)
        {
            throw new InvalidOperationException($"'{nameof(connection)}' is not in a state that supports reading.");
        }

        using var dataHandle = connection.RentDataSpan();

        ref readonly var dataFileHeader = ref DataFileHeader.AsRef(dataHandle);

        return dataHandle.Span.Slice(dataFileHeader.SessionInfoOffset, dataFileHeader.SessionInfoLength);
    }

    /// <summary>
    /// Reads the session information string from the specified <see cref="ISimulatorConnection" />.
    /// </summary>
    /// <param name="connection">The <see cref="ISimulatorConnection" /> instance from which to read the session information string.</param>
    /// <returns>The session information string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="connection"/> is not in a state that supports reading.</exception>
    public static string Read(ISimulatorConnection connection)
    {
        return Read(connection, out _);
    }

    /// <summary>
    /// Reads the session information string from the specified connection, returning the session information version as an out parameter.
    /// </summary>
    /// <param name="connection">The <see cref="ISimulatorConnection" /> instance from which to read the session information string.</param>
    /// <param name="version">The session information string version.</param>
    /// <returns>The session information string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="connection"/> is not in a state that supports reading.</exception>
    public static string Read(ISimulatorConnection connection, out int version)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!connection.CanRead)
        {
            throw new InvalidOperationException($"'{nameof(connection)}' is not in a state that supports reading.");
        }

        using var dataFile = connection.RentDataSpan();

        ref readonly var dataFileHeader = ref DataFileHeader.AsRef(dataFile);

        string sessionInfoStr;

        // Read the session info string, checking that the version we started with matches the version in the header after reading
        do
        {
            version = dataFileHeader.SessionInfoVersion;

            var span = dataFile.Span.Slice(dataFileHeader.SessionInfoOffset, dataFileHeader.SessionInfoLength);

            sessionInfoStr = Encoding.GetString(span);
        }
        while (version != dataFileHeader.SessionInfoVersion);

        return sessionInfoStr;
    }

    /// <summary>
    /// Reads the session information string version number from the specified connection.
    /// </summary>
    /// <param name="connection">
    /// The <see cref="ISimulatorConnection" /> instance from which to read the session information version number.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="connection"/> is not in a state that supports reading.</exception>
    public static int ReadVersion(ISimulatorConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!connection.CanRead)
        {
            throw new InvalidOperationException($"'{nameof(connection)}' is not in a state that supports reading.");
        }

        using var dataFile = connection.RentDataSpan();

        ref readonly var dataFileHeader = ref DataFileHeader.AsRef(dataFile);

        return dataFileHeader.SessionInfoVersion;
    }
}
