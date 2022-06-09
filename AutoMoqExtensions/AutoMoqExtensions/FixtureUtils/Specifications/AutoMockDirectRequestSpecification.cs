using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Specifications
{
    internal class AutoMockDirectRequestSpecification : IRequestSpecification
    {
        private readonly AutoMockableSpecification autoMockableSpecification = new();
        public bool IsSatisfiedBy(object request)
        {
            var mockRequest = request as AutoMockDirectRequest;
            if (mockRequest is null) return false;

            return autoMockableSpecification.IsSatisfiedBy(mockRequest.Request);
        }
    }
}
