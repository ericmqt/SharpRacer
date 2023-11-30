namespace SharpRacer.SourceGenerators.TelemetryVariables.InputModels;
internal class VariableInfo
{
    public string? DeprecatedBy { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDeprecated { get; set; }
    public bool IsTimeSliceArray { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ValueCount { get; set; }
    public VariableValueType ValueType { get; set; }
    public string? ValueUnit { get; set; }

    internal class EqualityComparer : IEqualityComparer<VariableInfo?>
    {
        private EqualityComparer() { }

        public static IEqualityComparer<VariableInfo?> Default { get; } = new EqualityComparer();

        public bool Equals(VariableInfo? x, VariableInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return StringComparer.Ordinal.Equals(x.Name, y.Name) &&
                x.ValueCount == y.ValueCount &&
                x.ValueType == y.ValueType &&
                StringComparer.Ordinal.Equals(x.ValueUnit, y.ValueUnit) &&
                x.IsTimeSliceArray == y.IsTimeSliceArray &&
                x.IsDeprecated == y.IsDeprecated &&
                StringComparer.Ordinal.Equals(x.DeprecatedBy, y.DeprecatedBy) &&
                StringComparer.Ordinal.Equals(x.Description, y.Description);
        }

        public int GetHashCode(VariableInfo? obj)
        {
            var hc = new HashCode();

            if (obj is null)
            {
                return hc.ToHashCode();
            }

            hc.Add(obj.Name, StringComparer.Ordinal);
            hc.Add(obj.ValueCount);
            hc.Add(obj.ValueType);
            hc.Add(obj.ValueUnit, StringComparer.Ordinal);
            hc.Add(obj.IsTimeSliceArray);
            hc.Add(obj.IsDeprecated);
            hc.Add(obj.DeprecatedBy, StringComparer.Ordinal);
            hc.Add(obj.Description, StringComparer.Ordinal);

            return hc.ToHashCode();
        }
    }
}
