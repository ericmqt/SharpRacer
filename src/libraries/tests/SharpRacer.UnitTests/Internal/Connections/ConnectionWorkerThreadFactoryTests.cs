using Microsoft.Extensions.Time.Testing;
using Moq;
using SharpRacer.Interop;

namespace SharpRacer.Internal.Connections;
public class ConnectionWorkerThreadFactoryTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();

        var factory = new ConnectionWorkerThreadFactory(dataReadyEventFactoryMock.Object);

        Assert.NotNull(factory);
    }

    [Fact]
    public void Ctor_ThrowsOnNullArgTest()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionWorkerThreadFactory(null!));
    }

    [Fact]
    public void Create_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var dataReadyEventFactoryMock = mocks.Create<IDataReadyEventFactory>();
        var ownerMock = mocks.Create<IConnectionWorkerThreadOwner>();

        var factory = new ConnectionWorkerThreadFactory(dataReadyEventFactoryMock.Object);

        var worker = factory.Create(ownerMock.Object, new FakeTimeProvider());

        Assert.NotNull(worker);
    }
}
