
namespace AutoMockFixture.FixtureUtils.MethodQueries;

internal partial class AutoMockConstructorQuery : IMethodQuery
{
    private static readonly DelegateSpecification DelegateSpecification = new DelegateSpecification();

    public AutoMockConstructorQuery(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public IEnumerable<IMethod> SelectMethods(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!AutoMockHelpers.IsAutoMock(type)) throw new ArgumentOutOfRangeException(nameof(type));

        var mockType = AutoMockHelpers.GetMockedType(type);
        if(mockType is null) throw new ArgumentOutOfRangeException(nameof(type));

        if (mockType.GetTypeInfo().IsInterface || DelegateSpecification.IsSatisfiedBy(mockType))
            return Enumerable.Empty<IMethod>();

        return from ci in mockType.GetPublicAndProtectedConstructors()
               let paramInfos = ci.GetParameters()
               orderby paramInfos.Length ascending
               select new CustomConstructorMethod(ci, AutoMockHelpers) as IMethod;
    }
}
