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
            Logger.LogInfo(targetType.Name);
            if(targetType is null) throw new ArgumentNullException(nameof(targetType));

            TargetType = targetType;
        }

        public Type TargetType { get; }

        public bool IsSatisfiedBy(object request) => TargetType.IsInstanceOfType(request);
    }
}
