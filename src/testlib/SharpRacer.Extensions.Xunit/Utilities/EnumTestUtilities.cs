using System.Numerics;
using System.Runtime.CompilerServices;

namespace SharpRacer.Extensions.Xunit.Utilities;

public static class EnumTestUtilities
{
    public static TEnumIntegral FlagsMaxIntegralValue<TEnum, TEnumIntegral>()
        where TEnum : unmanaged, Enum
        where TEnumIntegral : IBinaryNumber<TEnumIntegral>
    {
        VerifyEnumIntegralType<TEnum, TEnumIntegral>();

        if (!IsFlagsEnum<TEnum>())
        {
            throw new ArgumentException($"Type argument '{typeof(TEnum)}' is not decorated with FlagsAttribute.");
        }

        var definedValues = Enum.GetValuesAsUnderlyingType<TEnum>().Cast<TEnumIntegral>().ToArray();

        // Calculate the maximum possible integral value where all flags are set
        var max = TEnumIntegral.Zero;

        for (int i = 0; i < definedValues.Length; i++)
        {
            max |= definedValues[i];
        }

        return max;
    }

    /// <summary>
    /// Gets all possible combinations of a flags enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enum.</typeparam>
    /// <typeparam name="TEnumIntegral">The underlying type of <typeparamref name="TEnum"/>.</typeparam>
    /// <param name="excludeZeroValue">
    /// If <see langword="true"/>, the <typeparamref name="TEnum"/> value equal to zero is excluded from the returned array.
    /// </param>
    /// <param name="excludeDefinedFlagValues">
    /// If <see langword="true"/>, the returned array contains only valid flag combinations that are not explicitly defined by the
    /// enumeration.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static TEnum[] GetFlagCombinations<TEnum, TEnumIntegral>(bool excludeZeroValue = true, bool excludeDefinedFlagValues = false)
        where TEnum : unmanaged, Enum
        where TEnumIntegral : unmanaged, IBinaryNumber<TEnumIntegral>
    {
        if (!IsFlagsEnum<TEnum>())
        {
            throw new ArgumentException($"Type argument '{typeof(TEnum)}' is not decorated with FlagsAttribute.");
        }

        VerifyEnumIntegralType<TEnum, TEnumIntegral>();

        var definedValues = GetUnderlyingValues<TEnum, TEnumIntegral>();
        var maxIntegralValue = FlagsMaxIntegralValue<TEnum, TEnumIntegral>();

        var result = new List<TEnum>();

        // Loop over all of the possible integral values of the enum from TUnderlyingType.Zero to the highest possible value representing
        // an enum value where all defined flags are set.
        for (TEnumIntegral i = TEnumIntegral.Zero; i <= maxIntegralValue; i++)
        {
            // unaccountedBits may not represent a valid combination of enum values at this point, e.g. the enum does not define a value
            // for every possible bit between zero and max. We will eliminate all of the bits from this value that are not set by any of
            // the defined values collected earlier.
            var unaccountedBits = i;

            for (int j = 0; j < definedValues.Length; j++)
            {
                // Remove all of the bits set by each defined enum value from unaccountedBits
                unaccountedBits &= ~(definedValues[j]);

                // If unaccountedBits is zero, then there were no bits set that are not set by any of the defined enum values, so we can
                // reinterpret the value originally assigned to unaccountedBits (i) as an enum value, knowing that all of its bits
                // correspond to a bit set by a defined enumeration value.
                if (unaccountedBits == TEnumIntegral.Zero)
                {
                    var enumVal = Unsafe.As<TEnumIntegral, TEnum>(ref i);

                    if (!excludeDefinedFlagValues || !definedValues.Contains(i))
                    {
                        result.Add(enumVal);
                    }
                }
            }
        }

        // Remove the enum value equal to zero, if defined
        if (excludeZeroValue && TryGetZeroValue<TEnum, TEnumIntegral>(out var zeroValue))
        {
            result.Remove(zeroValue);
        }

        return result.ToArray();
    }

    public static TEnumIntegral[] GetUnderlyingValues<TEnum, TEnumIntegral>()
        where TEnum : unmanaged, Enum
        where TEnumIntegral : INumberBase<TEnumIntegral>
    {
        VerifyEnumIntegralType<TEnum, TEnumIntegral>();

        return Enum.GetValuesAsUnderlyingType<TEnum>().Cast<TEnumIntegral>().ToArray();
    }

    public static TEnum GetZeroValue<TEnum, TEnumIntegral>()
        where TEnum : unmanaged, Enum
        where TEnumIntegral : INumberBase<TEnumIntegral>
    {
        return (TEnum)(object)TEnumIntegral.Zero;
    }

    public static bool IsFlagsEnum<TEnum>()
        where TEnum : Enum
    {
        return typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0;
    }

    public static bool IsZeroDefined<TEnum, TEnumIntegral>()
        where TEnum : Enum
        where TEnumIntegral : INumberBase<TEnumIntegral>
    {
        VerifyEnumIntegralType<TEnum, TEnumIntegral>();

        return Enum.IsDefined(typeof(TEnum), TEnumIntegral.Zero);
    }

    public static TEnumIntegral MaxIntegralValue<TEnum, TEnumIntegral>()
        where TEnum : unmanaged, Enum
        where TEnumIntegral : INumber<TEnumIntegral>
    {
        VerifyEnumIntegralType<TEnum, TEnumIntegral>();

        if (IsFlagsEnum<TEnum>())
        {
            throw new ArgumentException($"Type argument '{typeof(TEnum)}' cannot be decorated with FlagsAttribute.");
        }

        var definedValues = GetUnderlyingValues<TEnum, TEnumIntegral>();

        TEnumIntegral max = TEnumIntegral.Zero;

        for (int i = 0; i < definedValues.Length; i++)
        {
            max = TEnumIntegral.Max(max, definedValues[i]);
        }

        return max;
    }

    public static bool TryGetZeroValue<TEnum, TEnumIntegral>(out TEnum enumZero)
        where TEnum : unmanaged, Enum
        where TEnumIntegral : INumberBase<TEnumIntegral>
    {
        if (!IsZeroDefined<TEnum, TEnumIntegral>())
        {
            enumZero = default;
            return false;
        }

        enumZero = GetZeroValue<TEnum, TEnumIntegral>();
        return true;
    }

    private static void VerifyEnumIntegralType<TEnum, TEnumIntegral>()
        where TEnum : Enum
        where TEnumIntegral : INumberBase<TEnumIntegral>
    {
        var integralType = Enum.GetUnderlyingType(typeof(TEnum));

        if (typeof(TEnumIntegral) != integralType)
        {
            throw new ArgumentException(
                $"Enumeration '{typeof(TEnum)}' has underlying type '{Enum.GetUnderlyingType(typeof(TEnum))}'. Expected: {typeof(TEnumIntegral)}");
        }
    }
}
