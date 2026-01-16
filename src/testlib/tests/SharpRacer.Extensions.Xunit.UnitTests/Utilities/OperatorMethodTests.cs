using Moq;
using SharpRacer.Extensions.Xunit.TestObjects;
using Xunit.Sdk;

namespace SharpRacer.Extensions.Xunit.Utilities;

public class OperatorMethodTests
{
    [Fact]
    public void Ctor_Test()
    {
        const string methodName = "op_Equality";
        const string operatorExpr = "==";

        var opMethod = new OperatorMethod(methodName, operatorExpr);

        Assert.Equal(methodName, opMethod.MethodName);
        Assert.Equal(operatorExpr, opMethod.OperatorExpression);
    }

    [Fact]
    public void Assert_DoesNotThrowIfResultEqualsExpectedTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var observerMock = mocks.Create<IOperatorCallbacks<ObservableOperatorStruct>>();

        var struct1 = new ObservableOperatorStruct(1, observerMock.Object);
        var struct2 = new ObservableOperatorStruct(1, observerMock.Object);

        observerMock.Setup(x => x.OnEqualityOperator(It.IsAny<ObservableOperatorStruct>(), It.IsAny<ObservableOperatorStruct>()))
            .Callback<ObservableOperatorStruct, ObservableOperatorStruct>((left, right) =>
            {
                Assert.Equal(struct1, left);
                Assert.Equal(struct2, right);
            })
            .Verifiable(Times.Once());

        OperatorMethods.Equality.Assert(true, struct1, struct2, nameof(struct1), nameof(struct2));
    }

    [Fact]
    public void Assert_ThrowIfResultNotEqualToExpectedTest()
    {
        const bool expected = true;
        const bool actual = false;

        var mocks = new MockRepository(MockBehavior.Strict);

        var observerMock = mocks.Create<IOperatorCallbacks<ObservableOperatorStruct>>();

        var struct1 = new ObservableOperatorStruct(1, observerMock.Object);
        var struct2 = new ObservableOperatorStruct(2, observerMock.Object);

        observerMock.Setup(x => x.OnEqualityOperator(It.IsAny<ObservableOperatorStruct>(), It.IsAny<ObservableOperatorStruct>()))
            .Callback<ObservableOperatorStruct, ObservableOperatorStruct>((left, right) =>
            {
                Assert.Equal(struct1, left);
                Assert.Equal(struct2, right);
            })
            .Verifiable(Times.Once());

        var xex = Assert.Throws<XunitException>(
            () => OperatorMethods.Equality.Assert(true, struct1, struct2, nameof(struct1), nameof(struct2)));

        Assert.Equal($"Expression '{nameof(struct1)} == {nameof(struct2)}' returned {actual}. Expected: {expected}", xex.Message);
    }

    [Fact]
    public void GetOrThrow_Test()
    {
        Assert.NotNull(OperatorMethods.Equality.GetOrThrow<EqualityStruct>());
    }

    [Fact]
    public void GetOrThrow_ThrowIfOperatorNotDefinedTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.Equality.GetOrThrow<PlainStruct>);
    }

    [Fact]
    public void GetOrThrow_ThrowIfOperatorReturnTypeIsNotBooleanTest()
    {
        Assert.Throws<XunitException>(OperatorMethods.Equality.GetOrThrow<NonBooleanEqualityStruct>);
    }

    [Fact]
    public void Invoke_EqualityTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var observerMock = mocks.Create<IOperatorCallbacks<ObservableOperatorStruct>>();

        var struct1 = new ObservableOperatorStruct(1, observerMock.Object);
        var struct2 = new ObservableOperatorStruct(1, observerMock.Object);

        observerMock.Setup(x => x.OnEqualityOperator(It.IsAny<ObservableOperatorStruct>(), It.IsAny<ObservableOperatorStruct>()))
            .Callback<ObservableOperatorStruct, ObservableOperatorStruct>((left, right) =>
            {
                Assert.Equal(struct1, left);
                Assert.Equal(struct2, right);
            })
            .Verifiable(Times.Once());

        Assert.True(OperatorMethods.Equality.Invoke(struct1, struct2));

        mocks.Verify();
    }

    [Fact]
    public void Invoke_InequalityTest()
    {
        var mocks = new MockRepository(MockBehavior.Strict);

        var observerMock = mocks.Create<IOperatorCallbacks<ObservableOperatorStruct>>();

        var struct1 = new ObservableOperatorStruct(1, observerMock.Object);
        var struct2 = new ObservableOperatorStruct(2, observerMock.Object);

        observerMock.Setup(x => x.OnInequalityOperator(It.IsAny<ObservableOperatorStruct>(), It.IsAny<ObservableOperatorStruct>()))
            .Callback<ObservableOperatorStruct, ObservableOperatorStruct>((left, right) =>
            {
                Assert.Equal(struct1, left);
                Assert.Equal(struct2, right);
            })
            .Verifiable(Times.Once());

        Assert.True(OperatorMethods.Inequality.Invoke(struct1, struct2));

        mocks.Verify();
    }
}
