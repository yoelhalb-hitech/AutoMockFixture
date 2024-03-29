﻿using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.FixtureUtils.Requests.MainRequests;
using AutoMockFixture.FixtureUtils.Requests.SpecialRequests;

namespace AutoMockFixture.FixtureUtils.Builders.SpecialBuilders;

internal class InnerBuilder : ISpecimenBuilder
{
    public InnerBuilder(IAutoMockHelpers autoMockHelpers)
    {
        AutoMockHelpers = autoMockHelpers ?? throw new ArgumentNullException(nameof(autoMockHelpers));
    }

    public IAutoMockHelpers AutoMockHelpers { get; }

    public object? Create(object request, ISpecimenContext context)
    {
        if (request is not InnerRequest innerRequest) return new NoSpecimen();

        var type = innerRequest.Request;

        object newRequest = innerRequest.OuterRequest switch
        {
            { } when AutoMockHelpers.IsAutoMock(type) => new AutoMockDirectRequest(type, innerRequest),
            AutoMockDependenciesRequest => new AutoMockDependenciesRequest(type, innerRequest),
            AutoMockRequest => new AutoMockRequest(type, innerRequest),
            NonAutoMockRequest => new NonAutoMockRequest(type, innerRequest),
            _ => type,
        };

        var specimen = context.Resolve(newRequest);

        (newRequest as ITracker)?.SetResult(specimen, this);
        if (newRequest is ITracker tracker)
        {
            tracker.SetResult(specimen, this);
            innerRequest.SetCompleted(this);
        }
        else innerRequest.SetResult(specimen, this);

        if (specimen is NoSpecimen || specimen is OmitSpecimen)
        {
            return new NoSpecimen(); // Let the system handle it
        }

        return specimen;
    }
}
