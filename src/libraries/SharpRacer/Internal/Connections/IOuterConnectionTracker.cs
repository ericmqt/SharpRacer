namespace SharpRacer.Internal.Connections;
internal interface IOuterConnectionTracker
{
    bool CanAttach { get; }

    bool Attach(IOuterConnection outerConnection);
    bool Detach(IOuterConnection connection, out bool isInnerConnectionOrphaned);
    IEnumerable<IOuterConnection> DetachAll();
    void DisableAttach();
}
