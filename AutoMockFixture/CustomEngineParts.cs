using AutoFixture;
using AutoMockFixture.FixtureUtils.MethodInvokers;
using AutoMockFixture.FixtureUtils.MethodQueries;

namespace AutoMockFixture;

internal class CustomEngineParts : DefaultEngineParts
{
    public CustomEngineParts(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public override IEnumerator<ISpecimenBuilder> GetEnumerator()
    {
        var be = base.GetEnumerator();
        while (be.MoveNext())
        {
            if (be.Current is MethodInvoker)
            {
                yield return new MethodInvokerWithRecursion(new CustomModestConstructorQuery(AutoMockHelpers), AutoMockHelpers);
            }
            else yield return be.Current;
        }
    }
}
