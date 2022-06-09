using AutoFixture.Kernel;
using AutoMoqExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Commands
{
    internal class CustomAutoPropertiesCommand : ISpecimenCommand
    {
        public Type? ExplicitSpecimenType { get; }
        public IRequestSpecification? Specification { get; }
        public CustomAutoPropertiesCommand()
            : this(new TrueRequestSpecification())
        {
            ExplicitSpecimenType = null;
        }

        public CustomAutoPropertiesCommand(IRequestSpecification specification)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
            ExplicitSpecimenType = null;
        }

        public CustomAutoPropertiesCommand(Type specimenType, IRequestSpecification specification)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
            ExplicitSpecimenType = specimenType ?? throw new ArgumentNullException(nameof(specimenType));
        }

        public CustomAutoPropertiesCommand(Type specimenType)
        {
            ExplicitSpecimenType = specimenType ?? throw new ArgumentNullException(nameof(specimenType));
        }

        public virtual void Execute(object specimen, ISpecimenContext context)
        {
            if (specimen == null) throw new ArgumentNullException(nameof(specimen));
            if (context == null) throw new ArgumentNullException(nameof(context));

            foreach (var pi in GetPropertiesWithSet(specimen))
            {
                try
                {
                    // If is static and it is already set then no need to set again
                    if (pi.GetMethod.IsStatic && pi.GetValue(null) != pi.PropertyType.GetDefault()) continue;

                    HandleProperty(specimen, context, pi);
                }
                catch { }
            }

            foreach (var fi in GetFields(specimen))
            {
                try
                {
                    // If is static and it is already set then no need to set again
                    if (fi.IsStatic && fi.GetValue(null) != fi.FieldType.GetDefault()) continue;

                    HandleField(specimen, context, fi);
                }
                catch { }
            }
        }

        protected virtual void HandleProperty(object specimen, ISpecimenContext context, PropertyInfo pi)
        {
            var propertyValue = context.Resolve(pi);
            if (!(propertyValue is OmitSpecimen))
                pi.SetValue(specimen, propertyValue, null);
        }

        protected virtual void HandleField(object specimen, ISpecimenContext context, FieldInfo fi)
        {
            var fieldValue = context.Resolve(fi);
            if (!(fieldValue is OmitSpecimen))
                fi.SetValue(specimen, fieldValue);
        }

        protected virtual Type GetSpecimenType(object specimen)
        {
            if (specimen == null) throw new ArgumentNullException(nameof(specimen));

            if (ExplicitSpecimenType is not null) return ExplicitSpecimenType;

            return specimen.GetType();
        }
       
        // We need to handle internal properties as the outside code might depend on it
        // However no need for private protected properties as it is only used if callbase and if so the base should set it...
        protected IEnumerable<FieldInfo> GetFields(object specimen)
        {
            return from fi in GetSpecimenType(specimen).GetTypeInfo().GetAllFields()
                   where !fi.IsInitOnly
                   && fi.IsPublicOrInternal()
                   && Specification?.IsSatisfiedBy(fi) != false
                   select fi;
        }

        protected IEnumerable<PropertyInfo> GetPropertiesWithSet(object specimen)
        {
            return from pi in GetSpecimenType(specimen).GetTypeInfo().GetAllProperties()
                   where pi.SetMethod != null
                   && pi.SetMethod.IsPublicOrInternal()
                   && pi.GetIndexParameters().Length == 0
                   && Specification?.IsSatisfiedBy(pi) != false
                   select pi;
        }


    }
}
