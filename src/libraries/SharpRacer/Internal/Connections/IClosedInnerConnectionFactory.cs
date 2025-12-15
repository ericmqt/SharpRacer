using SharpRacer.IO.Internal;

namespace SharpRacer.Internal.Connections;
internal interface IClosedInnerConnectionFactory
{
    IClosedInnerConnection CreateClosedInnerConnection(IConnectionDataFile dataFile);
    IClosedInnerConnection CreateClosedInnerConnection(IOpenInnerConnection openConnection);
}
