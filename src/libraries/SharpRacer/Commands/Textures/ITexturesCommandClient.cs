namespace SharpRacer.Commands.Textures;

/// <summary>
/// Defines methods for sending texture commands to the simulator.
/// </summary>
public interface ITexturesCommandClient
{
    /// <summary>
    /// Reloads custom textures for all cars.
    /// </summary>
    void ReloadAll();

    /// <summary>
    /// Reloads the custom texture for the car at the specified index.
    /// </summary>
    /// <param name="carIndex">The car index.</param>
    void ReloadCarAtIndex(int carIndex);
}
