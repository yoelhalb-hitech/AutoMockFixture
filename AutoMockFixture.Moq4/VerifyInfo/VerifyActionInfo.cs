using Moq;
using System.Linq.Expressions;

namespace AutoMockFixture.Moq4.VerifyInfo;

public class VerifyActionInfo<T> : IVerifyInfo<T> where T : class
{
    private readonly Expression<Action<T>> expression;
    private readonly Times times;

    public VerifyActionInfo(Expression<Action<T>> expression, Times times)
    {
        this.expression = expression;
        this.times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.Verify(expression, times);
    }
}
