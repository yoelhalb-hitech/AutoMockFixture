using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;

namespace AutoMockFixture.FixtureUtils.Specifications;

/// <summary>
/// For use to match a type or <see cref="AutoMockRequest"/> for the type or <see cref="AutoMockDirectRequest"/> for the type or <see cref="AutoMockDependenciesRequest"/> for the type
/// </summary>
internal class TypeOrRequestSpecification : IRequestSpecification
{
    public TypeOrRequestSpecification(IRequestSpecification specification, IAutoMockHelpers autoMockHelpers)
    {
        Specification = specification;
        AutoMockHelpers = autoMockHelpers;
    }

    public IRequestSpecification Specification { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public bool IsSatisfiedBy(object request)
    {
        return (request is Type t && Specification.IsSatisfiedBy(t))
                || (request is AutoMockRequest r && Specification.IsSatisfiedBy(r.Request))
                || (request is NonAutoMockRequest n && Specification.IsSatisfiedBy(n.Request))
                || (request is AutoMockDirectRequest dr
                            && (Specification.IsSatisfiedBy(dr.Request)
                                      || (AutoMockHelpers.IsAutoMock(dr.Request)
                                                 && Specification.IsSatisfiedBy(AutoMockHelpers.GetMockedType(dr.Request)))))
                || (request is AutoMockDependenciesRequest der && Specification.IsSatisfiedBy(der.Request));
    }
}
