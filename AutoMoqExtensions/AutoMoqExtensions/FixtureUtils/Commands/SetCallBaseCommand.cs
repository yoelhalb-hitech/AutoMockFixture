using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class SetCallBaseCommand : ISpecimenCommand
    {
        public void Execute(object specimen, ISpecimenContext context)
        {
            try
            {
                var mock = AutoMockHelpers.GetFromObj(specimen);
                if (mock is not null)
                {
                    mock.CallBase = false;
                }

            }
            catch { }
        }
    }
}
