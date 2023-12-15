using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace SharpRacer.SourceGenerators.Testing.Text;
public class JsonSourceText : SourceText
{
    private readonly string _json;

    public JsonSourceText(string json)
    {
        _json = json;
        Length = _json.Length;
        Encoding = Encoding.UTF8;
    }

    public override char this[int position] => _json[position];

    public override Encoding? Encoding { get; }
    public override int Length { get; }

    public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
    {
        _json.CopyTo(sourceIndex, destination, destinationIndex, count);
    }
}
