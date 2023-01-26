using Moq;

namespace AutoMockFixture.Moq.VerifyInfo;

public class VerifySetInfo<T> : IVerifyInfo<T> where T : class
{
    private readonly Action<T> expression;
    private readonly Times times;

    public VerifySetInfo(Action<T> expression, Times times)
    {
        this.expression = expression;
        this.times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.VerifySet(expression, times);
    }
}
