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
                catch(Exception ex)
                {
                    throw new Exception(@"Unable to create the object.
It's possible that changing `CallBase` setting will make a difference.
Otherwise please read carefully the inner messages.
Note that the inner messages are not neccesarily the actual error, for example CastleDynamicProxy can report that `it could not find a parameterless constructor` while actually having found more than one...", ex);
                }
            }
        }
    }
}
