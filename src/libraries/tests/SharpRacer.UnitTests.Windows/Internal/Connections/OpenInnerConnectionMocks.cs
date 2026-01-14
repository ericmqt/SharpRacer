using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
internal class OpenInnerConnectionMocks
{
    public OpenInnerConnectionMocks(MockRepository mockRepository)
        : this(mockRepository, new FakeTimeProvider())
    {

    }

    public OpenInnerConnectionMocks(MockRepository mockRepository, TimeProvider timeProvider)
    {
        MockRepository = mockRepository ?? throw new ArgumentNullException(nameof(mockRepository));

        TimeProvider = timeProvider;

        ClosedConnectionFactory = MockRepository.Create<IClosedInnerConnectionFactory>();
        ClosedInnerConnection = MockRepository.Create<IClosedInnerConnection>();
        ConnectionOwner = MockRepository.Create<IOpenInnerConnectionOwner>();
        DataFile = MockRepository.Create<IConnectionDataFile>();
        OuterConnectionTracker = MockRepository.Create<IOuterConnectionTracker>();
        WorkerThread = MockRepository.Create<IConnectionWorkerThread>();
        WorkerThreadFactory = MockRepository.Create<IConnectionWorkerThreadFactory>();

        // Configure sane defaults
        ClosedConnectionFactory.Setup(x => x.CreateClosedInnerConnection(It.IsAny<IOpenInnerConnection>()))
            .Returns(ClosedInnerConnection.Object);

        OuterConnectionTracker.SetupGet(x => x.CloseOnEmpty).Returns(true);

        WorkerThreadFactory.Setup(x => x.Create(It.IsAny<IConnectionWorkerThreadOwner>(), It.IsAny<TimeProvider>()))
            .Returns(WorkerThread.Object);
    }

    public OpenInnerConnectionMocks(MockBehavior mockBehavior = MockBehavior.Strict)
        : this(mockBehavior, new FakeTimeProvider())
    {

    }

    public OpenInnerConnectionMocks(MockBehavior mockBehavior, TimeProvider timeProvider)
        : this(new MockRepository(mockBehavior), timeProvider)
    {
        
    }

    public Mock<IClosedInnerConnection> ClosedInnerConnection { get; }
    public Mock<IClosedInnerConnectionFactory> ClosedConnectionFactory { get; }
    public Mock<IOpenInnerConnectionOwner> ConnectionOwner { get; }
    public Mock<IConnectionDataFile> DataFile { get; }
    public MockRepository MockRepository { get; }
    public Mock<IOuterConnectionTracker> OuterConnectionTracker { get; }
    public Mock<IConnectionWorkerThread> WorkerThread { get; }
    public Mock<IConnectionWorkerThreadFactory> WorkerThreadFactory { get; }
    public TimeProvider TimeProvider { get; }

    public Mock<T> Create<T>()
            where T : class
    {
        return MockRepository.Create<T>();
    }

    public void Verify()
    {
        MockRepository.Verify();
    }
}
