namespace SharpRacer.Internal.Connections.Requests;
internal interface IConnectionRequestScopeFactory
{
    ConnectionRequest.Scope CreateScope();
    AsyncConnectionRequestScope CreateAsyncScope();
}
