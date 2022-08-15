using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders
{
    internal class DelegateBuilder : NonConformingBuilder
    {
        public override Type[] SupportedTypes => new[] 
        {
            typeof(System.Delegate),
            typeof(System.MulticastDelegate),
        };

        public override int Repeat => 1;

        protected override object GetInnerSpecimens(IRequestWithType originalRequest, ISpecimenContext context)
        {
            if (originalRequest.Request.IsGenericType) return new NoSpecimen();

            return new object[] { new object[] { GetInnerSpecimen(typeof(Action), originalRequest, context) } };
        }

        public override object CreateResult(Type requestType, object[][] innerResults)
        {
            return innerResults.First().First();
        }
    }
}
