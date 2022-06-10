using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class AutoMockPropertyPostprocessor : ISpecimenBuilder
    {
        private static readonly AutoMockableSpecification autoMockableSpecification = new();

        public object? Create(object request, ISpecimenContext context)
        {
            if (request is not AutoMockPropertyRequest mockRequest) return new NoSpecimen();

            var type = mockRequest.PropertyInfo.PropertyType;
            if (autoMockableSpecification.IsSatisfiedBy(type)) return context.Resolve(new AutoMockRequest(type));

            return context.Resolve(mockRequest.PropertyInfo);
        }
    }
}
