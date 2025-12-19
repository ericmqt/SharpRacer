using Moq;
using SharpRacer.Internal;
using SharpRacer.Internal.Connections;

namespace SharpRacer;
internal class SimulatorConnectionMock
{
    public SimulatorConnectionMock()
    {
        MockRepository = new MockRepository(MockBehavior.Strict);

        CancellationTokenSource = MockRepository.Create<IConnectionCancellationTokenSource>();
        ConnectionManager = MockRepository.Create<IConnectionManager>();
        DataVariableInfoProvider = MockRepository.Create<IConnectionDataVariableInfoProvider>();
    }

    public Mock<IConnectionCancellationTokenSource> CancellationTokenSource { get; }
    public Mock<IConnectionManager> ConnectionManager { get; }
    public Mock<IConnectionDataVariableInfoProvider> DataVariableInfoProvider { get; }
    public MockRepository MockRepository { get; }

    public Mock<T> Create<T>()
            where T : class
    {
        return MockRepository.Create<T>();
    }

    public SimulatorConnection CreateInstance()
    {
        return new SimulatorConnection(ConnectionManager.Object, DataVariableInfoProvider.Object, CancellationTokenSource.Object);
    }
}
