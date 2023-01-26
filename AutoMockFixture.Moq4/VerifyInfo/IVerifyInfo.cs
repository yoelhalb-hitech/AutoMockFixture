using Moq;

namespace AutoMockFixture.Moq.VerifyInfo;

public interface IVerifyInfo<T> where T : class
{
    void Verify(Mock<T> obj);
}
