using AutoMockFixture.AutoMockUtils;

namespace AutoMockFixture.FixtureUtils.MethodQueries;

internal class CustomModestConstructorQuery : IMethodQuery
{
    public CustomModestConstructorQuery(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public IEnumerable<IMethod> SelectMethods(Type type)
    {
        return from ci in type.GetPublicAndProtectedConstructors()
               let paramInfos = ci.GetParameters()
               orderby paramInfos.Length ascending
               select new CustomConstructorMethod(ci, AutoMockHelpers) as IMethod;
    }
}
