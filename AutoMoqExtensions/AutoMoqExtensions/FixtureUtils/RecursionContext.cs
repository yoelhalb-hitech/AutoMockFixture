using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils
{
    internal class RecursionContext : SpecimenContext
    {
        public RecursionContext(ISpecimenBuilder builder) : base(builder)
        {
        }

        internal Dictionary<Type, object> BuilderCache { get; } = new Dictionary<Type, object>();
    }
}
