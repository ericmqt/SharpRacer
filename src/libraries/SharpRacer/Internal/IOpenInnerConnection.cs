namespace SharpRacer.Internal;
internal interface IOpenInnerConnection : ISimulatorInnerConnection
{
    IDisposable AcquireDataFileLifetimeHandle();
}
