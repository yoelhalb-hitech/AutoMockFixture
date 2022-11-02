using AutoMoqExtensions.FixtureUtils.Requests;

namespace AutoMoqExtensions.FixtureUtils.Postprocessors
{
    // AutoFixture has special handling for `Postprocessor` but still uses it as `ISpecimenBuilder`
    // AutoFixture expects `AutoPropertiesCommand` to be a single command, so we have to stuff anoything extra in an extra
    internal class PostprocessorWithRecursion  : Postprocessor, ISpecimenBuilder, ISpecimenBuilderNode
    {
        public PostprocessorWithRecursion(ISpecimenBuilder builder, ISpecimenCommand command,
                IRequestSpecification? specification = null, ISpecimenCommand? extraCommand = null)
                    : base(builder, command, specification ?? new TrueRequestSpecification())
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (command is null) throw new ArgumentNullException(nameof(command));
            ExtraCommand = extraCommand;
        }

        public ISpecimenCommand? ExtraCommand { get; }

        public new object? Create(object request, ISpecimenContext context)
        {
            return ((ISpecimenBuilder)this).Create(request, context);
        }

        public override ISpecimenBuilderNode Compose(IEnumerable<ISpecimenBuilder> builders)
        {
            var result = base.Compose(builders);

            if (result is not Postprocessor pp) return result;

            return new PostprocessorWithRecursion(pp.Builder, pp.Command, pp.Specification);
        }

        object? ISpecimenBuilder.Create(object request, ISpecimenContext context)
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

            if (ExtraCommand is not null) ExtraCommand.Execute(specimen, context);
            this.Command.Execute(specimen, context);
            return specimen;
        }
    }
}
