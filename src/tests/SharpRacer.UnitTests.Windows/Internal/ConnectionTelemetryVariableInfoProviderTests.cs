using Moq;
using SharpRacer.Internal.Connections;
using SharpRacer.IO;
using SharpRacer.IO.Internal;
using SharpRacer.Telemetry;
using SharpRacer.Testing.IO;
using SharpRacer.Testing.Telemetry;

namespace SharpRacer.Internal;

public class ConnectionTelemetryVariableInfoProviderTests
{
    [Fact]
    public void NotifyTelemetryVariableActivated_CallbackInvokedWhenAlreadyInitializedTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the test object
        var variableInfoProvider = new ConnectionTelemetryVariableInfoProvider();

        // Create the outer connection object
        var connection = new SimulatorConnection(
            connectionMocks.ConnectionManager.Object, variableInfoProvider, connectionMocks.CancellationTokenSource.Object);

        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state, which will update the variables on the provider
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);
        Assert.NotEmpty(fakeVariableHeaders.Headers);

        // Attempt to register a callback for one of our variables, knowing that the provider has already been initialized by the conn.
        var callbackVariableHeader = fakeVariableHeaders.Headers.First();
        var variableCallbackInvoked = false;

        variableInfoProvider.NotifyTelemetryVariableActivated(callbackVariableHeader.Name.ToString(), onVariableCallbackInvoked);

        Assert.True(variableCallbackInvoked);

        void onVariableCallbackInvoked(TelemetryVariableInfo variableInfo)
        {
            variableCallbackInvoked = true;
            Assert.Equal(callbackVariableHeader.Name.ToString(), variableInfo.Name);
        }
    }

    [Fact]
    public void OnTelemetryVariablesActivatedTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the test object
        var variableInfoProvider = new ConnectionTelemetryVariableInfoProvider();

        // Create the outer connection object
        var connection = new SimulatorConnection(
            connectionMocks.ConnectionManager.Object, variableInfoProvider, connectionMocks.CancellationTokenSource.Object);

        var outerConnection = (IOuterConnection)connection;

        // Register callbacks for the variable headers we configured
        var headerCallbackInvocations = new Dictionary<string, bool>();

        Assert.NotEmpty(fakeVariableHeaders.Headers);

        foreach (var sourceHeader in fakeVariableHeaders.Headers)
        {
            headerCallbackInvocations.Add(sourceHeader.Name.ToString(), false);

            variableInfoProvider.NotifyTelemetryVariableActivated(sourceHeader.Name.ToString(), onVariableActivated);
        }

        // Register a callback for a variable that won't exist
        variableInfoProvider.NotifyTelemetryVariableActivated("wlefkalkwehfjkahwefkajwefhkawef", onUnavailableVariableActivated);

        // Transition the outer connection to the opened connection state, which will update the variables on the provider
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        // Verify all of our callbacks were invoked
        foreach (var activationInvocation in headerCallbackInvocations)
        {
            Assert.True(
                activationInvocation.Value, $"Variable activation callback was not invoked for variable '{activationInvocation.Key}'.");
        }

        void onVariableActivated(TelemetryVariableInfo variableInfo)
        {
            Assert.NotNull(variableInfo);

            if (headerCallbackInvocations.ContainsKey(variableInfo.Name))
            {
                headerCallbackInvocations[variableInfo.Name] = true;
            }
            else
            {
                Assert.Fail($"NotifyTelemetryVariableActivated callback invoked for unexpected variable '{variableInfo.Name}'.");
            }
        }

        void onUnavailableVariableActivated(TelemetryVariableInfo variableInfo)
        {
            Assert.Fail(
                $"NotifyTelemetryVariableActivated callback invoked for variable that was not expected to be found: '{variableInfo.Name}'.");
        }
    }

    [Fact]
    public void OnTelemetryVariablesActivatedTest_SwallowsCallbackExceptionsTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the test object
        var variableInfoProvider = new ConnectionTelemetryVariableInfoProvider();

        // Create the outer connection object
        var connection = new SimulatorConnection(
            connectionMocks.ConnectionManager.Object, variableInfoProvider, connectionMocks.CancellationTokenSource.Object);

        var outerConnection = (IOuterConnection)connection;

        // Register the callback
        Assert.NotEmpty(fakeVariableHeaders.Headers);

        var callbackVariableHeader = fakeVariableHeaders.Headers.First();
        var variableCallbackInvoked = false;

        variableInfoProvider.NotifyTelemetryVariableActivated(callbackVariableHeader.Name.ToString(), onVariableCallbackInvoked);

        // Transition the outer connection to the opened connection state, which will update the variables on the provider
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        Assert.True(variableCallbackInvoked);

        void onVariableCallbackInvoked(TelemetryVariableInfo variableInfo)
        {
            variableCallbackInvoked = true;
            throw new InvalidOperationException("Whoops!");
        }
    }

    [Fact]
    public void OnTelemetryVariablesActivated_ThrowIfAlreadyInitializedTest()
    {
        var fakeVariableHeaders = new FakeTelemetryVariableHeaders();
        var fakeVariables = new FakeTelemetryVariables(fakeVariableHeaders);
        var fakeData = DataFileMemory.Create(b => b.ConfigureVariables(fakeVariableHeaders.Headers));

        var fakeMmf = new FakeMemoryMappedDataFile(fakeData.Memory);
        var connectionDataFile = new ConnectionDataFile(fakeMmf);
        var outerConnectionTracker = new OuterConnectionTracker(closeOnEmpty: true);

        var mocks = new MockRepository(MockBehavior.Strict);

        // Configure the open inner connection mocks so we can expose our data file to the outer connection
        var openInnerConnectionMocks = new OpenInnerConnectionMocks(mocks);

        openInnerConnectionMocks.ConnectionOwner.Setup(x => x.NewConnectionId()).Returns(1);

        var openInnerConnection = new OpenInnerConnection(
            openInnerConnectionMocks.ConnectionOwner.Object,
            connectionDataFile,
            outerConnectionTracker,
            openInnerConnectionMocks.ClosedConnectionFactory.Object,
            openInnerConnectionMocks.WorkerThreadFactory.Object,
            TimeProvider.System);

        // Configure the outer connection
        var connectionMocks = new SimulatorConnectionMock(mocks);

        connectionMocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));
        connectionMocks.TelemetryVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Create the test object
        var variableInfoProvider = new ConnectionTelemetryVariableInfoProvider();

        // Create the outer connection object
        var connection = new SimulatorConnection(
            connectionMocks.ConnectionManager.Object, variableInfoProvider, connectionMocks.CancellationTokenSource.Object);

        var outerConnection = (IOuterConnection)connection;

        // Transition the outer connection to the opened connection state, which will update the variables on the provider
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnection);

        Assert.Equal(SimulatorConnectionState.Open, connection.State);

        Assert.Throws<InvalidOperationException>(() => variableInfoProvider.OnTelemetryVariablesActivated(connection));
    }
}
