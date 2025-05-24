namespace SharpRacer.Internal.Connections;
internal interface IInnerConnectionFactory
{
    IInnerConnection CreateClosedInnerConnection();
    IOpenInnerConnection CreateOpenInnerConnection();
}
