using AutoFixture.Kernel;
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

            var specimen = Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen)
                return specimen;

            nonMockRequest.SetResult(specimen);

            return specimen;
        }
    }
}
