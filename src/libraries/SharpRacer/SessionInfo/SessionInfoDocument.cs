namespace SharpRacer.SessionInfo;

/// <summary>
/// Encapsulates the session information YAML document and its associated version.
/// </summary>
public class SessionInfoDocument
{
    /// <summary>
    /// Creates an instance of <see cref="SessionInfoDocument"/> from the specified YAML document string and version number.
    /// </summary>
    /// <param name="yamlDocument">The session information YAML document.</param>
    /// <param name="version">The session information version.</param>
    public SessionInfoDocument(string yamlDocument, int version)
    {
        YamlDocument = yamlDocument ?? string.Empty;
        Version = version;
    }

    /// <summary>
    /// Gets the version number of the session information document.
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// Gets the session information YAML document as provided by the simulator.
    /// </summary>
    public string YamlDocument { get; }
}
