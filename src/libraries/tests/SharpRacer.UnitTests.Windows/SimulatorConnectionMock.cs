using Moq;
using SharpRacer.Internal;
using SharpRacer.Internal.Connections;

namespace SharpRacer;

internal class SimulatorConnectionMock
{
    public SimulatorConnectionMock()
        : this(new MockRepository(MockBehavior.Strict))
    {

    }

    public SimulatorConnectionMock(MockRepository mockRepository)
    {
        MockRepository = mockRepository ?? throw new ArgumentNullException(nameof(mockRepository));

        CancellationTokenSource = MockRepository.Create<IConnectionCancellationTokenSource>();
        ConnectionManager = MockRepository.Create<IConnectionManager>();
        TelemetryVariableInfoProvider = MockRepository.Create<IConnectionTelemetryVariableInfoProvider>();
    }

    public Mock<IConnectionCancellationTokenSource> CancellationTokenSource { get; }
    public Mock<IConnectionManager> ConnectionManager { get; }
    public Mock<IConnectionTelemetryVariableInfoProvider> TelemetryVariableInfoProvider { get; }
    public MockRepository MockRepository { get; }

    public Mock<T> Create<T>()
            where T : class
    {
        return MockRepository.Create<T>();
    }

    public SimulatorConnection CreateInstance()
    {
        return new SimulatorConnection(ConnectionManager.Object, TelemetryVariableInfoProvider.Object, CancellationTokenSource.Object);
    }
}
