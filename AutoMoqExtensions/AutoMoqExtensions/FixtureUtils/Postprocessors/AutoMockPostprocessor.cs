using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Specifications;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal class AutoMockPostprocessor : ISpecimenBuilder
    {
        private static AutoMockDirectRequestSpecification requestSpecification = new();
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMockPostprocessor"/> class with the
        /// supplied <see cref="ISpecimenBuilder"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder which is expected to create <see cref="AutoMock{T}"/> instances.
        /// </param>
        public AutoMockPostprocessor(ISpecimenBuilder builder)
        {
            this.Builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        /// <summary>
        /// Gets the builder decorated by this instance.
        /// </summary>
        public ISpecimenBuilder Builder { get; }

        /// <summary>
        /// Modifies a <see cref="AutoMock{T}"/> instance created by <see cref="Builder"/>.
        /// </summary>
        /// <param name="request">The request that describes what to create.</param>
        /// <param name="context">A context that can be used to create other specimens.</param>
        /// <returns>
        /// The specimen created by <see cref="Builder"/>. If the instance is a correct
        /// <see cref="AutoMock{T}"/> instance, this instance modifies it before returning it.
        /// </returns>
        public object? Create(object request, ISpecimenContext context)
        {
            if (!requestSpecification.IsSatisfiedBy(request) || request is not AutoMockDirectRequest mockRequest)
                return new NoSpecimen();

            var specimen = this.Builder.Create(request, context);
            if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
                return specimen;

            var type = specimen.GetType();
            if (!AutoMockHelpers.IsAutoMock(type) || type != mockRequest.Request) return new NoSpecimen();

            ((Mock)specimen).DefaultValue = DefaultValue.Mock;

            return specimen;
        }
    }
}
