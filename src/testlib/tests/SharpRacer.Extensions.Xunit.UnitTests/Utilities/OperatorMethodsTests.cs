using SharpRacer.Extensions.Xunit.TestObjects;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit.Utilities;

public class OperatorMethodsTests
{
    [Fact]
    public void ComparisonOperators_GetOrThrow_Test()
    {
        Assert.NotNull(OperatorMethods.GreaterThan.GetOrThrow<ComparableStruct>());
        Assert.NotNull(OperatorMethods.GreaterThanOrEqual.GetOrThrow<ComparableStruct>());
        Assert.NotNull(OperatorMethods.LessThan.GetOrThrow<ComparableStruct>());
        Assert.NotNull(OperatorMethods.LessThanOrEqual.GetOrThrow<ComparableStruct>());
    }

    [Fact]
    public void ComparisonOperators_GetOrThrow_ThrowIfNotDefinedTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.GreaterThan.GetOrThrow<PlainStruct>);
        Assert.Throws<XunitException>(OperatorMethods.GreaterThanOrEqual.GetOrThrow<PlainStruct>);
        Assert.Throws<XunitException>(OperatorMethods.LessThan.GetOrThrow<PlainStruct>);
        Assert.Throws<XunitException>(OperatorMethods.LessThanOrEqual.GetOrThrow<PlainStruct>);
    }

    [Fact]
    public void ComparisonOperators_GetOrThrow_ThrowIfNotBoolReturnTypeTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.GreaterThan.GetOrThrow<NonBooleanComparableStruct>);
        Assert.Throws<XunitException>(OperatorMethods.GreaterThanOrEqual.GetOrThrow<NonBooleanComparableStruct>);
        Assert.Throws<XunitException>(OperatorMethods.LessThan.GetOrThrow<NonBooleanComparableStruct>);
        Assert.Throws<XunitException>(OperatorMethods.LessThanOrEqual.GetOrThrow<NonBooleanComparableStruct>);
    }

    [Fact]
    public void EqualityOperators_GetOrThrow_Test()
    {
        Assert.NotNull(OperatorMethods.Equality.GetOrThrow<EqualityStruct>());
    }

    [Fact]
    public void EqualityOperators_GetOrThrow_ThrowIfNotDefinedTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.Equality.GetOrThrow<PlainStruct>);
        Assert.Throws<XunitException>(OperatorMethods.Inequality.GetOrThrow<PlainStruct>);
    }

    [Fact]
    public void EqualityOperators_GetOrThrow_ThrowIfNotBoolReturnTypeTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.Equality.GetOrThrow<NonBooleanEqualityStruct>);
        Assert.Throws<XunitException>(OperatorMethods.Inequality.GetOrThrow<NonBooleanEqualityStruct>);
    }
}
