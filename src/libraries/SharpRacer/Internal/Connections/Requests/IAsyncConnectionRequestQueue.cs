namespace SharpRacer.Internal.Connections.Requests;
internal interface IAsyncConnectionRequestQueue
{
    void Enqueue(IAsyncConnectionRequest request);
    bool ProcessQueue(IConnectionProvider connectionProvider, bool force = false);
    bool ProcessQueue(IConnectionProvider connectionProvider, bool force, out bool queueEmptied);
}
