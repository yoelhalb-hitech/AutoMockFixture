using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils.Specifications;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.AutoMockUtils
{
    internal class AutoMockRelay : ISpecimenBuilder
    {
        public AutoMockRelay()
                 : this(new AutoMockableSpecification())
        {
        }

        public AutoMockRelay(IRequestSpecification mockableSpecification)
        {
            this.MockableSpecification = mockableSpecification ?? throw new ArgumentNullException(nameof(mockableSpecification));
        }

        public IRequestSpecification MockableSpecification { get; }

        public object? Create(object request, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!this.MockableSpecification.IsSatisfiedBy(request))
                return new NoSpecimen();


            var t = request as Type ?? (request as SeededRequest)?.Request as Type;
            if (t is null)
                return new NoSpecimen();

            try
            {
                if(!t.IsInterface && !t.IsAbstract)
                {
                    var dependeciesRequest = new AutoMockDependenciesRequest(t, null);
                    var dependeciesResult = context.Resolve(dependeciesRequest);

                    if (dependeciesResult is not NoSpecimen)
                    {
                        dependeciesRequest.SetResult(dependeciesResult);
                        return dependeciesResult;
                    }
                }
            }
            catch { }
            
            if (!AutoMockHelpers.IsAutoMock(t)) t = AutoMockHelpers.GetAutoMockType(t);
            using var directRequest = new AutoMockDirectRequest(t, null); // We do direct to bypass the specification test
            
            var result = context.Resolve(directRequest);

            // Note: null is a valid specimen (e.g., returned by NullRecursionHandler)
            if (result is NoSpecimen || result is OmitSpecimen || result is null)
                return result;

            if (!t.IsAssignableFrom(result.GetType())) return new NoSpecimen();

            directRequest.SetResult(result);
            return result;            
        }
    }
}
