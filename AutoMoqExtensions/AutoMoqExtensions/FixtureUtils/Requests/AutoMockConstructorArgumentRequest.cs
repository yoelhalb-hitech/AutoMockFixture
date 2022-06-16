using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    internal class AutoMockConstructorArgumentRequest 
        : ConstructorArgumentRequest, IEquatable<AutoMockConstructorArgumentRequest>
    {
        public AutoMockConstructorArgumentRequest(Type declaringType, ParameterInfo parameterInfo, ITracker? tracker) 
            : base(declaringType, parameterInfo, tracker)
        {
        }

        public override void SetResult(object? result)
        {
            if(result is not null && AutoMockHelpers.GetFromObj(result) is not null)
            {
                base.SetResult(result);
                return;
            }
            var type = result?.GetType();
            if (type is null || type.IsPrimitive || type == typeof(string) || type.IsEnum ||
                    type.GetPublicAndProtectedConstructors().All(c => !c.GetParameters().Any())) // Really we could have only cheked for one as normally take the least, but in case the user will ask for something that does have
            {
                // We want to avoid setting the same path many times which will cause problems
                // This can happen if the code is calling a factory method multiple times from the same method, but passing different arguments to the method
                // So at least not saving things that we know will probably not arrive at an AutoMock anyway...
                base.SetCompleted();
                return;
            }

            base.SetResult(result);
        }

        public override bool Equals(ConstructorArgumentRequest other)
            => other is AutoMockConstructorArgumentRequest r && this.Equals(r);

        public virtual bool Equals(AutoMockConstructorArgumentRequest other)
            => base.Equals((ConstructorArgumentRequest)other); // Force the correct overload
    }
}
