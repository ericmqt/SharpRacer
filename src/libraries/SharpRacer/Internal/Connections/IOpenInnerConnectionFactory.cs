namespace SharpRacer.Internal.Connections;
internal interface IOpenInnerConnectionFactory
{
    IOpenInnerConnection Create(IOpenInnerConnectionOwner owner);
}
