using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.Extensions;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Builders.MainBuilders
{
    internal class NonAutoMockBuilder : ISpecimenBuilder
    {        
        public NonAutoMockBuilder(ISpecimenBuilder builder)
        {
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public ISpecimenBuilder Builder { get; }

        public object? Create(object request, ISpecimenContext context)
        {
            if (request is not NonAutoMockRequest nonMockRequest)
                return new NoSpecimen();

            if (!AutoMockHelpers.IsAutoMockAllowed(nonMockRequest.Request)) // Basically all types that we want to leave for AutoFixture
            {
                // Note that IEnumerable etc. should already be handled in the special builders
                var result = context.Resolve(nonMockRequest.Request);
                nonMockRequest.SetResult(result);
                return result;
            }

            var specimen = Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen)
                return specimen;

            nonMockRequest.SetResult(specimen);

            return specimen;
        }
    }
}
