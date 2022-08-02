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
            if (request is not PropertyRequest propRequest) return new NoSpecimen();

            var type = propRequest.PropertyInfo.PropertyType;
            if (!autoMockableSpecification.IsSatisfiedBy(type) || !propRequest.ShouldAutoMock)
            {
                object newRequest = propRequest.IsInAutoMockChain || propRequest.IsInAutoMockDepnedencyChain
                                        ? new AutoMockDependenciesRequest(type, propRequest)
                                        : new NonAutoMockRequest(type, propRequest);
                var result = context.Resolve(newRequest);
                propRequest.SetResult(result);
                return result;
            }

            var specimen = context.Resolve(new AutoMockRequest(type, propRequest));

            if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
            {
                propRequest.SetResult(specimen);
                return specimen;
            }

            propRequest.SetCompleted();

            return specimen;
        }
    }
}
