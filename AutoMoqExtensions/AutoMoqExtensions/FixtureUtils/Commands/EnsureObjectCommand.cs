using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class EnsureObjectCommand : ISpecimenCommand
    {
        public void Execute(object specimen, ISpecimenContext context)
        {            
            var mock = AutoMockHelpers.GetFromObj(specimen);
            if (mock is not null)
            {
                try
                {
                    mock.EnsureMocked();
                }
                catch
                {
                    if (!mock.CallBase && mock.GetInnerType().IsDelegate()) throw; // Delagates aren't allowed callbase anyway

                    // Sometimes the mocked method doesn't work, either it hasn't been setup because of an error, or it's private/protected, or for other issues
                    // Similarly if Callbase is true then we might run into an error from the base ctor
                    mock.CallBase = !mock.CallBase;
                    mock.EnsureMocked();
                    mock.CallBase = !mock.CallBase;
                }
            }
        }
    }
}
