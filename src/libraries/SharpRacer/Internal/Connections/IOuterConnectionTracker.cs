namespace SharpRacer.Internal.Connections;
internal interface IOuterConnectionTracker
{
    bool CloseOnEmpty { get; }
    bool HasTrackedConnections { get; }
    bool IsClosed { get; }
    bool IsEmpty { get; }

    bool Attach(IOuterConnection outerConnection);
    void Close();
    bool Detach(IOuterConnection connection);
    IEnumerable<IOuterConnection> DetachAll();
}
