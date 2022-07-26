using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Specifications
{
    internal class InnerTypeSpecification : IRequestSpecification
    {
        public bool IsSatisfiedBy(object request)
        {
            if (request is not Type t) return false;

            return t.HasInnerType();
        }
    }
}
