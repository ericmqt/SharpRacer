using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataSpanHandleTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var ownerMock = mocks.Create<IConnectionDataSpanOwner>();

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var ownerToken = new ConnectionDataSpanHandleToken(123);

        var handle = new ConnectionDataSpanHandle(ownerMock.Object, ownerToken, memoryObj.Span);

        Assert.True(handle.IsOwned);
        Assert.Equal(ownerMock.Object, handle.Owner);
        Assert.Equal(ownerToken, handle.Token);
        Assert.True(handle.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Ownerless_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var handle = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        Assert.False(handle.IsOwned);
        Assert.Null(handle.Owner);
        Assert.Equal(ConnectionDataSpanHandleToken.Zero, handle.Token);
        Assert.True(handle.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Dispose_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var handleToken = new ConnectionDataSpanHandleToken(123);

        int ownerReturnCount = 0;
        ulong expectedTokenId = 2;
        var fakeOwner = new FakeConnectionDataSpanOwner(expectedTokenId, memoryObj, onOwnerReturned);

        var handle = fakeOwner.AcquireSpanHandle();

        Assert.True(handle.IsOwned);
        Assert.Equal(fakeOwner, handle.Owner);
        Assert.Equal(0, ownerReturnCount);

        // Return the owner to the pool
        handle.Dispose();

        Assert.Equal(1, ownerReturnCount);

        void onOwnerReturned(ConnectionDataSpanHandleToken ownerToken)
        {
            Assert.Equal(expectedTokenId, ownerToken.Id);
            ownerReturnCount++;
        }
    }

    [Fact]
    public void Dispose_OwnerlessTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var handle = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        handle.Dispose();
    }

    [Fact]
    public void SpanImplicitConversionTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var owner = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        ReadOnlySpan<byte> converted = owner;

        Assert.True(converted.SequenceEqual(memoryObj.Span));
    }

    private class FakeConnectionDataSpanOwner : IConnectionDataSpanOwner
    {
        private ulong _tokenId;
        private readonly Memory<byte> _memory;
        private readonly Action<ConnectionDataSpanHandleToken> _onReturn;

        public FakeConnectionDataSpanOwner(ulong tokenId, Memory<byte> memory, Action<ConnectionDataSpanHandleToken> onReturn)
        {
            _tokenId = tokenId;
            _memory = memory;
            _onReturn = onReturn ?? throw new ArgumentNullException(nameof(onReturn));
        }

        public bool IsClosed { get; }
        public bool IsDisposed { get; }

        public void Close()
        {

        }

        public void Dispose()
        {

        }

        public ConnectionDataSpanHandle AcquireSpanHandle()
        {
            return new ConnectionDataSpanHandle(this, new ConnectionDataSpanHandleToken(_tokenId), _memory.Span);
        }

        public void ReleaseSpanHandle(ref readonly ConnectionDataSpanHandle owner)
        {
            _onReturn(owner.Token);
        }
    }
}
