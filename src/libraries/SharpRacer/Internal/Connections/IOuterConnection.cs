namespace SharpRacer.Internal.Connections;
internal interface IOuterConnection
{
    void SetClosedInnerConnection(IInnerConnection closedInnerConnection);
    void SetOpenInnerConnection(IOpenInnerConnection openInnerConnection);
}
