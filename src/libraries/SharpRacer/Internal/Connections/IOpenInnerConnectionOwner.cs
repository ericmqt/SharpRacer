namespace SharpRacer.Internal.Connections;
internal interface IOpenInnerConnectionOwner
{
    int NewConnectionId();
    void OnConnectionClosing(IOpenInnerConnection connectionObject);
}
