using AutoMockFixture.FixtureUtils;
using NUnit.Framework.Internal.Commands;
using System;

namespace AutoMockFixture.NUnit3.Moq4;

internal class DisposeFixtureCommand : AfterTestCommand
{
    public DisposeFixtureCommand(TestCommand innerCommand, AutoMockFixtureBase fixture) : base(innerCommand)
    {
        AfterTest = context =>
        {
            try { fixture?.Dispose(); } catch { }
        };
    }
}
