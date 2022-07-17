using AutoMoqExtensions.AutoMockUtils;

namespace AutoMoqExtensions.FixtureUtils.MethodQueries;

internal class CustomModestConstructorQuery : IMethodQuery
{
    public IEnumerable<IMethod> SelectMethods(Type type)
    {
        return from ci in type.GetPublicAndProtectedConstructors()
               let paramInfos = ci.GetParameters()
               orderby paramInfos.Length ascending
               select new CustomConstructorMethod(ci) as IMethod;
    }
}
