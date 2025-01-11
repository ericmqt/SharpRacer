namespace SharpRacer.Internal.Connections;
internal interface IConnectionObjectManager
{
    bool ClearConnection(IOpenInnerConnection innerConnection);
    void ClearConnectionException();
    IOpenInnerConnection? GetConnection();
    SimulatorConnectionException? GetConnectionException();
    void SetConnection(IOpenInnerConnection innerConnection);
    void SetConnectionException(SimulatorConnectionException connectionException);
}
