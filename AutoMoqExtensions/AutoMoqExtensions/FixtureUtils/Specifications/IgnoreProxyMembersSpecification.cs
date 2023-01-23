using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Specifications;

internal class IgnoreProxyMembersSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        switch (request)
        {
            case FieldInfo fi:
                return !IsProxyMember(fi);

            case PropertyInfo _:
                return true;

            default:
                return false;
        }
    }

    private static bool IsProxyMember(FieldInfo fi)
    {
        return string.Equals(fi.Name, "__interceptors", StringComparison.Ordinal) ||
                string.Equals(fi.Name, "__target", StringComparison.Ordinal);
    }
}
