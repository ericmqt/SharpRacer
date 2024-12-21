namespace SharpRacer.Internal.Connections;
internal interface IOuterConnectionTracker
{
    bool CanAttach { get; set; }

    bool Attach(ISimulatorOuterConnection outerConnection);
    bool Detach(ISimulatorOuterConnection connection, out bool isInnerConnectionOrphaned);
    IEnumerable<ISimulatorOuterConnection> DetachAll();
}
