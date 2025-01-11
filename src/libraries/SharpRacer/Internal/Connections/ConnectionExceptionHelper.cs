using System.Diagnostics;

namespace SharpRacer.Internal.Connections;
internal static class ConnectionExceptionHelper
{
    [StackTraceHidden]
    internal static SimulatorConnectionException FailedToConnect(Exception innerException)
    {
        return GetConnectionException("Failed to connect to the simulator. See inner exception for details.", innerException);
    }

    [StackTraceHidden]
    internal static SimulatorConnectionException FailedToConnect_SimulatorNotAvailable()
    {
        return GetConnectionException("Failed to connect to the simulator because the simulator is not available.");
    }

    [StackTraceHidden]
    internal static SimulatorConnectionException GetConnectionException(string message, Exception? innerException = null)
    {
        try
        {
            if (innerException is null)
            {
                throw new SimulatorConnectionException(message);
            }

            throw new SimulatorConnectionException(message, innerException);
        }
        catch (SimulatorConnectionException connEx)
        {
            return connEx;
        }
    }
}
