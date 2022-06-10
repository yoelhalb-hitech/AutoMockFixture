using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    public class CorrectPostprocessor : ISpecimenBuilder
    {
        // The built in Postprocessor has a bug in that it creates and returns the specimen even if the specification doesn't match
        // It only doesn't execute commands which is not what we expect

        public CorrectPostprocessor(ISpecimenBuilder builder, ISpecimenCommand command, IRequestSpecification? specification = null)
        {
            if(builder is null) throw new ArgumentNullException(nameof(builder));
            if(command is null) throw new ArgumentNullException(nameof(command));

            Builder = builder;
            Command = command;
            Specification = specification ?? new TrueRequestSpecification();
        }

        public ISpecimenBuilder Builder { get; }
        public ISpecimenCommand Command { get; }
        public IRequestSpecification Specification { get; }

        public object Create(object request, ISpecimenContext context)
        {
            if (!this.Specification.IsSatisfiedBy(request)) return new NoSpecimen();

            var specimen = this.Builder.Create(request, context);
            Command.Execute(specimen, context);

            return specimen;
        }
    }
}
