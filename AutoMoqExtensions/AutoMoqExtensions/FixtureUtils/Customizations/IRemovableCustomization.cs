using AutoFixture;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Customizations
{
    public interface IRemovableCustomization : ICustomization
    {
        void RemoveCustomization(IFixture fixture);
    }
}
