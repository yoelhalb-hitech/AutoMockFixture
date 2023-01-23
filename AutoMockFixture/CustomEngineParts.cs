using AutoFixture;
using AutoMoqExtensions.FixtureUtils.MethodInvokers;
using AutoMoqExtensions.FixtureUtils.MethodQueries;

namespace AutoMoqExtensions;

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
