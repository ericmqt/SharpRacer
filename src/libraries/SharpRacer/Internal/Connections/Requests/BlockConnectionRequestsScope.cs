namespace SharpRacer.Internal.Connections.Requests;
internal ref struct BlockConnectionRequestsScope
{
    private readonly IConnectionRequestSignals _requestSignals;
    private bool _isDisposed;

    public BlockConnectionRequestsScope(IConnectionRequestSignals requestSignals)
    {
        _requestSignals = requestSignals;

        _requestSignals.BlockRequestExecution();
        _requestSignals.BlockAsyncRequestCreation();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _requestSignals.AllowRequestExecution();
            _requestSignals.AllowAsyncRequestCreation();

            _isDisposed = true;
        }
    }
}
