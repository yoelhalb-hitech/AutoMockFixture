using Moq;

namespace AutoMockFixture.Moq.VerifyInfo;

public class VerifyRemoveInfo<T> : IVerifyInfo<T> where T : class
{
    private readonly Action<T> expression;
    private readonly Times times;

    public VerifyRemoveInfo(Action<T> expression, Times times)
    {
        this.expression = expression;
        this.times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.VerifyRemove(expression, times);
    }
}
