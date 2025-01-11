namespace SharpRacer.Interop;
internal interface IDataReadyEventFactory
{
    AutoResetEvent CreateAutoResetEvent(bool initialState = false);
}
