using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders
{
    internal class RecursionBuilder : ISpecimenBuilder
    {
        public RecursionBuilder(){}


        public object? Create(object request, ISpecimenContext context)
        {
            if (context is not RecursionContext recursionContext)
                return new NoSpecimen();

            var type = request as Type ?? (request as IRequestWithType)?.Request;
            if(type is null) return new NoSpecimen();

            if (recursionContext.BuilderCache.ContainsKey(type))
                return recursionContext.BuilderCache[type];
            
            return new NoSpecimen();
        }
    }
}
