using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
internal interface IConnectionAcquisitionHandler
{
    IConnectionAcquisitionWorker CreateWorker(
        IOpenInnerConnectionOwner connectionOwner,
        IOpenInnerConnectionFactory connectionFactory,
        IConnectionProvider connectionProvider,
        IDataReadyEventFactory dataReadyEventFactory);

    void SetConnection(IConnectionProvider connectionProvider, IOpenInnerConnection innerConnection);
    void SetConnectionException(IConnectionProvider connectionProvider, SimulatorConnectionException connectionException);
    bool TryAbort();
}
