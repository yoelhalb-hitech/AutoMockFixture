using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal class AutoMockMethodInvoker : ISpecimenBuilder
    {
        
        public AutoMockMethodInvoker(IMethodQuery query)
        {
            this.Query = query ?? throw new ArgumentNullException(nameof(query));
        }

        /// <summary>
        /// Gets the <see cref="IMethodQuery"/> that defines which constructors will be
        /// attempted.
        /// </summary>
        public IMethodQuery Query { get; }

        /// <summary>
        /// Creates a specimen of the requested type by invoking the first constructor or method it
        /// can satisfy.
        /// </summary>
        /// <param name="request">The request that describes what to create.</param>
        /// <param name="context">A context that can be used to create other specimens.</param>
        /// <returns>
        /// A specimen generated from a method of the requested type, if possible;
        /// otherwise, <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses the first constructor or method returned by <see cref="Query"/> where
        /// <paramref name="context"/> can create values for all parameters.
        /// </para>
        /// </remarks>
        public object Create(object request, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var mockRequest = request as AutoMockDirectRequest;
            var dependencyRequest = request as AutoMockDependenciesRequest;

            if (mockRequest is null && dependencyRequest is null) return new NoSpecimen();

            var type = mockRequest?.Request ?? dependencyRequest?.Request;
            foreach (var ci in Query.SelectMethods(type))
            {
                var paramValues = ci.Parameters
                                    .Select(pi => new AutoMockConstructorArgumentRequest(type!, pi))
                                    .Select(r =>
                                    {
                                        var r1 = context.Resolve(r);
                                        return r1;
                                    })
                                    .ToList();

                if (paramValues.All(v => v is not NoSpecimen && v is not OmitSpecimen))
                    return ci.Invoke(paramValues.ToArray());
            }

            return new NoSpecimen();
        }
    }
}
