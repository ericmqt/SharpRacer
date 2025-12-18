using Moq;

namespace SharpRacer.IO.Internal;
public class ConnectionDataSpanHandleTests
{
    [Fact]
    public void Ctor_Test()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var poolMock = mocks.Create<IConnectionDataSpanOwner>();

        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var ownerToken = new ConnectionDataSpanHandleToken(123);

        var owner = new ConnectionDataSpanHandle(poolMock.Object, ownerToken, memoryObj.Span);

        Assert.True(owner.IsOwned);
        Assert.Equal(poolMock.Object, owner.Pool);
        Assert.Equal(ownerToken, owner.Token);
        Assert.True(owner.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Ownerless_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var owner = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        Assert.False(owner.IsOwned);
        Assert.Null(owner.Pool);
        Assert.Equal(ConnectionDataSpanHandleToken.Empty, owner.Token);
        Assert.True(owner.Span.SequenceEqual(memoryObj.Span));
    }

    [Fact]
    public void Dispose_Test()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);
        var ownerToken = new ConnectionDataSpanHandleToken(123);

        int ownerReturnCount = 0;
        ulong expectedTokenId = 2;
        var fakePool = new FakeDataFileSpanPool(expectedTokenId, memoryObj, onOwnerReturned);

        var owner = fakePool.Rent();

        Assert.True(owner.IsOwned);
        Assert.Equal(fakePool, owner.Pool);
        Assert.Equal(0, ownerReturnCount);

        // Return the owner to the pool
        owner.Dispose();

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

        var owner = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        owner.Dispose();
    }

    [Fact]
    public void SpanImplicitConversionTest()
    {
        var memoryObj = new Memory<byte>([0xDE, 0xAD, 0xBE, 0xEF]);

        var owner = ConnectionDataSpanHandle.Ownerless(memoryObj.Span);

        ReadOnlySpan<byte> converted = owner;

        Assert.True(converted.SequenceEqual(memoryObj.Span));
    }

    private class FakeDataFileSpanPool : IConnectionDataSpanOwner
    {
        private ulong _tokenId;
        private readonly Memory<byte> _memory;
        private readonly Action<ConnectionDataSpanHandleToken> _onReturn;

        public FakeDataFileSpanPool(ulong tokenId, Memory<byte> memory, Action<ConnectionDataSpanHandleToken> onReturn)
        {
            _tokenId = tokenId;
            _memory = memory;
            _onReturn = onReturn ?? throw new ArgumentNullException(nameof(onReturn));
        }

        public void Close()
        {

        }

        public void Dispose()
        {

        }

        public ConnectionDataSpanHandle Rent()
        {
            return new ConnectionDataSpanHandle(this, new ConnectionDataSpanHandleToken(_tokenId), _memory.Span);
        }

        public void Return(ref readonly ConnectionDataSpanHandle owner)
        {
            _onReturn(owner.Token);
        }
    }
}
