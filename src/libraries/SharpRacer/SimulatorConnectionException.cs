namespace SharpRacer;

public class SimulatorConnectionException : Exception
{
    public SimulatorConnectionException(string message)
        : base(message)
    {

    }

    public SimulatorConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}
