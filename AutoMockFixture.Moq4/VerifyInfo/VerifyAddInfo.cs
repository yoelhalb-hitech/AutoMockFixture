
namespace AutoMockFixture.Moq4.VerifyInfo;

public class VerifyAddInfo<T> : IVerifyInfo<T> where T : class
{
    private readonly Action<T> expression;
    private readonly Times times;

    public VerifyAddInfo(Action<T> expression, Times times)
    {
        this.expression = expression;
        this.times = times;
    }

    public void Verify(Mock<T> obj)
    {
        obj.VerifyAdd(expression, times);
    }
}
