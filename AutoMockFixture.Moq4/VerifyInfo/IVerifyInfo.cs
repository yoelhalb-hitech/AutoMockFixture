
namespace AutoMockFixture.Moq4.VerifyInfo;

public interface IVerifyInfo<T> where T : class
{
    void Verify(Mock<T> obj);
}
