using Microsoft.Win32.SafeHandles;
using SharpRacer.Interop;

namespace SharpRacer.Internal;
partial class ConnectionPool
{
    private sealed class DataReadyCallback : IDisposable
    {
        private bool _allowExecute;
        private readonly Action<DataReadyCallback, bool> _callback;
        private readonly SafeWaitHandle _dataReadyEventWaitHandle;
        private readonly ManualResetEvent _dataReadyEvent;
        private readonly SemaphoreSlim _executeSempahore;
        private bool _isDisposed;
        private readonly RegisteredWaitHandle _registeredWaitHandle;

        private DataReadyCallback(
            Action<DataReadyCallback, bool> callback,
            TimeSpan timeout)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));

            _allowExecute = true;
            _executeSempahore = new SemaphoreSlim(1, 1);

            _dataReadyEventWaitHandle = DataReadyEventHandle.CreateSafeWaitHandle();
            _dataReadyEvent = new ManualResetEvent(initialState: false) { SafeWaitHandle = _dataReadyEventWaitHandle };

            _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                _dataReadyEvent, OnCallbackInvoked, this, timeout, executeOnlyOnce: false);
        }

        public static DataReadyCallback StartNew(Action<DataReadyCallback, bool> callback, TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(callback);

            return new DataReadyCallback(callback, timeout);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _allowExecute = false;
                _registeredWaitHandle.Unregister(null);
                _dataReadyEvent.Dispose();
                _dataReadyEventWaitHandle.Dispose();

                _isDisposed = true;
            }
        }

        private void OnCallbackInvoked(object? state, bool timedOut)
        {
            var callbackState = (DataReadyCallback)state!;

            _executeSempahore.Wait();

            try
            {
                if (!callbackState._allowExecute)
                {
                    return;
                }

                _callback(callbackState, timedOut);
            }
            finally
            {
                _executeSempahore.Release();
            }
        }
    }
}
