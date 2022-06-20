using AutoFixture.Kernel;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class CacheCommand : ISpecimenCommand
    {
        public CacheCommand(Cache cache)
        {
            Cache = cache;
        }

        public Cache Cache { get; }

        public void Execute(object specimen, ISpecimenContext context)
        {
            if (specimen is null) return; // Have no way of knowing what type is the request, plus for null it doesn't make a difference caching...

            Cache.AddIfNeeded(specimen.GetType(), specimen);
        }
    }
}
