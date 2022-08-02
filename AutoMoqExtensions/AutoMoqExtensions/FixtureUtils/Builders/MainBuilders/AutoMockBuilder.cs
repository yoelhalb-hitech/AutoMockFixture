using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;

namespace AutoMoqExtensions.FixtureUtils.Builders.MainBuilders;

internal class AutoMockBuilder : ISpecimenBuilder
{
    private static AutoMockDirectRequestSpecification requestSpecification = new();

    public AutoMockBuilder(ISpecimenBuilder builder)
    {
        this.Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public ISpecimenBuilder Builder { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request) || request is not AutoMockDirectRequest mockRequest)
            return new NoSpecimen();

        var specimen = this.Builder.Create(request, context);
        if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            return specimen;

        if (specimen is not IAutoMock autoMock || specimen.GetType() != mockRequest.Request)
        {
            mockRequest.SetCompleted();
            return new NoSpecimen();
        }

        autoMock.DefaultValue = DefaultValue.Mock;
        autoMock.Tracker = mockRequest;

        mockRequest.SetResult(specimen);

        return specimen;
    }
}
