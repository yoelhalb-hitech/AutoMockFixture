
namespace AutoMockFixture;

internal interface ISetCallBase
{
    /// <summary>
    /// While in general we don't support setting callbase after the inner obejct is already created, there are exceptions and in this case we can force it with this
    /// </summary>
    /// <param name="value"></param>
    void ForceSetCallbase(bool value);
}