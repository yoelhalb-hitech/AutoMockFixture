using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class AutoMockRequestPostprocessor : ISpecimenBuilder
    {
        private static readonly AutoMockRequestSpecification autoMockRequestSpecification = new();
        
        public object? Create(object request, ISpecimenContext context)
        {            
            if (!autoMockRequestSpecification.IsSatisfiedBy(request) || request is not AutoMockRequest mockRequest)
                        return new NoSpecimen();

            var type = mockRequest.Request;
            if (!AutoMockHelpers.IsAutoMock(type)) type = AutoMockHelpers.GetAutoMockType(type);

            var directRequest = new AutoMockDirectRequest(type, mockRequest);

            var specimen = context.Resolve(directRequest);
            if(specimen is null)
            {
                mockRequest.SetResult(null);
                return specimen;
            }

            var t = specimen.GetType();
            if (specimen is NoSpecimen || specimen is OmitSpecimen || !AutoMockHelpers.IsAutoMock(t) || t != type)
            {
                // Try to unwrap it and see if we can get anything
                var unwrapResult = context.Resolve(AutoMockHelpers.GetMockedType(type));

                mockRequest.SetResult(unwrapResult);
                return unwrapResult;
            }

            var result = AutoMockHelpers.GetFromObj(specimen)!.GetMocked();
            mockRequest.Completed();

            return result;
        }
    }
}
