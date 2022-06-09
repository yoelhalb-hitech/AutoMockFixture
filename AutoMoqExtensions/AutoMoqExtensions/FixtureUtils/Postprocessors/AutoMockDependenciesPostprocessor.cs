using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class AutoMockDependenciesPostprocessor : ISpecimenBuilder
    {

        public AutoMockDependenciesPostprocessor(ISpecimenBuilder builder)
        {
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public ISpecimenBuilder Builder { get; }

        public object? Create(object request, ISpecimenContext context)
        {
            if (request is not AutoMockDependenciesRequest dependencyRequest)
                return new NoSpecimen();

            var specimen = Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen 
                || specimen is null || specimen.GetType() == dependencyRequest.Request)
                return specimen;

            return new NoSpecimen();
        }
    }
}
