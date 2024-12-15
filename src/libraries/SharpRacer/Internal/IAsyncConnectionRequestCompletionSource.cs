namespace SharpRacer.Internal;
internal interface IAsyncConnectionRequestCompletionSource
{
    bool TryCompleteRequest(AsyncConnectionRequest request);
}
