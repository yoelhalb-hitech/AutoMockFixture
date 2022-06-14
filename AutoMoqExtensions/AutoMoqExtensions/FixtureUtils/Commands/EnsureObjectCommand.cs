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
                    // While it anyway calls the base ctor, if the ctor calls another method we want that mocked
                    mock.CallBase = false;
                    mock.EnsureMocked();
                }
                catch
                {
                    // Sometimes the mocked method doesn't work, either it hasn't been setup because of an error, or it's private/protected, or for other issues
                    mock.CallBase = !mock.GetInnerType().IsDelegate(); // Delagate aren't allowed callbase anyway
                    mock.EnsureMocked();
                }
            }
        }
    }
}
