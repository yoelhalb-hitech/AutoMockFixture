﻿using AutoFixture;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.HelperRequests.AutoMock;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System.Reflection;

namespace AutoMoqExtensions.FixtureUtils.Commands;

internal class AutoMockDependenciesAutoPropertiesHandlerCommand : ISpecimenCommand
{
    public AutoMockDependenciesAutoPropertiesHandlerCommand(AutoMockFixture fixture)
    {
        Fixture = fixture;
    }

    public AutoMockFixture Fixture { get; }

    public virtual void Execute(object specimen, ISpecimenContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        Fixture.ProcessingTrackerDict.TryGetValue(specimen, out var existingTracker);
        
        var command = new AutoMockAutoPropertiesCommand(Fixture) 
        {
            // Private setters is always the job of the object code, and mocked non callbase doesn't need it
            IncludePrivateSetters = false,
        };
        
        command.Execute(specimen, context);
    }
}