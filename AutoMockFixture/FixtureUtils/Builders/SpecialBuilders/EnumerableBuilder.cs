using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class EnumerableBuilder : NonConformingBuilder
{
    public override Type[] SupportedTypes => new Type[]
    {
        typeof(Array),
        typeof(IEnumerable<>),
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        typeof(IAsyncEnumerable<>)
#endif
    };

    public override Type[] NotSupportedTypes => new[] { typeof(string) };

    public EnumerableBuilder(IAutoMockHelpers autoMockHelpers, int repeat)
    {
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
        this.Repeat = repeat;
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public int Repeat { get; }

    public override bool NoGenerateInner => true;

    public override object? CreateResult(Type requestType, object[] innerResults, IRequestWithType typeRequest, ISpecimenContext context)
    {
        var ctors = requestType.GetConstructors(BindingFlagsExtensions.AllBindings).Where(c => c.IsConstructor && !c.IsStatic);

        if(requestType.IsAbstract || requestType.IsInterface
            || (AutoMockHelpers.IsAutoMock(requestType) && AutoMockHelpers.IsAutoMockAllowed(AutoMockHelpers.GetMockedType(requestType)!))
            || (typeRequest is AutoMockRequest && AutoMockHelpers.IsAutoMockAllowed(requestType)))
        {
            var mockType = AutoMockHelpers.IsAutoMock(requestType) ? requestType : AutoMockHelpers.GetAutoMockType(requestType);
            var directRequest = new AutoMockDirectRequest(mockType, typeRequest)
            {
                MockShouldCallBase = (typeRequest as IFixtureTracker)?.MockShouldCallBase, // Forward any explicit `MockShouldCallBase` on the request
            };

            var specimen = context.Resolve(directRequest);
            if (specimen is null || specimen is NoSpecimen || specimen is OmitSpecimen) return new NoSpecimen();

            return specimen is IAutoMock mock ? mock.GetMocked() : specimen;
        }

        var hasCountCtor = ctors.Any(x => x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == typeof(int));
        if(hasCountCtor) return Activator.CreateInstance(requestType, Repeat);

        if(requestType.IsArray && requestType.GetArrayRank() > 1)
        {
            var countCtor = ctors.FirstOrDefault(x => x.GetParameters().Length == requestType.GetArrayRank() && x.GetParameters().All(p => p.ParameterType == typeof(int)));
            if (countCtor is not null) return countCtor.Invoke(Enumerable.Repeat(Repeat as object, requestType.GetArrayRank()).ToArray());
        }

        var hasDefaultCtor = ctors.Any(x => !x.GetParameters().Any());
        if(hasDefaultCtor) return Activator.CreateInstance(requestType);

        return new NoSpecimen();
    }

    protected override InnerRequest GetInnerRequest(Type type, IRequestWithType originalRequest, int argIndex)
    {
        throw new NotImplementedException("Should not arrive here");
    }
}
