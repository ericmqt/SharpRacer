namespace SharpRacer.IO.Internal;

internal interface IDataFileSpanPool : IDisposable
{
    void Close();
    DataFileSpanOwner Rent();
    void Return(ref readonly DataFileSpanOwner owner);
}
