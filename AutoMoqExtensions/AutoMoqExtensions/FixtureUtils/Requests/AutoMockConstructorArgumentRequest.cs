using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockConstructorArgumentRequest 
        : ConstructorArgumentRequest, IEquatable<AutoMockConstructorArgumentRequest>
    {
        public AutoMockConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo) 
            : base(declaringType, parameterInfo)
        {
        }

        public override bool Equals(ConstructorArgumentRequest other)
            => other is AutoMockConstructorArgumentRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockConstructorArgumentRequest other)
            => base.Equals((ConstructorArgumentRequest)other); // Force the correct overload
    }
}
