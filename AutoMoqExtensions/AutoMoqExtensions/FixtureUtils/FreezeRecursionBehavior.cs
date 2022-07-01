using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils
{    
    public class FreezeRecursionBehavior : ISpecimenBuilderTransformation
    {
        private const int DefaultRecursionDepth = 1;
        private readonly int recursionDepth;

        /// <summary>
        /// Initializes new instance of the <see cref="OmitOnRecursionBehavior" /> class with default recursion depth.
        /// The default recursion depth will omit assignment on first recursion.
        /// </summary>
        public FreezeRecursionBehavior()
            : this(DefaultRecursionDepth)
        {
        }

        /// <summary>
        /// Initializes new instance of the <see cref="OmitOnRecursionBehavior" /> class with specific recursion depth.
        /// </summary>
        /// <param name="recursionDepth">The recursion depth at which the request will be omitted.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">recursionDepth is less than one.</exception>
        public FreezeRecursionBehavior(int recursionDepth)
        {
            if (recursionDepth < 1)
                throw new ArgumentOutOfRangeException(nameof(recursionDepth), "Recursion depth must be greater than 0.");

            this.recursionDepth = recursionDepth;
        }

        /// <summary>
        /// Decorates the supplied <see cref="ISpecimenBuilder" /> with an
        /// <see cref="RecursionGuard"/>.
        /// </summary>
        /// <param name="builder">The builder to decorate.</param>
        /// <returns>
        /// <paramref name="builder" /> decorated with an
        /// <see cref="RecursionGuard" />.
        /// </returns>
        public ISpecimenBuilderNode Transform(ISpecimenBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return new RecursionGuard(builder, new FreezeOnRecursionHandler(), this.recursionDepth);
        }
    }
}
