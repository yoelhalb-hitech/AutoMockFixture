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
    internal class AutoMockConstructorArgumentPostprocessor : ISpecimenBuilder
    {
        private static readonly AutoMockableSpecification autoMockableSpecification = new();

        public object? Create(object request, ISpecimenContext context)
        {
            if (request is not AutoMockConstructorArgumentRequest mockRequest) return new NoSpecimen();

            var type = mockRequest.ParameterInfo.ParameterType;

            if (!autoMockableSpecification.IsSatisfiedBy(type))
            {
                var result = context.Resolve(mockRequest.ParameterInfo);
                mockRequest.SetResult(result);
                return result;
            }

            var specimen = context.Resolve(new AutoMockRequest(type, mockRequest));

            if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            {
                mockRequest.SetResult(specimen);
                return specimen;
            }                

            mockRequest.Completed();
            return specimen;
        }
        }
    }
}
