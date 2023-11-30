using Microsoft.CodeAnalysis;

namespace SharpRacer.SourceGenerators;
public static class DiagnosticComparer
{
    public sealed class IdAndLocationComparer : IEqualityComparer<Diagnostic?>
    {
        private IdAndLocationComparer() { }

        public static IEqualityComparer<Diagnostic?> Default { get; } = new IdAndLocationComparer();

        public bool Equals(Diagnostic? x, Diagnostic? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Id == y.Id && x.Location == y.Location;
        }

        public int GetHashCode(Diagnostic? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.Id);
            hc.Add(obj.Location);

            return hc.ToHashCode();
        }
    }
}
