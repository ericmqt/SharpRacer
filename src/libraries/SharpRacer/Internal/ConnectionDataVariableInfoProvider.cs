﻿using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SharpRacer.Interop;
using SharpRacer.Telemetry;

namespace SharpRacer.Internal;
internal class ConnectionDataVariableInfoProvider : IDataVariableInfoProvider
{
    private readonly Dictionary<string, ConcurrentQueue<Action<DataVariableInfo>>> _callbacks = [];
    private readonly ReaderWriterLockSlim _callbacksLock = new();
    private bool _isInitialized;
    private readonly List<DataVariableInfo> _variables = [];
    private readonly ReaderWriterLockSlim _variablesLock = new();

    public ConnectionDataVariableInfoProvider()
    {

    }

    public IEnumerable<DataVariableInfo> DataVariables => _variables;

    public void NotifyDataVariableActivated(string variableName, Action<DataVariableInfo> callback)
    {
        ArgumentException.ThrowIfNullOrEmpty(variableName);
        ArgumentNullException.ThrowIfNull(callback);

        // Acquire a read lock for the variables collection to prevent initialization occurring on another thread
        _variablesLock.EnterReadLock();

        try
        {
            // Only register callback if variables haven't been initialized, otherwise callback would never fire because the queue is only
            // processed when variables are initialized.

            if (!_isInitialized)
            {
                RegisterDataVariableActivatedCallback(variableName, callback);
            }
            else
            {
                var variableInfo = _variables.FindByName(variableName);

                if (variableInfo != null)
                {
                    callback(variableInfo);
                }
            }
        }
        finally
        {
            _variablesLock.ExitReadLock();
        }
    }

    internal void InitializeVariables(ISimulatorConnection connection)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException("Telemetry variables have already been initialized.");
        }

        /*
         * A note on locks:
         * 
         * When this method acquires a write lock on the variables collection, we are guaranteed to have no other threads currently holding
         * a read lock. Calls to NotifyDataVariableActivated are then blocked prior to checking whether the variable collection has
         * been initialized or subsequently acquiring a read/write lock on the callbacks dictionary and scheduling callbacks.
         * 
         * As long as this method holds a write lock on the variables collection, we are free to modify either collection while only
         * needing to hold the write lock for the variables collection.
         */

        _variablesLock.EnterWriteLock();

        try
        {
            // Read variable headers from connection
            ref readonly var header = ref MemoryMarshal.AsRef<DataFileHeader>(connection.Data[..DataFileHeader.Size]);

            var variableHeaders = new DataVariableHeader[header.VariableCount];
            var variableHeaderBytes = connection.Data.Slice(header.VariableHeaderOffset, DataVariableHeader.Size * header.VariableCount);

            variableHeaderBytes.CopyTo(MemoryMarshal.AsBytes<DataVariableHeader>(variableHeaders));

            // Add variables to collection
            foreach (var varHeader in variableHeaders)
            {
                _variables.Add(new DataVariableInfo(varHeader));
            }

            // Invoke registered callbacks for active variables
            foreach (var variableName in _callbacks.Keys)
            {
                if (!_variables.TryFindByName(variableName, out var variableInfo))
                {
                    continue;
                }

                var callbackQueue = _callbacks[variableName];

                while (callbackQueue.TryDequeue(out var callback))
                {
                    try { callback(variableInfo); }
                    catch
                    {
                        // Swallow the exception. Do not allow a callback that throws an exception prevent executing remaining callbacks.
                    }
                }
            }

            _isInitialized = true;
        }
        finally
        {
            _variablesLock.ExitWriteLock();
        }
    }

    private void RegisterDataVariableActivatedCallback(string variableName, Action<DataVariableInfo> callback)
    {
        ArgumentException.ThrowIfNullOrEmpty(variableName);
        ArgumentNullException.ThrowIfNull(callback);

        _callbacksLock.EnterUpgradeableReadLock();

        try
        {
            if (!_callbacks.TryGetValue(variableName, out var callbackQueue))
            {
                _callbacksLock.EnterWriteLock();

                try
                {
                    // Avoid attempting to add a duplicate key by checking the dictionary again in case another thread managed to
                    // acquire a write lock just before we did.

                    if (!_callbacks.TryGetValue(variableName, out callbackQueue))
                    {
                        callbackQueue = new ConcurrentQueue<Action<DataVariableInfo>>();

                        _callbacks.Add(variableName, callbackQueue);
                    }
                }
                finally
                {
                    _callbacksLock.ExitWriteLock();
                }
            }

            callbackQueue.Enqueue(callback);
        }
        finally
        {
            _callbacksLock.ExitUpgradeableReadLock();
        }
    }
}
