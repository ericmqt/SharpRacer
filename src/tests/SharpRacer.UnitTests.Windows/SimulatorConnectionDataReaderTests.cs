using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SharpRacer.Internal.Connections;
using SharpRacer.IO;

namespace SharpRacer;

public class SimulatorConnectionDataReaderTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new SimulatorConnectionMock();

        var dataHandleMock = mocks.MockRepository.Create<IConnectionDataHandle>();

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        openInnerConnectionMock.Setup(x => x.AcquireDataHandle()).Returns(dataHandleMock.Object);

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Open the connection
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        var reader = new SimulatorConnectionDataReader(connection);

        openInnerConnectionMock.Verify(x => x.AcquireDataHandle(), Times.Once);
    }

    [Fact]
    public void Ctor_ThrowIfConnectionIsNotOpenTest()
    {
        var connection = new SimulatorConnection();

        Assert.Throws<ArgumentException>(() => new SimulatorConnectionDataReader(connection));
    }

    [Fact]
    public void Dispose_Test()
    {
        var mocks = new SimulatorConnectionMock();

        var dataHandleMock = mocks.MockRepository.Create<IConnectionDataHandle>();
        dataHandleMock.Setup(x => x.Dispose());

        var openInnerConnectionMock = mocks.MockRepository.Create<IOpenInnerConnection>();

        openInnerConnectionMock.Setup(x => x.AcquireDataHandle()).Returns(dataHandleMock.Object);

        // Create SimulatorConnection object and get the IOuterConnection implementation
        var connection = mocks.CreateInstance();
        var outerConnection = (IOuterConnection)connection;

        mocks.ConnectionManager.Setup(x => x.Connect(It.IsAny<IOuterConnection>(), It.IsAny<TimeSpan>()));

        mocks.DataVariableInfoProvider.Setup(x => x.OnTelemetryVariablesActivated(It.IsAny<ISimulatorConnection>()));

        // Open the connection
        connection.Open();
        outerConnection.SetOpenInnerConnection(openInnerConnectionMock.Object);

        var reader = new SimulatorConnectionDataReader(connection);
        reader.Dispose();

        dataHandleMock.Verify(x => x.Dispose(), Times.Once);
        openInnerConnectionMock.Verify(x => x.AcquireDataHandle(), Times.Once);
    }
}
