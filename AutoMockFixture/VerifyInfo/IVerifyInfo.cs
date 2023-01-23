using Moq;

namespace AutoMoqExtensions.VerifyInfo;

public interface IVerifyInfo<T> where T : class
{
    void Verify(Mock<T> obj);
}
