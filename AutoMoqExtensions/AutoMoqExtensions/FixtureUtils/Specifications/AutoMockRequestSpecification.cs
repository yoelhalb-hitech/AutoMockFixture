using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Specifications
{
    internal class AutoMockRequestSpecification : IRequestSpecification
    {
        private readonly AutoMockableSpecification autoMockableSpecification = new();
        public bool IsSatisfiedBy(object request)
        {
            var mockRequest = request as AutoMockRequest;
            if (mockRequest is null) return false;

            return autoMockableSpecification.IsSatisfiedBy(mockRequest.Request);
        }
    }
}
