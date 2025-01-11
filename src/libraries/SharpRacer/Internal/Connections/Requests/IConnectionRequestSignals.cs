namespace SharpRacer.Internal.Connections.Requests;
internal interface IConnectionRequestSignals
{
    void AllowAsyncRequestCreation();
    void AllowRequestExecution();
    void BlockAsyncRequestCreation();
    void BlockRequestExecution();
    void WaitForAsyncRequestCreation();
    void WaitForRequestExecution();
}
