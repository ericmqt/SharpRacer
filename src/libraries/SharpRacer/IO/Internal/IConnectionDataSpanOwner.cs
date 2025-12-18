namespace SharpRacer.IO.Internal;

internal interface IConnectionDataSpanOwner : IDisposable
{
    void Close();
    ConnectionDataSpanHandle Rent();
    void Return(ref readonly ConnectionDataSpanHandle owner);
}
