using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils.Specifications
{
    internal class AutoMockableSpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request)
        {
            var t = request as Type;
            if (t is null) return false;

            return AutoMockHelpers.IsAutoMockAllowed(t);
        }
    }
}
