using System.Diagnostics.CodeAnalysis;
using SharpRacer.Internal.Connections.Requests;

namespace SharpRacer.Internal.Connections;
internal interface IConnectionProvider
{
    void Connect(IOuterConnection outerConnection);
    void Connect(IOuterConnection outerConnection, TimeSpan timeout);
    Task ConnectAsync(IOuterConnection outerConnection, CancellationToken cancellationToken = default);
    Task ConnectAsync(
        IOuterConnection outerConnection,
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    bool TryGetConnection(
        IAsyncConnectionRequest asyncConnectionRequest,
        [NotNullWhen(true)] out IOpenInnerConnection? innerConnection);

    bool TryGetConnection(
        TimeSpan waitTimeout,
        bool allowCreateConnection,
        [NotNullWhen(true)] out IOpenInnerConnection? innerConnection);
}
