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
            
            var mockRequest = new AutoMockRequest(t);
            var result = context.Resolve(mockRequest);

            // Note: null is a valid specimen (e.g., returned by NullRecursionHandler)
            if (result is NoSpecimen || result is OmitSpecimen || result is null)
                return result;

            return t.IsAssignableFrom(result.GetType()) ? result :  new NoSpecimen();
        }
    }
}
