using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Specifications
{
    internal class TypeMatchSpecification : IRequestSpecification
    {
        public TypeMatchSpecification(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }

        public bool IsSatisfiedBy(object request)
        {
            if (request is null) return false;

            var t = (request as Type) ?? request.GetType();            

            return t == TargetType;
        }
    }
}
