namespace SharpRacer.IO.Internal;
internal interface IConnectionDataFileLifetime
{
    IConnectionDataFileLifetimeHandle AcquireLifetimeHandle();
    void ReleaseLifetimeHandle(IConnectionDataFileLifetimeHandle handle);
}
