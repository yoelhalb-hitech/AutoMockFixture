using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    internal class PostprocessorWithRecursion  : ISpecimenBuilder
    {
        public PostprocessorWithRecursion(ISpecimenBuilder builder, ISpecimenCommand command, IRequestSpecification? specification = null)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (command is null) throw new ArgumentNullException(nameof(command));

            Builder = builder;
            Command = command;
            Specification = specification ?? new TrueRequestSpecification();
        }

        public ISpecimenBuilder Builder { get; }
        public ISpecimenCommand Command { get; }
        public IRequestSpecification Specification { get; }

        public object? Create(object request, ISpecimenContext context)
        {
            var specimen = this.Builder.Create(request, context);
            if (specimen == null)
                return specimen;

            var ns = specimen as NoSpecimen;
            if (ns != null)
                return ns;

            if (!this.Specification.IsSatisfiedBy(request))
                return specimen;

            var type = request as Type ?? (request as IRequestWithType)?.Request;
            if (type is not null && context is RecursionContext recursionContext 
                    && recursionContext.BuilderCache.ContainsKey(type)) return specimen; // We are in recursion so no commands

            this.Command.Execute(specimen, context);
            return specimen;
        }
    }
}
