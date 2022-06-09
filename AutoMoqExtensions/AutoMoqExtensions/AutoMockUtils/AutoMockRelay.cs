using AutoFixture.Kernel;
using AutoMoqExtensions.AutoMockUtils.Specifications;
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

            var t = request as Type;
            if (t == null)
                return new NoSpecimen();

            var type = AutoMockHelpers.GetAutoMockType(t);
            var result = context.Resolve(type);

            // Note: null is a valid specimen (e.g., returned by NullRecursionHandler)
            if (result is NoSpecimen || result is OmitSpecimen || result == null)
                return result;

            var m = result as IAutoMock;
            if (m == null)
                return new NoSpecimen();

            return m;
        }
    }
}
