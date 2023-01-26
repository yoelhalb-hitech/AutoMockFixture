using AutoFixture.Kernel;
using Moq;
using System.Reflection;

namespace AutoMockFixture.Moq.FixtureUtils.Specifications;

internal class MockRequestSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
        => request is Type t && typeof(Mock).IsAssignableFrom(t) && t.IsGenericType;
}
