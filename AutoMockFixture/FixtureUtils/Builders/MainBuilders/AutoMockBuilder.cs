using AutoMockFixture.AutoMockUtils;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Specifications;

namespace AutoMockFixture.FixtureUtils.Builders.MainBuilders;

internal class AutoMockBuilder : ISpecimenBuilder
{
    private AutoMockDirectRequestSpecification requestSpecification => new(AutoMockHelpers);

    public AutoMockBuilder(ISpecimenBuilder builder, IAutoMockHelpers autoMockHelpers)
    {
        this.Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public ISpecimenBuilder Builder { get; }
    public IAutoMockHelpers AutoMockHelpers { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request) || request is not AutoMockDirectRequest mockRequest)
            return new NoSpecimen();

        var specimen = this.Builder.Create(request, context);
        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            return specimen;

        if (specimen is not IAutoMock autoMock || specimen.GetType() != mockRequest.Request)
        {
            mockRequest.SetCompleted(this);
            return new NoSpecimen();
        }

        autoMock.Tracker = mockRequest;

        mockRequest.SetResult(specimen, this);

        return specimen;
    }
}
