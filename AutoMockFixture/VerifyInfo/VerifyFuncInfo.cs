using Moq;
using System.Linq.Expressions;

namespace AutoMoqExtensions.VerifyInfo;

public class VerifyFuncInfo<T, TResult> : IVerifyInfo<T> where T : class
{
    private readonly Expression<Func<T, TResult>> expression;
    private readonly Times times;

    public VerifyFuncInfo(Expression<Func<T, TResult>> expression, Times times)
    {
        this.expression = expression;
        this.times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.Verify(expression, times);
    }
}
