using AutoFixture.AutoMoq;
using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class AutoMockStubAllPropertiesCommand : ISpecimenCommand
    {
        public void Execute(object specimen, ISpecimenContext context)
        {
            var mock = AutoMockHelpers.GetFromObj(specimen);
            if (mock is null) return;

            if (mock.CallBase) return; // We don't want to setup as it will destroy any existing values from the ctor

            new StubPropertiesCommand().Execute(mock, context);
        }
    }
}
