namespace SharpRacer.Extensions.Xunit;

public interface IOperatorCallbacks<TLeft, TRight>
{
    void OnEqualityOperator(TLeft left, TRight right);
    void OnInequalityOperator(TLeft left, TRight right);

    void OnGreaterThanOperator(TLeft left, TRight right);
    void OnGreaterThanOrEqualOperator(TLeft left, TRight right);
    void OnLessThanOperator(TLeft left, TRight right);
    void OnLessThanOrEqualOperator(TLeft left, TRight right);
}

public interface IOperatorCallbacks<T> : IOperatorCallbacks<T, T>
{

}
