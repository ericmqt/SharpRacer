using System.Text;

namespace SharpRacer.Interop;

public class SessionInfoStringTests
{
    [Fact]
    public void EncodingIsLatin1Test()
    {
        Assert.Equal(Encoding.Latin1, SessionInfoString.Encoding);
    }
}
