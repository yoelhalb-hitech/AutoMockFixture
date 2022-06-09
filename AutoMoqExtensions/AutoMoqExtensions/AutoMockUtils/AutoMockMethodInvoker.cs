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

            foreach (var ci in this.GetConstructors((request as Type) ?? (request as AutoMockDirectRequest)?.Request))
            {
                var paramValues = (from pi in ci.Parameters
                                    select ResolveParameter(context, pi)).ToList();

                if (paramValues.All(IsValueValid))
                {
                    return ci.Invoke(paramValues.ToArray());
                }
            }

            return new NoSpecimen();
        }

        private object ResolveParameter(ISpecimenContext context, ParameterInfo parameter)
        {
            if(!AutoMockHelpers.IsAutoMockAllowed(parameter.ParameterType))
            {
                return context.Resolve(parameter);
            }

            try
            {
                var newParam = new AutoMockParameterInfo(parameter);
                var specimen = context.Resolve(newParam);

                if (specimen is NoSpecimen || specimen is OmitSpecimen || specimen is null)
                    return context.Resolve(parameter);

                return specimen;
            }
            catch
            {
                   
                return context.Resolve(parameter);
            }
        }


        private IEnumerable<IMethod> GetConstructors(object request)
        {
            var requestedType = request as Type;
            if (requestedType == null)
            {
                return Enumerable.Empty<IMethod>();
            }

            return this.Query.SelectMethods(requestedType);
        }

        private static bool IsValueValid(object value)
        {
            return !(value is NoSpecimen)
                && !(value is OmitSpecimen);
        }
    }
}
