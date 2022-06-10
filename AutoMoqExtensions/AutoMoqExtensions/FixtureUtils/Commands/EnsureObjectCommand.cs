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
                    mock.CallBase = !mock.GetInnerType().IsDelegate();
                    mock.EnsureMocked();
                }
                catch
                {
                    mock.CallBase = false;
                    mock.EnsureMocked();
                }
            }
        }
    }
}
