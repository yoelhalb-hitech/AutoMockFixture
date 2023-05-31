
namespace AutoMockFixture.Moq4.VerifyInfo;

public class VerifyFuncInfo<T, TResult> : IVerifyInfo<T> where T : class
{
    public Expression<Func<T, TResult>> Expression { get; }
    public Times Times { get; }

    public VerifyFuncInfo(Expression<Func<T, TResult>> expression, Times times)
    {
        this.Expression = expression;
        this.Times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.Verify(Expression, Times);
    }
}
