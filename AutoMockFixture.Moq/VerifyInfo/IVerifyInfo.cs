using Moq;

namespace AutoMockFixture.VerifyInfo;

public interface IVerifyInfo<T> where T : class
{
    void Verify(Mock<T> obj);
}
