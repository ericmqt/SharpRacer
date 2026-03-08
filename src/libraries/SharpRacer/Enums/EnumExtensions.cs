namespace SharpRacer.Enums;

/// <summary>
/// Provides extension methods for enumerations in the <see cref="SharpRacer.Enums"/> namespace.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the <see cref="IncidentPenalty"/> value from the specified <see cref="IncidentFlags"/> value.
    /// </summary>
    /// <param name="incidentFlags">The <see cref="IncidentFlags"/> value from which the incident penalty will be retrieved.</param>
    /// <returns>
    /// The <see cref="IncidentPenalty"/> value encoded in the second byte of the <see cref="IncidentFlags"/> value.
    /// </returns>
    public static IncidentPenalty GetIncidentPenalty(this IncidentFlags incidentFlags)
    {
        return (IncidentPenalty)((uint)incidentFlags & 0xFF00);
    }

    /// <summary>
    /// Retrieves the <see cref="IncidentType"/> value from the specified <see cref="IncidentFlags"/> value.
    /// </summary>
    /// <param name="incidentFlags">The <see cref="IncidentFlags"/> value from which the reported incident will be retrieved.</param>
    /// <returns>
    /// The <see cref="IncidentType"/> value encoded in the first byte of the <see cref="IncidentFlags"/> value.
    /// </returns>
    public static IncidentType GetIncidentType(this IncidentFlags incidentFlags)
    {
        return (IncidentType)((uint)incidentFlags & 0xFF);
    }
}
