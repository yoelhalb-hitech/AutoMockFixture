
namespace AutoMockFixture.Moq4.VerifyInfo;

public class VerifyActionInfo<T> : IVerifyInfo<T> where T : class
{
    public Expression<Action<T>> Expression { get; }
    public Times Times { get; }

    public VerifyActionInfo(Expression<Action<T>> expression, Times times)
    {
        this.Expression = expression;
        this.Times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.Verify(Expression, Times);
    }
}
