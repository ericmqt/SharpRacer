using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.Testing.Text;
public class NullSourceText : SourceText
{
    public override char this[int position] => '\0';

    public override Encoding? Encoding { get; }
    public override int Length { get; } = 0;

    public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
    {
        string.Empty.CopyTo(sourceIndex, destination, destinationIndex, count);
    }

    public override string ToString()
    {
        return null!;
    }
}
