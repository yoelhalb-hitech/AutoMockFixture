using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils.Specifications
{    
    internal class AttributeMatchSpecification : IRequestSpecification
    {
        public AttributeMatchSpecification(Type type)
        {
            Type = type;
        }

        public Type Type { get; }

        public bool IsSatisfiedBy(object request)
        {
            if (request is not Type t) return false;

            return t.IsDefined(Type, false);
        }
    }
}
