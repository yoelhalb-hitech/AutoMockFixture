
namespace AutoMockFixture.FixtureUtils.Specifications;

internal class TypeSpecification : IRequestSpecification
{
    public TypeSpecification(Type targetType, IAutoMockHelpers autoMockHelpers)
    {
        Logger.LogInfo(targetType.Name);
        if (targetType is null) throw new ArgumentNullException(nameof(targetType));

        TargetType = targetType;
        AutoMockHelpers = autoMockHelpers;
    }

    public Type TargetType { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public bool IsSatisfiedBy(object request) => request is Type t
                    && (TargetType.IsAssignableFrom(t)
                            || (AutoMockHelpers.IsAutoMock(t) && AutoMockHelpers.IsAutoMock(TargetType)
                                    && AutoMockHelpers.GetMockedType(TargetType)!.IsAssignableFrom(AutoMockHelpers.GetMockedType(t))));
}
