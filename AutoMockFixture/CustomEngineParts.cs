using AutoFixture;
using AutoMockFixture.FixtureUtils.MethodInvokers;
using AutoMockFixture.FixtureUtils.MethodQueries;

namespace AutoMockFixture;

internal class CustomEngineParts : DefaultEngineParts
{
    public CustomEngineParts(AutoMockFixture fixture)
    {
        Fixture = fixture;
    }

    public AutoMockFixture Fixture { get; }

    public override IEnumerator<ISpecimenBuilder> GetEnumerator()
    {
        var be = base.GetEnumerator();
        while (be.MoveNext())
        {
            if (be.Current is MethodInvoker)
            {
                yield return new MethodInvokerWithRecursion(
                                    new CustomModestConstructorQuery());
            }
            else yield return be.Current;
        }
    }
}
