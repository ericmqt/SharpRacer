namespace SharpRacer.Internal.Connections;
internal interface IConnectionWorkerThread : IDisposable
{
    Thread Thread { get; }

    void Start();
    void Stop();
}
