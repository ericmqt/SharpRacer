namespace SharpRacer.Internal;
internal interface ISimulatorOuterConnection
{
    void SetClosedInnerConnection(ISimulatorInnerConnection internalConnection);
    void SetOpenInnerConnection(ISimulatorInnerConnection internalConnection, IDisposable dataFileLifetimeHandle);
}
