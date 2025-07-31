
namespace AutoMockFixture.Moq4;

public static class Is
{
    public static T DefaultValue<T>() => Moq.It.Is<T>(x => object.Equals(x, default(T)));
    public static bool False() => Moq.It.Is<bool>(x => x.Equals(false));
    public static bool True() => Moq.It.Is<bool>(x => x.Equals(true));
}
