using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class AutoMockConstructorArgumentPostprocessor : ISpecimenBuilder
    {
        private static readonly AutoMockConstructorArgumentSpecification autoMockCtorArgSpecification = new();
        
        public object? Create(object request, ISpecimenContext context)
        {
            if (!autoMockCtorArgSpecification.IsSatisfiedBy(request) 
                    || request is not AutoMockConstructorArgumentRequest mockRequest)
                        return new NoSpecimen();

            var type = mockRequest.ParameterInfo.ParameterType;
            var newRequest = new AutoMockRequest(type);

            return context.Resolve(newRequest);            
        }
    }
}
