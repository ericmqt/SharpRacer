namespace SharpRacer.Commands.ForceFeedback;

/// <summary>
/// Defines methods for sending force-feedback commands to the simulator.
/// </summary>
public interface IForceFeedbackCommandClient
{
    /// <summary>
    /// Sets the maximum force when mapping steering wheel torque to direct input units.
    /// </summary>
    /// <param name="forceNm">The maximum force in Newton-meters.</param>
    void SetMaxForce(float forceNm);
}
