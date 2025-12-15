namespace SharpRacer.Internal.Connections;
internal interface IConnectionWorkerThreadFactory
{
    IConnectionWorkerThread Create(IConnectionWorkerThreadOwner owner, TimeProvider timeProvider);
}
